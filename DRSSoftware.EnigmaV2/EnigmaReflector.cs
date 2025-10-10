namespace DRSSoftware.EnigmaV2;

internal class EnigmaReflector : IEnigmaReflector
{
    internal bool _isInitialized;
    internal IEnigmaWheel? _outgoingWheel;
    internal int _wheelIndex;
    private readonly int[] _reflectorTable = new int[TableSize];

    public void ConnectOutgoingWheel(IEnigmaWheel enigmaWheel) => _outgoingWheel = enigmaWheel;

    public void Initialize(string seed)
    {
        bool[] slotIsTaken = new bool[TableSize];
        char[] displacements = seed.ToCharArray();

        int slotsRemaining = TableSize;
        int seedIndex = 0;
        int index1 = 0;
        int index2 = TableSize / 2;

        while (slotsRemaining > 0)
        {
            index1 = DisplaceIndex(index1, displacements[seedIndex]);
            index1 = FindAvailableSlot(index1, slotIsTaken);
            seedIndex = GetIndex(seedIndex, displacements.Length, 1);
            index2 = DisplaceIndex(index2, displacements[seedIndex]);
            index2 = FindAvailableSlot(index2, slotIsTaken);
            seedIndex = GetIndex(seedIndex, displacements.Length, 1);
            _reflectorTable[index1] = index2;
            _reflectorTable[index2] = index1;
            slotsRemaining -= 2;
        }

        _isInitialized = true;
    }

    public void SetWheelIndex(int indexValue)
    {
        if (_isInitialized)
        {
            if (indexValue is < 0 or > MaxIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(indexValue), $"The value passed into the SetWheelIndex method must be greater than or equal to zero and less than {TableSize}, but it was {indexValue}.");
            }

            int delta = indexValue - _wheelIndex;
            int rotateAmount = delta < 0 ? TableSize + delta : delta;
            RotateWheel(rotateAmount);
        }
        else
        {
            throw new InvalidOperationException("The Enigma wheel must be initialized before the wheel index can be set.");
        }
    }

    public int TransformIn(int c, bool shouldRotateWheel)
    {
        if (_isInitialized)
        {
            if (shouldRotateWheel)
            {
                RotateWheel();
            }

            int transformedValue = _reflectorTable[c];
            return _outgoingWheel is not null
                ? _outgoingWheel.TransformOut(transformedValue)
                : throw new InvalidOperationException("An outgoing wheel hasn't been connected to the Enigma reflector.");
        }

        throw new InvalidOperationException("The Enigma reflector must be initialized before calling the TransformIn method.");
    }

    private void RotateWheel(int amount = 1)
    {
        if (amount == 0)
        {
            return;
        }

        int[] temp = new int[TableSize];
        _reflectorTable.CopyTo(temp, 0);

        for (int i = 0; i < TableSize; i++)
        {
            int deltaI = i - amount;
            int j = deltaI < 0 ? deltaI + TableSize : deltaI;
            int deltaJ = temp[j] + i;
            _reflectorTable[i] = deltaJ < TableSize ? deltaJ : deltaJ - TableSize;
        }

        _wheelIndex = GetIndex(_wheelIndex, TableSize, amount);
    }
}