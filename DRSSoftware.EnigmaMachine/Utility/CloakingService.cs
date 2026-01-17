namespace DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// A static class used for applying and removing cloaking transformations on text.
/// </summary>
/// <param name="numberGenerator">
/// A reference to an object used for generating cryptographically secure random integer values.
/// </param>
internal sealed class CloakingService(ISecureNumberGenerator numberGenerator) : ICloakingService
{
    /// <summary>
    /// Represents the carriage return character ('\r', or U+000D).
    /// </summary>
    private const char CarriageReturn = '\r';

    /// <summary>
    /// Represents the number of character pairs that make up the cloaking indicator string and the
    /// embedded configuration indicator string.
    /// </summary>
    private const int IndicatorSize = 3;

    /// <summary>
    /// Represents the line feed character ('\n', or U+000A).
    /// </summary>
    private const char LineFeed = '\n';

    /// <summary>
    /// Represents the maximum valid character value (U+007F) supported by the Enigma machine.
    /// </summary>
    private const char MaxChar = '\u007f';

    /// <summary>
    /// Represents a character value that is one higher than the maximum cloaking indicator value.
    /// </summary>
    private const char MaxIndicatorChar = '\u007d';

    /// <summary>
    /// The maximum integer value corresponding to the maximum character value supported by the
    /// Enigma machine.
    /// </summary>
    private const int MaxValue = MaxChar - MinChar;

    /// <summary>
    /// Represents the minimum valid character value (the space character, or U+0020) supported by
    /// the Enigma machine.
    /// </summary>
    private const char MinChar = '\u0020';

    /// <summary>
    /// Holds a reference to an object used for generating cryptographically secure random integer
    /// values.
    /// </summary>
    private readonly ISecureNumberGenerator _numberGenerator = numberGenerator;

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
        int[] inValues = StringToIntArray(inputText);
        int[] cloakValues = StringToIntArray(cloakText);
        int[] outValues = new int[inValues.Length];
        int cloakIndex = 0;

        string indicatorString = GenerateIndicatorString();

        for (int i = 0; i < inValues.Length; i++)
        {
            outValues[i] = inValues[i] - cloakValues[cloakIndex++];

            if (cloakIndex == cloakValues.Length)
            {
                cloakIndex = 0;
            }
        }

        return indicatorString + IntArrayToString(outValues);
    }

    /// <summary>
    /// Determine whether or not the supplied <paramref name="inputText" /> starts with a cloaking
    /// indicator string.
    /// </summary>
    /// <param name="inputText">
    /// The input text to be checked for the indicator string.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the <paramref name="inputText" /> starts with and indicator
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

            if (firstCharValue + secondCharValue != MinChar + MaxIndicatorChar)
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
        if (!HasIndicatorString(inputText))
        {
            return inputText;
        }

        int[] inValues = StringToIntArray(inputText[(IndicatorSize * 2)..]);
        int[] cloakValues = StringToIntArray(cloakText);
        int[] outValues = new int[inValues.Length];
        int cloakIndex = 0;

        for (int i = 0; i < inValues.Length; i++)
        {
            outValues[i] = inValues[i] + cloakValues[cloakIndex++];

            if (cloakIndex == cloakValues.Length)
            {
                cloakIndex = 0;
            }
        }

        return IntArrayToString(outValues);
    }

    /// <summary>
    /// Converts an array of integers to the equivalent string representation.
    /// </summary>
    /// <param name="values">
    /// An array of integers representing character values.
    /// </param>
    /// <returns>
    /// The string value corresponding to the integers contained in the <paramref name="values" />
    /// array.
    /// </returns>
    private static string IntArrayToString(int[] values)
    {
        List<char> outChars = [];

        for (int i = 0; i < values.Length; i++)
        {
            int value = values[i];

            while (value < 0)
            {
                value += MaxValue + 1;
            }

            while (value > MaxValue)
            {
                value -= MaxValue + 1;
            }

            if (value is MaxValue)
            {
                outChars.AddRange(Environment.NewLine);
            }
            else
            {
                outChars.Add((char)(value + MinChar));
            }
        }

        return new([.. outChars]);
    }

    /// <summary>
    /// Converts the specified string to an array of integers.
    /// </summary>
    /// <remarks>
    /// Carriage return characters are skipped. Line feed characters are mapped to the maximum
    /// value. All other characters are mapped based on their offset from the minimum character
    /// value, with out-of-range characters replaced by the minimum character.
    /// </remarks>
    /// <param name="text">
    /// The input string to convert.
    /// </param>
    /// <returns>
    /// An array of integers representing the mapped values of the input string's characters.
    /// </returns>
    private static int[] StringToIntArray(string text)
    {
        List<int> result = [];
        char[] chars = text.ToCharArray();

        for (int i = 0; i < text.Length; i++)
        {
            if (chars[i] is CarriageReturn)
            {
                continue;
            }

            if (chars[i] is LineFeed)
            {
                result.Add(MaxValue);
            }
            else if (chars[i] is < MinChar or >= MaxChar)
            {
                result.Add(0);
            }
            else
            {
                result.Add(chars[i] - MinChar);
            }
        }

        return [.. result];
    }

    /// <summary>
    /// Generates a random indicator string used for cloaking or embedding configuration data.
    /// </summary>
    /// <remarks>
    /// Each character in the first half of the indicator string, when added to the corresponding
    /// character in the second half of the string, will equal (MinChar + MaxChar).
    /// </remarks>
    /// <returns>
    /// A new randomly generated indicator string.
    /// </returns>
    private string GenerateIndicatorString()
    {
        char[] indicatorChars = new char[IndicatorSize * 2];

        for (int i = 0; i < IndicatorSize; i++)
        {
            int indicatorValue = _numberGenerator.GetNext(MinChar, MaxIndicatorChar);
            indicatorChars[i] = (char)indicatorValue;
            indicatorChars[i + IndicatorSize] = (char)(MaxIndicatorChar - indicatorValue + MinChar);
        }

        return new string(indicatorChars);
    }
}