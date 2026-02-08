namespace DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// A service used for embedding/extracting the Enigma configuration into/out of a text string.
/// </summary>
/// <param name="numberGenerator">
/// A reference to an object used for generating cryptographically secure random integer values.
/// </param>
internal sealed class EmbeddingService(ISecureNumberGenerator numberGenerator) : IEmbeddingService
{
    /// <summary>
    /// Lock object used to ensure thread safety.
    /// </summary>
    private readonly Lock _lock = new();

    /// <summary>
    /// Holds a reference to an object used for generating cryptographically secure random integer
    /// values.
    /// </summary>
    private readonly ISecureNumberGenerator _numberGenerator = numberGenerator;

    /// <summary>
    /// Holds a value indicating whether or not we have reached the end of the index values.
    /// </summary>
    private bool _endOfIndexValues;

    /// <summary>
    /// Holds a value indicating whether or not we have reached the end of the input text.
    /// </summary>
    private bool _endOfInputText;

    /// <summary>
    /// Holds a value indicating whether or not we have reached the end of the seed value.
    /// </summary>
    private bool _endOfSeedValue;

    /// <summary>
    /// An index value that points to the current index value in the list of index values.
    /// </summary>
    private int _indexValuesIndex;

    /// <summary>
    /// Holds a value equal to the number of index values (rotors and reflector).
    /// </summary>
    private int _indexValuesLength;

    /// <summary>
    /// An index value that points to the current character in the input text string.
    /// </summary>
    private int _inputTextIndex;

    /// <summary>
    /// An index value that points to the current character in the seed value text string.
    /// </summary>
    private int _seedValueIndex;

    /// <summary>
    /// Embed the Enigma machine configuration details into the given text string.
    /// </summary>
    /// <remarks>
    /// The input text will be returned unchanged if it is determined that it already contains
    /// embedded configuration values.
    /// </remarks>
    /// <param name="inputText">
    /// An encrypted text string into which the configuration will be embedded.
    /// </param>
    /// <param name="configuration">
    /// The configuration details that are to be embedded into the text string.
    /// </param>
    /// <returns>
    /// The input text string with the Enigma machine configuration details embedded into it.
    /// </returns>
    public string Embed(string inputText, EnigmaConfiguration configuration)
    {
        if (HasIndicatorString(inputText))
        {
            return inputText;
        }

        List<char> result = [];
        string seedValue = configuration.SeedValue;
        int[] indexValues =
            [
            configuration.ReflectorIndex,
            configuration.RotorIndex1,
            configuration.RotorIndex2,
            configuration.RotorIndex3,
            configuration.RotorIndex4,
            configuration.RotorIndex5,
            configuration.RotorIndex6,
            configuration.RotorIndex7,
            configuration.RotorIndex8
            ];

        result.AddRange(GenerateIndicatorString());

        lock (_lock)
        {
            _inputTextIndex = 0;
            _endOfInputText = false;
            _seedValueIndex = 0;
            _endOfSeedValue = false;
            _indexValuesLength = configuration.NumberOfRotors + 1;
            _indexValuesIndex = 0;
            _endOfIndexValues = false;

            while (!(_endOfIndexValues && _endOfSeedValue && _endOfInputText))
            {
                if (!_endOfInputText)
                {
                    result.AddRange(EmbedInputText(inputText));
                }

                if (!_endOfSeedValue)
                {
                    result.AddRange(EmbedSeedValue(seedValue));
                }

                if (!_endOfIndexValues)
                {
                    result.AddRange(EmbedIndexValues(indexValues));
                }
            }
        }

        return new([.. result]);
    }

