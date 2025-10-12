namespace DRSSoftware.EnigmaV2;

internal class Reflector : IReflector
{
    internal bool _isInitialized;
    internal int _reflectorIndex;
    internal IRotor? _rotorOut;
    private readonly int[] _reflectorTable = new int[TableSize];

    public void ConnectOutgoing(IRotor rotor) => _rotorOut = rotor;

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

    public void SetIndex(int indexValue)
    {
        if (_isInitialized)
        {
            if (indexValue is < 0 or > MaxIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(indexValue), $"The value passed into the SetIndex method must be greater than or equal to zero and less than {TableSize}, but it was {indexValue}.");
            }

            int delta = indexValue - _reflectorIndex;
            int rotateAmount = delta < 0 ? TableSize + delta : delta;
            Rotate(rotateAmount);
        }
        else
        {
            throw new InvalidOperationException("The reflector must be initialized before the index can be set.");
        }
    }

    public int TransformIn(int c, bool shouldRotate)
    {
        if (_isInitialized)
        {
            if (shouldRotate)
            {
                Rotate();
            }

            int transformedValue = _reflectorTable[c];
            return _rotorOut is not null
                ? _rotorOut.TransformOut(transformedValue)
                : throw new InvalidOperationException("The outgoing rotor hasn't been connected to the reflector.");
        }

        throw new InvalidOperationException("The reflector must be initialized before calling the TransformIn method.");
    }

    private void Rotate(int amount = 1)
    {
        if (amount == 0)
        {
            return;
        }

        int[] reflectorTable = new int[TableSize];
        _reflectorTable.CopyTo(reflectorTable, 0);

        for (int i = 0; i < TableSize; i++)
        {
            int deltaI = i - amount;
            int j = deltaI < 0 ? deltaI + TableSize : deltaI;
            int deltaJ = reflectorTable[j] + i;
            _reflectorTable[i] = deltaJ < TableSize ? deltaJ : deltaJ - TableSize;
        }

        _reflectorIndex = GetIndex(_reflectorIndex, TableSize, amount);
    }
}