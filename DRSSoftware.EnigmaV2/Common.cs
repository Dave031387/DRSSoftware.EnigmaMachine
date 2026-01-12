namespace DRSSoftware.EnigmaV2;

/// <summary>
/// A static class that provides global constants and utility methods used by the Enigma V2 class
/// library.
/// </summary>
/// <remarks>
/// This class contains constants that define the character range, table size, and other
/// configuration values for the <see cref="EnigmaMachine" />, as well as utility methods for
/// converting between character and integer values.
/// </remarks>
internal static class Common
{
    /// <summary>
    /// Represents the carriage return character ('\r', or U+000D).
    /// </summary>
    internal const char CarriageReturn = '\r';

    /// <summary>
    /// Represents the default <see cref="Rotor" /> count used in configuring the
    /// <see cref="EnigmaMachine" />.
    /// </summary>
    /// <remarks>
    /// This constant is used by the default constructor of the <see cref="EnigmaMachine" /> class
    /// to specify the number of <see cref="Rotor" /> objects to configure.
    /// </remarks>
    internal const int DefaultNumberOfRotors = 4;

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
    /// Specifies the maximum number of rotors supported by the <see cref="EnigmaMachine" />
    /// constructor that takes a single <see langword="int" /> parameter.
    /// </summary>
    internal const int MaxRotors = 8;

    /// <summary>
    /// Represents the minimum valid character value (the space character, or U+0020) supported by
    /// the <see cref="EnigmaMachine" />.
    /// </summary>
    internal const char MinChar = '\u0020';

    /// <summary>
    /// Specifies the minimum number of rotors required for the operation of the
    /// <see cref="EnigmaMachine" />.
    /// </summary>
    internal const int MinRotors = 1;

    /// <summary>
    /// Represents the minimum length of the seed value that is used for initializing the
    /// <see cref="Rotor" /> and <see cref="Reflector" /> objects.
    /// </summary>
    internal const int MinSeedLength = 10;

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
    /// Converts a character <paramref name="c" /> to its corresponding integer value based on the
    /// minimum character value supported by the <see cref="EnigmaMachine" />.
    /// </summary>
    /// <remarks>
    /// The conversion is performed by subtracting the <see cref="MinChar" /> value from the
    /// character's Unicode value. <br /> The returned value is guaranteed to be between 0 and
    /// <see cref="MaxIndex" /> inclusive. <br /> If the character is outside the valid range then
    /// it will be converted to 0.
    /// </remarks>
    /// <param name="c">
    /// The character that is to be converted to an integer value.
    /// </param>
    /// <returns>
    /// The integer value obtained by subtracting the minimum character value from the given value
    /// <paramref name="c" />.
    /// </returns>
    internal static int CharToInt(char c) => c is LineFeed ? MaxIndex : c is < MinChar or > MaxChar ? 0 : c - MinChar;

    /// <summary>
    /// Converts the integer value <paramref name="i" /> to its corresponding character
    /// representation.
    /// </summary>
    /// <remarks>
    /// The converted value is computed by adding the <see cref="MinChar" /> value to
    /// <paramref name="i" />. <br /> The integer value will be converted to the space character if
    /// it is less than 0 or greater than <see cref="MaxIndex" />.
    /// </remarks>
    /// <param name="i">
    /// The integer that is to be converted to a character value.
    /// </param>
    /// <returns>
    /// The character value corresponding to the specified integer value <paramref name="i" />.
    /// </returns>
    internal static char IntToChar(int i) => i is < 0 or > MaxIndex ? MinChar : (char)(i + MinChar);
}