    /// <summary>
    /// Extract the Enigma machine configuration from the given input text.
    /// </summary>
    /// <remarks>
    /// The original input text will be returned along with the default Enigma machine configuration
    /// if it is found that the input text doesn't begin with the required embedding indicator
    /// characters.
    /// </remarks>
    /// <param name="embeddedText">
    /// The input text that contains the embedded configuration.
    /// </param>
    /// <param name="configuration">
    /// The Enigma configuration details that will be returned to the caller.
    /// </param>
    /// <returns>
    /// The remaining input text after the configuration has been extracted. The configuration
    /// itself is returned in the <paramref name="configuration" /> out parameter.
    /// </returns>
    public string Extract(string embeddedText, out EnigmaConfiguration configuration)
    {
        configuration = new();

        if (string.IsNullOrWhiteSpace(embeddedText) || embeddedText.Length < IndicatorSize || !HasIndicatorString(embeddedText))
        {
            return embeddedText;
        }

        List<char> result = [];
        List<char> seedChars = [];
        List<int> indexValues = [];

        lock (_lock)
        {
            _inputTextIndex = IndicatorSize;
            _endOfInputText = false;
            _endOfSeedValue = false;
            _endOfIndexValues = false;

            do
            {
                if (!_endOfInputText)
                {
                    result.AddRange(ExtractInputText(embeddedText));
                }

                if (!_endOfSeedValue)
                {
                    seedChars.AddRange(ExtractSeedValue(embeddedText));
                }

                if (!_endOfIndexValues)
                {
                    (int indexValue, bool hasValue) = ExtractIndexValue(embeddedText);

                    if (hasValue)
                    {
                        indexValues.Add(indexValue);
                    }
                }
            } while (_inputTextIndex < embeddedText.Length);
        }

        int numberOfRotors = indexValues.Count - 1;
        configuration.NumberOfRotors = numberOfRotors;
        configuration.ReflectorIndex = indexValues[0];
        configuration.RotorIndex1 = numberOfRotors > 0 ? indexValues[1] : 0;
        configuration.RotorIndex2 = numberOfRotors > 1 ? indexValues[2] : 0;
        configuration.RotorIndex3 = numberOfRotors > 2 ? indexValues[3] : 0;
        configuration.RotorIndex4 = numberOfRotors > 3 ? indexValues[4] : 0;
        configuration.RotorIndex5 = numberOfRotors > 4 ? indexValues[5] : 0;
        configuration.RotorIndex6 = numberOfRotors > 5 ? indexValues[6] : 0;
        configuration.RotorIndex7 = numberOfRotors > 6 ? indexValues[7] : 0;
        configuration.RotorIndex8 = numberOfRotors > 7 ? indexValues[8] : 0;
        configuration.SeedValue = new([.. seedChars]);
        configuration.UseEmbeddedConfiguration = false;

        return new([.. result]);
    }

