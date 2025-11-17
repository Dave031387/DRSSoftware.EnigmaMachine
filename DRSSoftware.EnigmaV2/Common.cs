namespace DRSSoftware.EnigmaV2;

/// <summary>
/// A static class that provides global constants and utility methods used by the Enigma V2 class
/// library.
/// </summary>
/// <remarks>
/// This class contains constants that define the character range, table size, and other
/// configuration values for the <see cref="EnigmaMachine" />, as well as utility methods for
/// character and index manipulation.
/// </remarks>
internal static class Common
{
    /// <summary>
    /// Represents the carriage return character ('\r', or U+000D).
    /// </summary>
    internal const char CarriageReturn = '\r';

    /// <summary>
    /// Represents the line feed character ('\n', or U+000A).
    /// </summary>
    internal const char LineFeed = '\n';

    /// <summary>
    /// Represents the maximum valid character value (U+007F) supported by the
    /// <see cref="EnigmaMachine" />.
    /// </summary>
    /// <remarks>
    /// This character is normally the DEL (delete) character. But it is used by the
    /// <see cref="EnigmaMachine" /> to represent a new line character. <br /> New lines are
    /// converted to <see cref="MaxChar" /> when they are read in by the
    /// <see cref="EnigmaMachine" />, and <see cref="MaxChar" /> is converted back to new lines when
    /// the transformed text is returned back to the user.
    /// </remarks>
    internal const char MaxChar = '\u007f';

    /// <summary>
    /// Represents the maximum index value that the <see cref="Reflector" /> and each
    /// <see cref="Rotor" /> of the <see cref="EnigmaMachine" /> can be set to.
    /// </summary>
    /// <remarks>
    /// This constant is derived from the range of characters defined by <see cref="MinChar" /> and
    /// <see cref="MaxChar" />. It is used to determine the upper bound for indexing operations
    /// within this range.
    /// </remarks>
    internal const int MaxIndex = MaxChar - MinChar;

    /// <summary>
    /// Represents the minimum valid character value (the space character, or U+0020) supported by
    /// the <see cref="EnigmaMachine" />.
    /// </summary>
    internal const char MinChar = '\u0020';

    /// <summary>
    /// Represents the minimum length of the seed value that is used for initializing the
    /// <see cref="Rotor" /> and <see cref="Reflector" /> objects.
    /// </summary>
    internal const int MinSeedLength = 10;

    /// <summary>
    /// Represents the <see cref="Rotor" /> count (number of cipher wheels) used in the
    /// <see cref="EnigmaMachine" />.
    /// </summary>
    internal const int NumberOfRotors = 4;

    /// <summary>
    /// Represents the number of positions on the <see cref="Reflector" /> and each
    /// <see cref="Rotor" /> of the <see cref="EnigmaMachine" />.
    /// </summary>
    /// <remarks>
    /// This constant is calculated by adding 1 to the <see cref="MaxIndex" /> constant and is equal
    /// to the total number of characters supported by the <see cref="EnigmaMachine" />.
    /// </remarks>
    internal const int TableSize = MaxIndex + 1;

    /// <summary>
    /// Converts a character to its corresponding integer value based on the minimum character value
    /// supported by the <see cref="EnigmaMachine" />.
    /// </summary>
    /// <remarks>
    /// The conversion is performed by subtracting the <see cref="MinChar" /> value from the
    /// character's Unicode value. Ensure that the input character is within the valid range for the
    /// intended conversion.
    /// </remarks>
    /// <param name="c">
    /// The character to convert.
    /// </param>
    /// <returns>
    /// The integer value of the character after applying the offset.
    /// </returns>
    internal static int CharToInt(char c) => c - MinChar;

    /// <summary>
    /// Converts an integer to its corresponding character representation.
    /// </summary>
    /// <remarks>
    /// The converted value is computed by adding the <see cref="MinChar" /> value to
    /// <paramref name="i" />.
    /// </remarks>
    /// <param name="i">
    /// The integer to convert. Must be within the valid range for character conversion.
    /// </param>
    /// <returns>
    /// The character corresponding to the specified integer.
    /// </returns>
    internal static char IntToChar(int i) => (char)(i + MinChar);
}