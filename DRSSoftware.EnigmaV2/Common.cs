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
    /// Represents the total number of transformers ( <see cref="Reflector" /> plus
    /// <see cref="Rotor" /> count) used in the <see cref="EnigmaMachine" />.
    /// </summary>
    /// <remarks>
    /// This constant is obtained by adding 1 to <see cref="NumberOfRotors" /> since there is always
    /// only 1 <see cref="Reflector" />.
    /// </remarks>
    internal const int NumberOfTransformers = NumberOfRotors + 1;

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
    /// Calculates a new index value by applying an offset derived from the specified seed
    /// character.
    /// </summary>
    /// <param name="index">
    /// The original index to be displaced. Must be greater than or equal to 0 and less than
    /// <see cref="MaxIndex" />.
    /// </param>
    /// <param name="seedChar">
    /// The character used to determine the displacement offset. If the character is outside the
    /// valid range, a default offset of 1 is used.
    /// </param>
    /// <returns>
    /// The displaced index, adjusted based on the specified seed character and constrained by
    /// <see cref="TableSize" />.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="index" /> is less than 0 or greater than the maximum allowable
    /// index.
    /// </exception>
    internal static int DisplaceIndex(int index, char seedChar)
    {
        if (index is < 0 or > MaxIndex)
        {
            throw new ArgumentOutOfRangeException(nameof(index), $"The index passed into the DisplaceIndex method must be greater than or equal to zero and less than {TableSize}, but it was {index}.");
        }

        int offset = seedChar is < MinChar or > MaxChar ? 1 : seedChar == MinChar ? 1 : CharToInt(seedChar);
        return GetValueWithOffset(index, TableSize, offset);
    }

    /// <summary>
    /// Finds the first unassigned slot on a <see cref="Reflector" /> or <see cref="Rotor" />,
    /// starting from the specified index. <br /> Used during initialization when "wiring" the
    /// <see cref="Reflector" /> or <see cref="Rotor" /> based on a given seed value.
    /// </summary>
    /// <remarks>
    /// This method modifies the <paramref name="slotIsTaken" /> array by marking the returned slot
    /// as taken. The search wraps around in a circular manner, ensuring that all slots are checked
    /// exactly once.
    /// </remarks>
    /// <param name="startIndex">
    /// The index from which to begin the search for an available slot.
    /// </param>
    /// <param name="slotIsTaken">
    /// A boolean array representing the availability of slots. Each element indicates whether the
    /// corresponding slot is taken ( <see langword="true" />) or available (
    /// <see langword="false" />).
    /// </param>
    /// <returns>
    /// The index of the first available slot, starting from <paramref name="startIndex" /> and
    /// wrapping around if necessary.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no available slot can be found.
    /// </exception>
    internal static int FindAvailableSlot(int startIndex, bool[] slotIsTaken)
    {
        int index = startIndex;

        while (slotIsTaken[index])
        {
            index = GetValueWithOffset(index, TableSize, 1);

            if (index == startIndex)
            {
                throw new InvalidOperationException("The FindAvailableSlot method is unable to find an available slot.");
            }
        }

        slotIsTaken[index] = true;
        return index;
    }

    /// <summary>
    /// Gets the transformed value by adding the specified <paramref name="offset" /> to the value
    /// retrieved from the specified <paramref name="table" /> using the given
    /// <paramref name="baseValue" />.
    /// </summary>
    /// <param name="table">
    /// A table of integers used to look up the transformed value for the given
    /// <paramref name="baseValue" />.
    /// </param>
    /// <param name="baseValue">
    /// The integer value that is being transformed.
    /// </param>
    /// <param name="offset">
    /// An integer value used for adjusting the <paramref name="baseValue" /> to arrive at the
    /// desired transformed value.
    /// </param>
    /// <returns>
    /// The integer value corresponding to the transformed <paramref name="baseValue" />.
    /// </returns>
    internal static int GetTransformedValue(int[] table, int baseValue, int offset)
    {
        int index = GetValueWithOffset(baseValue, table.Length, -offset);
        return GetValueWithOffset(table[index], table.Length, offset);
    }

    /// <summary>
    /// Calculates the adjusted value within the bounds of an array using the specified
    /// <paramref name="offset" />, wrapping around if the result exceeds the array size or is
    /// negative.
    /// </summary>
    /// <param name="baseValue">
    /// The base value to start from.
    /// </param>
    /// <param name="arraySize">
    /// The size of the array. Must be greater than zero.
    /// </param>
    /// <param name="offset">
    /// The offset to apply to the base value.
    /// </param>
    /// <returns>
    /// The adjusted value, guaranteed to be within the range [0, <paramref name="arraySize" /> -
    /// 1].
    /// </returns>
    internal static int GetValueWithOffset(int baseValue, int arraySize, int offset)
    {
        int index = baseValue + offset;

        return index >= arraySize ? index - arraySize : index < 0 ? index + arraySize : index;
    }

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