    /// <summary>
    /// Determine whether or not the supplied <paramref name="inputText" /> starts with a Embedding
    /// indicator string.
    /// </summary>
    /// <param name="inputText">
    /// The input text to be checked for the indicator string.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the <paramref name="inputText" /> starts with an indicator
    /// string.
    /// </returns>
    public bool HasIndicatorString(string inputText)
    {
        if (inputText.Length < IndicatorSize)
        {
            return false;
        }

        for (int i = 0; i < IndicatorPairs; i++)
        {
            int firstCharValue = inputText[i];
            int secondCharValue = inputText[i + IndicatorPairs];

            if (firstCharValue + secondCharValue != EmbeddedIndicatorValue)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Determine if the current position in the given text string is the start of a Carriage
    /// Return/Line Feed pair.
    /// </summary>
    /// <param name="firstChar">
    /// The first character in the potential CR/LF pair.
    /// </param>
    /// <param name="text">
    /// The text string that is being checked.
    /// </param>
    /// <param name="textIndex">
    /// The index of the next character position in the text string.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the current position in the given text string contains a Carriage
    /// Return/Line Feed pair.
    /// </returns>
    private static bool IsCarriageReturnLineFeed(char firstChar, string text, int textIndex)
        => firstChar is CarriageReturn && textIndex < text.Length && text[textIndex] is LineFeed;

    /// <summary>
    /// Embed the next rotor or reflector index value into the embedded output string.
    /// </summary>
    /// <param name="indexValues">
    /// The list of rotor and reflector index values that are being embedded.
    /// </param>
    /// <returns>
    /// The string representation of the next index value to be embedded into the output string. Or,
    /// a delimiter character if we have reached the end of the list of index values but not the end
    /// of the input text or seed value.
    /// </returns>
    private string EmbedIndexValues(int[] indexValues)
    {
        if (_indexValuesIndex >= _indexValuesLength)
        {
            _endOfIndexValues = true;

            if (!(_endOfSeedValue && _endOfInputText))
            {
                return new([DelimiterChar]);
            }

            return string.Empty;
        }
        else
        {
            return new([(char)(indexValues[_indexValuesIndex++] + MinChar)]);
        }
    }

    /// <summary>
    /// Embed the next character from the input text into the embedded output text.
    /// </summary>
    /// <param name="inputText">
    /// The input text that is being embedded into the output text.
    /// </param>
    /// <returns>
    /// The string representation of the next character (or CR/LF pair) to be embedded into the
    /// output string. Or, a delimiter character if we have reached the end of the input text but
    /// not the end of the seed value or index values.
    /// </returns>
    private string EmbedInputText(string inputText)
    {
        if (_inputTextIndex >= inputText.Length)
        {
            _endOfInputText = true;

            if (!(_endOfIndexValues && _endOfSeedValue))
            {
                return new([DelimiterChar]);
            }

            return string.Empty;
        }
        else
        {
            char nextInputChar = inputText[_inputTextIndex++];

            if (IsCarriageReturnLineFeed(nextInputChar, inputText, _inputTextIndex))
            {
                _inputTextIndex++;
                return CRLF;
            }

            return new([nextInputChar]);
        }
    }

    /// <summary>
    /// Embed the next character from the seed value text into the embedded output text.
    /// </summary>
    /// <param name="seedValue">
    /// The seed value text that is being embedded into the output text.
    /// </param>
    /// <returns>
    /// The string representation of the next character (or CR/LF pair) to be embedded into the
    /// output string. Or, a delimiter character if we have reached the end of the seed value text
    /// but not the end of the input text or index values.
    /// </returns>
    private string EmbedSeedValue(string seedValue)
    {
        if (_seedValueIndex >= seedValue.Length)
        {
            _endOfSeedValue = true;

            if (!(_endOfIndexValues && _endOfInputText))
            {
                return new([DelimiterChar]);
            }

            return string.Empty;
        }
        else
        {
            char nextSeedChar = seedValue[_seedValueIndex++];

            if (IsCarriageReturnLineFeed(nextSeedChar, seedValue, _seedValueIndex))
            {
                _seedValueIndex++;
                return CRLF;
            }

            return new([nextSeedChar]);
        }
    }

    /// <summary>
    /// Extract the next rotor or reflector index value from the embedded text string.
    /// </summary>
    /// <param name="embeddedText">
    /// The embedded text string that we're extracting the index value from.
    /// </param>
    /// <returns>
    /// A tuple containing an integer representing the extracted index value, and a boolean
    /// indicating whether or not we actually extracted a value. The boolean value will be set to
    /// false if we find a delimiter character instead of an index value.
    /// </returns>
    private (int indexValue, bool hasValue) ExtractIndexValue(string embeddedText)
    {
        char nextIndexChar = embeddedText[_inputTextIndex++];

        if (nextIndexChar is DelimiterChar)
        {
            _endOfIndexValues = true;
            return (0, false);
        }
        else
        {
            return (nextIndexChar - MinChar, true);
        }
    }

    /// <summary>
    /// Extract the next input text character from the embedded text string.
    /// </summary>
    /// <param name="embeddedText">
    /// The embedded text from which we are extracting the input text.
    /// </param>
    /// <returns>
    /// A string representing the next input text character (or CR/LF pair) to be added to the input
    /// text string. Or, an empty string if we have found a delimiter character indicating we have
    /// reached the end of the input text.
    /// </returns>
    private string ExtractInputText(string embeddedText)
    {
        char nextOutputChar = embeddedText[_inputTextIndex++];

        if (nextOutputChar is DelimiterChar)
        {
            _endOfInputText = true;
            return string.Empty;
        }
        else
        {
            if (IsCarriageReturnLineFeed(nextOutputChar, embeddedText, _inputTextIndex))
            {
                _inputTextIndex++;
                return CRLF;
            }

            return new([nextOutputChar]);
        }
    }

    /// <summary>
    /// Extract the next seed value character from the embedded text string.
    /// </summary>
    /// <param name="embeddedText">
    /// The embedded text from which we are extracting the seed value.
    /// </param>
    /// <returns>
    /// A string representing the next seed value character (or CR/LF pair) to be added to the seed
    /// value string. Or, an empty string if we have found a delimiter character indicating we have
    /// reached the end of the seed value string.
    /// </returns>
    private string ExtractSeedValue(string embeddedText)
    {
        char nextSeedChar = embeddedText[_inputTextIndex++];

        if (nextSeedChar is DelimiterChar)
        {
            _endOfSeedValue = true;
            return string.Empty;
        }
        else
        {
            if (IsCarriageReturnLineFeed(nextSeedChar, embeddedText, _inputTextIndex))
            {
                _inputTextIndex++;
                return CRLF;
            }

            return new([nextSeedChar]);
        }
    }

    /// <summary>
    /// Generates a random indicator string used for embedding configuration data.
    /// </summary>
    /// <returns>
    /// A new randomly generated indicator string.
    /// </returns>
    private string GenerateIndicatorString()
    {
        char[] indicatorChars = new char[IndicatorSize];

        for (int i = 0; i < IndicatorPairs; i++)
        {
            int indicatorValue = _numberGenerator.GetNext(MinChar, EmbeddingIndicatorChar);
            indicatorChars[i] = (char)indicatorValue;
            indicatorChars[i + IndicatorPairs] = (char)(EmbeddedIndicatorValue - indicatorValue);
        }

        return new string(indicatorChars);
    }
}