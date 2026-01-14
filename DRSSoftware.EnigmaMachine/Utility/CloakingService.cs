namespace DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// A static class used for applying and removing cloaking transformations on text.
/// </summary>
internal static class CloakingService
{
    /// <summary>
    /// Represents the carriage return character ('\r', or U+000D).
    /// </summary>
    private const char CarriageReturn = '\r';

    /// <summary>
    /// Represents the line feed character ('\n', or U+000A).
    /// </summary>
    private const char LineFeed = '\n';

    /// <summary>
    /// Represents the maximum valid character value (U+007F) supported by the Enigma machine.
    /// </summary>
    private const char MaxChar = '\u007f';

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
    /// Applies a cloaking transformation to the specified output text using the provided cloak text
    /// as a key.
    /// </summary>
    /// <param name="inputText">
    /// The text to be transformed by applying the cloak.
    /// </param>
    /// <param name="cloakText">
    /// The cloak text used to transform the output text.
    /// </param>
    /// <returns>
    /// A string representing the result of applying the cloak transformation to the output text.
    /// </returns>
    internal static string ApplyCloak(string inputText, string cloakText)
    {
        int[] inValues = StringToIntArray(inputText);
        int[] cloakValues = StringToIntArray(cloakText);
        int[] outValues = new int[inValues.Length];
        int cloakIndex = 0;

        for (int i = 0; i < inValues.Length; i++)
        {
            outValues[i] = inValues[i] - cloakValues[cloakIndex++];

            if (cloakIndex == cloakValues.Length)
            {
                cloakIndex = 0;
            }
        }

        return IntArrayToString(outValues);
    }

    /// <summary>
    /// Removes the cloaking transformation from the specified input text using the provided cloak
    /// text as a key.
    /// </summary>
    /// <param name="inputText">
    /// The text to be transformed by removing the cloak.
    /// </param>
    /// <param name="cloakText">
    /// The cloak text used to transform the input text.
    /// </param>
    /// <returns>
    /// A string representing the result of removing the cloak transformation from the input text.
    /// </returns>
    internal static string RemoveCloak(string inputText, string cloakText)
    {
        int[] inValues = StringToIntArray(inputText);
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
}