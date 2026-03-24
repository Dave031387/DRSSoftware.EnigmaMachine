namespace DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// A service used for embedding/extracting the Enigma configuration into/out of a text string.
/// </summary>
/// <param name="indicatorStringGenerator">
/// A reference to an object used for generating indicator strings.
/// </param>
internal sealed class EmbeddingService(IIndicatorStringGenerator indicatorStringGenerator) : IEmbeddingService
{
    /// <summary>
    /// Holds a reference to the indicator string generator.
    /// </summary>
    private readonly IIndicatorStringGenerator _indicatorStringGenerator = indicatorStringGenerator;

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
        if (string.IsNullOrEmpty(inputText))
        {
            return string.Empty;
        }

        if (HasIndicatorString(inputText))
        {
            return inputText;
        }

        List<char> embeddedChars = [];
        int inputTextIndex = 0;
        bool endOfInputText = false;
        string seedValue = configuration.SeedValue;
        int seedValueIndex = 0;
        bool endOfSeedValue = false;
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
        int indexValuesLength = configuration.NumberOfRotors + 1;
        int indexValuesIndex = 0;
        bool endOfIndexValues = false;

        embeddedChars.AddRange(_indicatorStringGenerator.GetIndicatorString(EmbeddingIndicatorChar));

        while (!(endOfIndexValues && endOfSeedValue && endOfInputText))
        {
            if (!endOfInputText)
            {
                embeddedChars.AddRange(EmbedNextChar(inputText, ref inputTextIndex, ref endOfInputText, ref endOfSeedValue, ref endOfIndexValues));
            }

            if (!endOfSeedValue)
            {
                embeddedChars.AddRange(EmbedNextChar(seedValue, ref seedValueIndex, ref endOfSeedValue, ref endOfInputText, ref endOfIndexValues));
            }

            if (!endOfIndexValues)
            {
                embeddedChars.AddRange(EmbedIndexValues(indexValues, indexValuesLength, ref indexValuesIndex, ref endOfIndexValues, ref endOfInputText, ref endOfSeedValue));
            }
        }

