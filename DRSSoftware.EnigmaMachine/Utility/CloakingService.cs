namespace DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// A service used for applying and removing cloaking transformations on text.
/// </summary>
/// <param name="numberGenerator">
/// A reference to an object used for generating cryptographically secure random integer values.
/// </param>
internal sealed class CloakingService(ISecureNumberGenerator numberGenerator) : ICloakingService
{
    /// <summary>
    /// Holds a reference to an object used for generating cryptographically secure random integer
    /// values.
    /// </summary>
    private readonly ISecureNumberGenerator _numberGenerator = numberGenerator;

    private int _cloakIndex;
    private string _cloakText = string.Empty;

    /// <summary>
    /// Cloak the given <paramref name="inputText" /> using the supplied
    /// <paramref name="cloakText" />.
    /// </summary>
    /// <remarks>
    /// A special cloaking indicator string will be prepended to the cloaked text to indicate that
    /// the text has been cloaked.
    /// </remarks>
    /// <param name="inputText">
    /// The text to be transformed by applying the cloak.
    /// </param>
    /// <param name="cloakText">
    /// The string used for performing the cloaking transformation.
    /// </param>
    /// <returns>
    /// The string obtained by applying the cloaking transformation to the input string.
    /// </returns>
    public string ApplyCloak(string inputText, string cloakText)
    {
        List<char> outputChars = [];
        _cloakText = cloakText;
        _cloakIndex = 0;

        outputChars.AddRange(GenerateIndicatorString());

        for (int i = 0; i < inputText.Length; i++)
        {
            char inputChar = inputText[i];

            // Ignore carriage return characters in the input string.
            if (inputChar is CarriageReturn)
            {
                continue;
            }

            // Don't cloak or decloak delimiter characters.
            if (inputChar is DelimiterChar)
            {
                outputChars.Add(DelimiterChar);
                continue;
            }

            char cloakChar = GetNextCloakChar();

            // Apply the cloaking transform to the input character using the given cloaking
            // character.
            char cloakedChar = ApplyCloak(inputChar, cloakChar);

            // Replace MaxChar with new line characters (CR/LF on Windows systems).
            if (cloakedChar is MaxChar)
            {
                outputChars.AddRange(Environment.NewLine);
            }
            else
            {
                outputChars.Add(cloakedChar);
            }
        }

        return new([.. outputChars]);
    }

    /// <summary>
    /// Determine whether or not the supplied <paramref name="inputText" /> starts with a cloaking
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

            if (firstCharValue + secondCharValue != CloakedIndicatorValue)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Remove the cloak from the given <paramref name="inputText" /> using the supplied
    /// <paramref name="cloakText" />.
    /// </summary>
    /// <remarks>
    /// The method assumes that the input text has been previously cloaked using the same
    /// <paramref name="cloakText" />. The cloaking indicator string is removed from the
    /// <paramref name="inputText" /> before removing the cloak.
    /// </remarks>
    /// <param name="inputText">
    /// The text to be transformed by removing the cloak.
    /// </param>
    /// <param name="cloakText">
    /// The string used for undoing the cloaking transformation.
    /// </param>
    /// <returns>
    /// The string obtained by removing the cloaking transformation from the input string. The input
    /// string will be returned unchanged if it does not start with a cloaking indicator string.
    /// </returns>
    public string RemoveCloak(string inputText, string cloakText)
    {
        // Return the input text unchanged if it doesn't begin with the expected cloaking indicator
        // characters.
        if (!HasIndicatorString(inputText))
        {
            return inputText;
        }

        List<char> outputChars = [];
        _cloakText = cloakText;
        _cloakIndex = 0;

        for (int i = IndicatorSize; i < inputText.Length; i++)
        {
            char inputChar = inputText[i];

            // Ignore carriage return characters in the input string.
            if (inputChar is CarriageReturn)
            {
                continue;
            }

            // Don't cloak or decloak delimiter characters.
            if (inputChar is DelimiterChar)
            {
                outputChars.Add(DelimiterChar);
                continue;
            }

            char cloakChar = GetNextCloakChar();

            // Remove the cloaking transform from the input character using the given cloaking
            // character.
            char decloakedChar = RemoveCloak(inputChar, cloakChar);

            // Replace MaxChar with new line characters (CR/LF on Windows systems).
            if (decloakedChar is MaxChar)
            {
                outputChars.AddRange(Environment.NewLine);
            }
            else
            {
                outputChars.Add(decloakedChar);
            }
        }

        return new([.. outputChars]);
    }

