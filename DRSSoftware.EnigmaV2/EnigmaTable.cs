namespace DRSSoftware.EnigmaV2;

internal static class EnigmaTable
{
    internal const char CarriageReturn = '\r';
    internal const char LineFeed = '\n';
    internal const char MaxChar = '\u007f';
    internal const int MaxIndex = MaxChar - MinChar;
    internal const char MinChar = '\u0020';
    internal const int TableSize = MaxIndex + 1;

    internal static int CharToInt(char c) => c - MinChar;

    internal static int DisplaceIndex(int index, char seedChar)
    {
        if (index is < 0 or > MaxIndex)
        {
            throw new ArgumentOutOfRangeException(nameof(index), $"The index passed into the DisplaceIndex method must be greater than or equal to zero and less than {TableSize}, but it was {index}.");
        }

        int offset = seedChar is < MinChar or > MaxChar ? 1 : seedChar == MinChar ? 1 : CharToInt(seedChar);
        return GetIndex(index, TableSize, offset);
    }

    internal static int FindAvailableSlot(int startIndex, bool[] slotIsTaken)
    {
        int index = startIndex;

        while (slotIsTaken[index])
        {
            index = GetIndex(index, TableSize, 1);

            if (index == startIndex)
            {
                throw new InvalidOperationException("The FindAvailableSlot method is unable to find an available slot.");
            }
        }

        slotIsTaken[index] = true;
        return index;
    }

    internal static int GetIndex(int indexBase, int arraySize, int offset)
    {
        int index = indexBase + offset;

        return index >= arraySize ? index - arraySize : index < 0 ? index + arraySize : index;
    }

    internal static char IntToChar(int i) => (char)(i + MinChar);
}