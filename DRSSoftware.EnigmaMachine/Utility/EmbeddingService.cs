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
        bool endOfInputText = false;

        // Add the indicator string to the start of the resulting text.
        string indicatorString = GenerateIndicatorString();
        result.AddRange(indicatorString);

        // Embed the number of rotors into the text.
        result.Add(IntToChar(configuration.NumberOfRotors));

        // Embed the reflector index value into the text.
        result.Add(IntToChar(configuration.ReflectorIndex));

        int rotorIndex = 0;
        int textIndex = 0;

        // Embed the rotor index values into the text.
        while (rotorIndex < configuration.NumberOfRotors)
        {
            if (!endOfInputText && textIndex < inputText.Length)
            {
                result.Add(inputText[textIndex++]);
            }
            else
            {
                if (textIndex == inputText.Length)
                {
                    result.Add(MaxChar);
                    endOfInputText = true;
                }
            }

            rotorIndex++;

            switch (rotorIndex)
            {
                case 1:
                    result.Add(IntToChar(configuration.RotorIndex1));
                    break;

                case 2:
                    result.Add(IntToChar(configuration.RotorIndex2));
                    break;

                case 3:
                    result.Add(IntToChar(configuration.RotorIndex3));
                    break;

                case 4:
                    result.Add(IntToChar(configuration.RotorIndex4));
                    break;

                case 5:
                    result.Add(IntToChar(configuration.RotorIndex5));
                    break;

                case 6:
                    result.Add(IntToChar(configuration.RotorIndex6));
                    break;

                case 7:
                    result.Add(IntToChar(configuration.RotorIndex7));
                    break;

                default:
                    result.Add(IntToChar(configuration.RotorIndex8));
                    break;
            }
        }

        // Embed the seed value length into the text.
        result.Add(IntToChar(configuration.SeedValue.Length));

        int seedIndex = 0;

        // Embed the seed value into the text.
        while (!endOfInputText || seedIndex < configuration.SeedValue.Length)
        {
            if (textIndex < inputText.Length)
            {
                result.Add(inputText[textIndex++]);
            }
            else
            {
                if (!endOfInputText)
                {
                    if (seedIndex < configuration.SeedValue.Length)
                    {
                        result.Add(MaxChar);
                    }

                    endOfInputText = true;
                }
            }

            if (seedIndex < configuration.SeedValue.Length)
            {
                result.Add(configuration.SeedValue[seedIndex++]);
            }
        }

        // Return the text with the embedded configuration.
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

        if (string.IsNullOrWhiteSpace(inputText) || inputText.Length < IndicatorSize * 2 || !HasIndicatorString(inputText))
        {
            return inputText;
        }

        List<char> result = [];
        int textIndex = IndicatorSize * 2;
        bool endOfInputText = false;

        // Extract the number of rotors from the input text.
        int numberOfRotors = inputText[textIndex++] - MinChar;

        // Extract the reflector index value from the text.
        int reflectorIndex = inputText[textIndex++] - MinChar;

        int rotorIndex1 = 0;
        int rotorIndex2 = 0;
        int rotorIndex3 = 0;
        int rotorIndex4 = 0;
        int rotorIndex5 = 0;
        int rotorIndex6 = 0;
        int rotorIndex7 = 0;
        int rotorIndex8 = 0;
        int rotorIndex = 0;

        // Extract the rotor index values from the text.
        while (rotorIndex < numberOfRotors)
        {
            if (!endOfInputText)
            {
                char nextChar = inputText[textIndex++];

                if (nextChar is MaxChar)
                {
                    endOfInputText = true;
                }
                else
                {
                    result.Add(nextChar);
                }
            }

            int rotorIndexValue = inputText[textIndex++] - MinChar;
            rotorIndex++;

            switch (rotorIndex)
            {
                case 1:
                    rotorIndex1 = rotorIndexValue;
                    break;

                case 2:
                    rotorIndex2 = rotorIndexValue;
                    break;

                case 3:
                    rotorIndex3 = rotorIndexValue;
                    break;

                case 4:
                    rotorIndex4 = rotorIndexValue;
                    break;

                case 5:
                    rotorIndex5 = rotorIndexValue;
                    break;

                case 6:
                    rotorIndex6 = rotorIndexValue;
                    break;

                case 7:
                    rotorIndex7 = rotorIndexValue;
                    break;

                default:
                    rotorIndex8 = rotorIndexValue;
                    break;
            }
        }

        // Extract the seed value length from the text.
        int seedLength = inputText[textIndex++] - MinChar;

        char[] seedChars = new char[seedLength];
        int seedIndex = 0;

        // Extract the seed value from the text.
        while (textIndex < inputText.Length)
        {
            if (!endOfInputText)
            {
                char nextChar = inputText[textIndex++];

                if (nextChar is MaxChar)
                {
                    endOfInputText = true;
                }
                else
                {
                    result.Add(nextChar);
                }
            }

            if (seedIndex < seedLength)
            {
                seedChars[seedIndex++] = inputText[textIndex++];
            }
        }

        // Save the Enigma configuration.
        configuration.NumberOfRotors = numberOfRotors;
        configuration.ReflectorIndex = reflectorIndex;
        configuration.RotorIndex1 = rotorIndex1;
        configuration.RotorIndex2 = rotorIndex2;
        configuration.RotorIndex3 = rotorIndex3;
        configuration.RotorIndex4 = rotorIndex4;
        configuration.RotorIndex5 = rotorIndex5;
        configuration.RotorIndex6 = rotorIndex6;
        configuration.RotorIndex7 = rotorIndex7;
        configuration.RotorIndex8 = rotorIndex8;
        configuration.SeedValue = new(seedChars);
        configuration.UseEmbeddedConfiguration = false;

        // Return the input text with the embedded configuration data removed.
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
    /// Convert an integer value to its corresponding character value.
    /// </summary>
    /// <param name="value">
    /// The integer value to be converted to a character value.
    /// </param>
    /// <returns>
    /// The character value corresponding to the given integer value.
    /// </returns>
    private static char IntToChar(int value) => (char)(value + MinChar);

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