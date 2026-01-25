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
    /// Holds a reference to an object used for generating cryptographically secure random integer
    /// values.
    /// </summary>
    private readonly ISecureNumberGenerator _numberGenerator = numberGenerator;

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
        int inputTextLength = inputText.Length;
        int inputTextIndex = 0;
        bool endOfInputText = false;
        string seedValue = configuration.SeedValue;
        int seedValueLength = configuration.SeedValue.Length;
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

        // Ad the embedded indicator string to the beginning of the output text string.
        result.AddRange(GenerateIndicatorString());

        while (!(endOfIndexValues && endOfSeedValue && endOfInputText))
        {
            // Embed the next input text character to the result. Carriage return/line feed
            // characters are first replace with a single MaxChar before embedding.
            if (!endOfInputText)
            {
                if (inputTextIndex >= inputTextLength)
                {
                    endOfInputText = true;

                    if (!(endOfIndexValues && endOfSeedValue))
                    {
                        // Add a DelimiterChar to the result to indicate that we have reached the
                        // end of the input text.
                        result.Add(DelimiterChar);
                    }
                }
                else
                {
                    char nextInputChar = inputText[inputTextIndex++];

                    if (IsCarriageReturnLineFeed(nextInputChar, inputText, inputTextIndex))
                    {
                        result.Add(nextInputChar);
                        nextInputChar = inputText[inputTextIndex++];
                    }

                    result.Add(nextInputChar);
                }
            }

            // Embed the next seed value character to the result. Note that carriage return/line
            // feed pairs are kept together. (This assumes that a carriage return will always be
            // followed by a line feed.)
            if (!endOfSeedValue)
            {
                if (seedValueIndex >= seedValueLength)
                {
                    endOfSeedValue = true;

                    if (!(endOfIndexValues && endOfInputText))
                    {
                        // Add a DelimiterChar to the result to indicate that we have reached the
                        // end of the seed value.
                        result.Add(DelimiterChar);
                    }
                }
                else
                {
                    char nextSeedChar = seedValue[seedValueIndex++];

                    if (IsCarriageReturnLineFeed(nextSeedChar, seedValue, seedValueIndex))
                    {
                        result.Add(nextSeedChar);
                        nextSeedChar = seedValue[seedValueIndex++];
                    }

                    result.Add(nextSeedChar);
                }
            }

            // Embed the next reflector or rotor index value to the result.
            if (!endOfIndexValues)
            {
                if (indexValuesIndex >= indexValuesLength)
                {
                    endOfIndexValues = true;

                    if (!(endOfSeedValue && endOfInputText))
                    {
                        // Add a DelimiterChar value to the result to indicate that we have reached
                        // the end of the reflector and rotor index values.
                        result.Add(DelimiterChar);
                    }
                }
                else
                {
                    result.Add((char)(indexValues[indexValuesIndex++] + MinChar));
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
    /// <param name="inputText">
    /// The input text that contains the embedded configuration.
    /// </param>
    /// <param name="configuration">
    /// The Enigma configuration details that will be returned to the caller.
    /// </param>
    /// <returns>
    /// The remaining input text after the configuration has been extracted. The configuration
    /// itself is returned in the <paramref name="configuration" /> out parameter.
    /// </returns>
    public string Extract(string inputText, out EnigmaConfiguration configuration)
    {
        configuration = new();
        int inputTextLength = inputText.Length;

        if (string.IsNullOrWhiteSpace(inputText) || inputTextLength < IndicatorSize * 2 || !HasIndicatorString(inputText))
        {
            return inputText;
        }

        List<char> outputChars = [];
        List<char> seedChars = [];
        List<int> indexValues = [];
        int inputTextIndex = IndicatorSize * 2;
        bool endOfOutputText = false;
        bool endOfSeedValue = false;
        bool endOfIndexValues = false;

        do
        {
            // Extract the next output text character from the input text. If the character is a
            // carriage return, then also extract the following line feed character.
            if (!endOfOutputText)
            {
                char nextOutputChar = inputText[inputTextIndex++];

                if (nextOutputChar is DelimiterChar)
                {
                    // If the next input text character is a DelimiterChar, then we have already
                    // extracted the last output text character.
                    endOfOutputText = true;
                }
                else
                {
                    if (IsCarriageReturnLineFeed(nextOutputChar, inputText, inputTextIndex))
                    {
                        outputChars.Add(nextOutputChar);
                        nextOutputChar = inputText[inputTextIndex++];
                    }

                    outputChars.Add(nextOutputChar);
                }
            }

            // Extract the next seed value character from the input text. If the character is a
            // carriage return, then also extract the following line feed character.
            if (!endOfSeedValue)
            {
                char nextSeedChar = inputText[inputTextIndex++];

                if (nextSeedChar is DelimiterChar)
                {
                    // If the next input text character is a DelimiterChar, then we have already
                    // extracted the last seed value character.
                    endOfSeedValue = true;
                }
                else
                {
                    if (IsCarriageReturnLineFeed(nextSeedChar, inputText, inputTextIndex))
                    {
                        seedChars.Add(nextSeedChar);
                        nextSeedChar = inputText[inputTextIndex++];
                    }

                    seedChars.Add(nextSeedChar);
                }
            }

            // Extract the next reflector or rotor index value from the input text.
            if (!endOfIndexValues)
            {
                char nextIndexChar = inputText[inputTextIndex++];

                if (nextIndexChar is DelimiterChar)
                {
                    // If the next input text character is a DelimiterChar, then we have already
                    // extracted the last of the reflector and rotor index values.
                    endOfIndexValues = true;
                }
                else
                {
                    indexValues.Add(nextIndexChar - MinChar);
                }
            }
        } while (inputTextIndex < inputTextLength);

        // Save the Enigma configuration.
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

        // Return the input text with the embedded configuration data removed.
        return new([.. outputChars]);
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
        if (inputText.Length < IndicatorSize * 2)
        {
            return false;
        }

        for (int i = 0; i < IndicatorSize; i++)
        {
            int firstCharValue = inputText[i];
            int secondCharValue = inputText[i + IndicatorSize];

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
    /// Generates a random indicator string used for embedding configuration data.
    /// </summary>
    /// <returns>
    /// A new randomly generated indicator string.
    /// </returns>
    private string GenerateIndicatorString()
    {
        char[] indicatorChars = new char[IndicatorSize * 2];

        for (int i = 0; i < IndicatorSize; i++)
        {
            int indicatorValue = _numberGenerator.GetNext(MinChar, EmbeddingIndicatorChar);
            indicatorChars[i] = (char)indicatorValue;
            indicatorChars[i + IndicatorSize] = (char)(EmbeddedIndicatorValue - indicatorValue);
        }

        return new string(indicatorChars);
    }
}