        return new([.. embeddedChars]);
    }

    /// <summary>
    /// Extract the Enigma machine configuration from the given input text.
    /// </summary>
    /// <remarks>
    /// The original input text will be returned along with a null Enigma machine configuration if
    /// it is found that the input text doesn't begin with the required embedding indicator
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
    public string Extract(string embeddedText, out EnigmaConfiguration? configuration)
    {
        configuration = null;

        if (string.IsNullOrEmpty(embeddedText) || embeddedText.Length < MinEmbeddedStringSize || !HasIndicatorString(embeddedText))
        {
            return embeddedText ?? string.Empty;
        }

        int embeddedTextIndex = IndicatorSize;
        List<char> inputChars = [];
        bool endOfInputText = false;
        List<char> seedChars = [];
        bool endOfSeedValue = false;
        List<int> indexValues = [];
        bool endOfIndexValues = false;

        do
        {
            if (!endOfInputText)
            {
                inputChars.AddRange(ExtractNextChar(embeddedText, ref embeddedTextIndex, ref endOfInputText));
            }

            if (!endOfSeedValue)
            {
                seedChars.AddRange(ExtractNextChar(embeddedText, ref embeddedTextIndex, ref endOfSeedValue));
            }

            if (!endOfIndexValues)
            {
                (int indexValue, bool hasValue) = ExtractIndexValue(embeddedText, ref embeddedTextIndex, ref endOfIndexValues);

                if (hasValue)
                {
                    indexValues.Add(indexValue);
                }
            }
        } while (embeddedTextIndex < embeddedText.Length);

        int numberOfRotors = indexValues.Count - 1;

        // We assume that everything is okay as long as the number of rotors falls within the valid
        // range and the seed string is at least the minimum valid length. This does not, however,
        // ensure that there aren't any other problems with the extracted configuration.
        if (numberOfRotors is >= MinRotorCount and <= MaxRotorCount && seedChars.Count >= MinStringLength)
        {
            configuration = new()
            {
                NumberOfRotors = numberOfRotors,
                ReflectorIndex = indexValues[0],
                RotorIndex1 = numberOfRotors > 0 ? indexValues[1] : 0,
                RotorIndex2 = numberOfRotors > 1 ? indexValues[2] : 0,
                RotorIndex3 = numberOfRotors > 2 ? indexValues[3] : 0,
                RotorIndex4 = numberOfRotors > 3 ? indexValues[4] : 0,
                RotorIndex5 = numberOfRotors > 4 ? indexValues[5] : 0,
                RotorIndex6 = numberOfRotors > 5 ? indexValues[6] : 0,
                RotorIndex7 = numberOfRotors > 6 ? indexValues[7] : 0,
                RotorIndex8 = numberOfRotors > 7 ? indexValues[8] : 0,
                SeedValue = new([.. seedChars]),
                UseEmbeddedConfiguration = false
            };
            return new([.. inputChars]);
        }

        return EmbeddingExtractErrorMessage;
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
        if (inputText is null || inputText.Length < IndicatorSize)
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
    /// Embed the next rotor or reflector index value into the embedded output string.
    /// </summary>
    /// <param name="indexValues">
    /// The list of rotor and reflector index values that are being embedded.
    /// </param>
    /// <param name="indexValuesLength">
    /// The number of reflector and rotor index values stored in the <paramref name="indexValues" />
    /// array.
    /// </param>
    /// <param name="indexValuesIndex">
    /// The index position of the current value in the <paramref name="indexValues" /> array.
    /// </param>
    /// <param name="endOfIndexValues">
    /// A value that indicates whether or not we have reached the end of the list of reflector and
    /// rotor index values.
    /// </param>
    /// <param name="endOfOther1">
    /// A value indicating whether or not we have reached the end of the next component of the
    /// embedded output text string.
    /// </param>
    /// <param name="endOfOther2">
    /// A value indicating whether or not we have reached the end of the last component of the
    /// embedded output text string.
    /// </param>
    /// <returns>
    /// The string representation of the next index value to be embedded into the output string. Or,
    /// a delimiter character if we have reached the end of the list of index values but not the end
    /// of the input text or seed value.
    /// </returns>
    private static string EmbedIndexValues(int[] indexValues, int indexValuesLength, ref int indexValuesIndex, ref bool endOfIndexValues, ref bool endOfOther1, ref bool endOfOther2)
    {
        if (indexValuesIndex == indexValuesLength)
        {
            endOfIndexValues = true;

            if (endOfOther1 && endOfOther2)
            {
                return string.Empty;
            }

            return new([DelimiterChar]);
        }
        else
        {
            return new([(char)(indexValues[indexValuesIndex++] + MinChar)]);
        }
    }

    /// <summary>
    /// Embed the next character into the embedded output text string.
    /// </summary>
    /// <param name="textValue">
    /// The text value from which to retrieve the next character.
    /// </param>
    /// <param name="textIndex">
    /// The current index position within the <paramref name="textValue" />.
    /// </param>
    /// <param name="isEndOfTextValue">
    /// A value indicating whether or not we have reached the end of the
    /// <paramref name="textValue" />.
    /// </param>
    /// <param name="isEndOfOther1">
    /// A value indicating whether or not we have reached the end of the next component of the
    /// embedded output text string.
    /// </param>
    /// <param name="isEndOfOther2">
    /// A value indicating whether or not we have reached the end of the last component of the
    /// embedded output text string.
    /// </param>
    /// <returns>
    /// The next character from <paramref name="textValue" /> that is to be added to the embedded
    /// output text string. <br /> If <paramref name="textIndex" /> is beyond the end of
    /// <paramref name="textValue" /> then a delimiter character will be returned if we haven't
    /// reached the end of the other two components. Otherwise, an empty string will be returned.
    /// </returns>
    private static string EmbedNextChar(string textValue, ref int textIndex, ref bool isEndOfTextValue, ref bool isEndOfOther1, ref bool isEndOfOther2)
    {
        if (textIndex == textValue.Length)
        {
            isEndOfTextValue = true;

            if (isEndOfOther1 && isEndOfOther2)
            {
                return string.Empty;
            }

            return new([DelimiterChar]);
        }
        else
        {
            char nextChar = textValue[textIndex++];

            if (IsCarriageReturnLineFeed(nextChar, textValue, textIndex))
            {
                textIndex++;
                return CRLF;
            }

            return new([nextChar]);
        }
    }

    /// <summary>
    /// Extract the next rotor or reflector index value from the embedded text string.
    /// </summary>
    /// <param name="embeddedText">
    /// The embedded text string that we're extracting the index value from.
    /// </param>
    /// <param name="embeddedTextIndex">
    /// The index position of the current character in the embedded text string.
    /// </param>
    /// <param name="endOfIndexValues">
    /// A value indicating whether or not we have reached the end of the index values within the
    /// embedded text string.
    /// </param>
    /// <returns>
    /// A tuple containing an integer representing the extracted index value, and a boolean
    /// indicating whether or not we actually extracted a value. The boolean value will be set to
    /// false if we find a delimiter character instead of an index value.
    /// </returns>
    private static (int indexValue, bool hasValue) ExtractIndexValue(string embeddedText, ref int embeddedTextIndex, ref bool endOfIndexValues)
    {
        char nextIndexChar = embeddedText[embeddedTextIndex++];

        if (nextIndexChar is DelimiterChar)
        {
            endOfIndexValues = true;
            return (0, false);
        }
        else
        {
            return (nextIndexChar - MinChar, true);
        }
    }

    /// <summary>
    /// Extract the next character from the embedded text string.
    /// </summary>
    /// <param name="embeddedText">
    /// The embedded text from which we are extracting the next character.
    /// </param>
    /// <param name="embeddedTextIndex">
    /// The index position of the current character in the embedded text string.
    /// </param>
    /// <param name="isEndOfString">
    /// A boolean value that is set to <see langword="true" /> if the next character happens to be a
    /// delimiter character.
    /// </param>
    /// <returns>
    /// An empty string if the next character happens to be a delimiter character. Otherwise, the
    /// next character converted to a string value.
    /// </returns>
    private static string ExtractNextChar(string embeddedText, ref int embeddedTextIndex, ref bool isEndOfString)
    {
        char nextChar = embeddedText[embeddedTextIndex++];

        if (nextChar is DelimiterChar)
        {
            isEndOfString = true;
            return string.Empty;
        }
        else
        {
            if (IsCarriageReturnLineFeed(nextChar, embeddedText, embeddedTextIndex))
            {
                embeddedTextIndex++;
                return CRLF;
            }

            return new([nextChar]);
        }
    }

    /// <summary>
    /// Determine if the current position in the given text string is the start of a Carriage
    /// Return/Line Feed pair.
    /// </summary>
    /// <param name="firstChar">
    /// The first character in the potential CR/LF pair.
    /// </param>
    /// <param name="textValue">
    /// The text string that is being checked.
    /// </param>
    /// <param name="textIndex">
    /// The index of the next character position in the text string.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the current position in the given text string contains a Carriage
    /// Return/Line Feed pair.
    /// </returns>
    private static bool IsCarriageReturnLineFeed(char firstChar, string textValue, int textIndex)
        => firstChar is CarriageReturn && textIndex < textValue.Length && textValue[textIndex] is LineFeed;
}