    /// <summary>
    /// Adjust the input character by replacing line feed characters with MaxChar.
    /// </summary>
    /// <remarks>
    /// This method assumes that any input characters that aren't line feed characters will be in
    /// the range between MinChar and MaxChar.
    /// </remarks>
    /// <param name="inputChar">
    /// The input character to be adjusted.
    /// </param>
    /// <returns>
    /// An input character within the valid range of characters between MinChar and MaxChar.
    /// </returns>
    private static int AdjustInputChar(char inputChar)
    {
        return inputChar is LineFeed
            ? MaxValue
            : inputChar - MinChar;
    }

    /// <summary>
    /// Apply the cloaking transform to the given input character using the specified cloak
    /// character.
    /// </summary>
    /// <param name="inputChar">
    /// The input character that is to be cloaked.
    /// </param>
    /// <param name="cloakChar">
    /// The cloaking character that is used by the cloaking transform to cloak the input character.
    /// </param>
    /// <returns>
    /// The transformed input character after the cloak has been applied.
    /// </returns>
    private static char ApplyCloak(char inputChar, char cloakChar)
    {
        int inValue = AdjustInputChar(inputChar);
        int cloakValue = cloakChar - MinChar;

        int cloakedValue = inValue - cloakValue;

        while (cloakedValue < 0)
        {
            cloakedValue += MaxValue + 1;
        }

        return (char)(cloakedValue + MinChar);
    }

    /// <summary>
    /// Remove the cloaking transform from the given input character using the specified cloak
    /// character.
    /// </summary>
    /// <param name="inputChar">
    /// The input character that is to be decloaked.
    /// </param>
    /// <param name="cloakChar">
    /// The cloaking character that is used by the cloaking transform to decloak the input
    /// character.
    /// </param>
    /// <returns>
    /// The transformed input character after the cloak has been removed.
    /// </returns>
    private static char RemoveCloak(char inputChar, char cloakChar)
    {
        int inValue = AdjustInputChar(inputChar);
        int cloakValue = cloakChar - MinChar;

        int cloakedValue = inValue + cloakValue;

        while (cloakedValue > MaxValue)
        {
            cloakedValue -= MaxValue + 1;
        }

        return (char)(cloakedValue + MinChar);
    }

    /// <summary>
    /// Generates a random indicator string used for cloaking.
    /// </summary>
    /// <returns>
    /// A new randomly generated indicator string.
    /// </returns>
    private string GenerateIndicatorString()
    {
        char[] indicatorChars = new char[IndicatorSize];

        for (int i = 0; i < IndicatorPairs; i++)
        {
            int indicatorValue = _numberGenerator.GetNext(MinChar, CloakingIndicatorChar);
            indicatorChars[i] = (char)indicatorValue;
            indicatorChars[i + IndicatorPairs] = (char)(CloakedIndicatorValue - indicatorValue);
        }

        return new string(indicatorChars);
    }

    /// <summary>
    /// Get the next cloaking character. Wrap around back to the beginning of the cloak string if we
    /// reach the end.
    /// </summary>
    /// <remarks>
    /// Carriage return characters are skipped if found in the cloaking string.
    /// </remarks>
    /// <returns>
    /// The next character from the cloaking string.
    /// </returns>
    private char GetNextCloakChar()
    {
        if (_cloakIndex >= _cloakText.Length)
        {
            _cloakIndex = 0;
        }

        int saveIndex = _cloakIndex;

        while (_cloakText[_cloakIndex] is CarriageReturn)
        {
            if (++_cloakIndex >= _cloakText.Length)
            {
                _cloakIndex = 0;
            }

            // Ensure that we can't enter into an endless loop.
            if (saveIndex == _cloakIndex)
            {
                break;
            }
        }

        char cloakChar = _cloakText[_cloakIndex++];

        // Ensure the cloak character is within the valid range of values between MinChar and
        // MaxChar before returning it to the caller of this method.
        return cloakChar is LineFeed
            ? MaxChar
            : cloakChar is < MinChar or >= MaxChar
                ? (char)(MinChar + (cloakChar % (MaxValue + 1)))
                : cloakChar;
    }
}