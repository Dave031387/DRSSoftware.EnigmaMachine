namespace DRSSoftware.EnigmaV2;

internal class EnigmaWheel : IEnigmaWheel
{
    internal readonly int[] _incomingTable = new int[TableSize];
    internal readonly int[] _outgoingTable = new int[TableSize];
    internal IEnigmaWheel? _incomingWheel;
    internal bool _isInitialized;
    internal ITransformerBase? _outgoingWheel;
    internal bool _transformIsInProgress;
    internal int _wheelIndex;

    public void ConnectIncomingWheel(IEnigmaWheel enigmaWheel)
    {
        ArgumentNullException.ThrowIfNull(enigmaWheel, nameof(enigmaWheel));

        if (_incomingWheel is not null)
        {
            throw new InvalidOperationException("Invalid attempt to add an outgoing wheel when one is already defined.");
        }

        _incomingWheel = enigmaWheel;
    }

    public void ConnectOutgoingWheel(ITransformerBase enigmaWheel)
    {
        ArgumentNullException.ThrowIfNull(enigmaWheel, nameof(enigmaWheel));

        if (_outgoingWheel is not null)
        {
            throw new InvalidOperationException("Invalid attempt to add an incoming wheel when one is already defined.");
        }

        _outgoingWheel = enigmaWheel;
    }

    public void Initialize(string seed)
    {
        if (seed.Length < 10)
        {
            throw new ArgumentException("The seed string passed into the Initialize method must be at least 10 characters long.", nameof(seed));
        }

        bool[] slotIsTaken = new bool[TableSize];
        char[] displacements = seed.ToCharArray();

        int seedIndex = 0;
        int index = 0;

        for (int i = 0; i < TableSize; i++)
        {
            index = DisplaceIndex(index, displacements[seedIndex]);
            index = FindAvailableSlot(index, slotIsTaken);
            _incomingTable[i] = index;
            _outgoingTable[index] = i;
            seedIndex = GetIndex(seedIndex, displacements.Length, 1);
        }

        _wheelIndex = 0;
        _isInitialized = true;
    }

    public void SetWheelIndex(int indexValue)
    {
        // TODO: check for indexValue in correct range
        if (_isInitialized)
        {
            if (indexValue is < 0 or > MaxIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(indexValue), $"The value passed into the SetWheelIndex method must be greater than or equal to zero and less than {TableSize}, but it was {indexValue}.");
            }

            int rotateAmount = indexValue >= _wheelIndex ? indexValue - _wheelIndex : TableSize - _wheelIndex + indexValue;
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
            _transformIsInProgress = true;
            bool rotateNextWheel = false;

            if (shouldRotateWheel)
            {
                RotateWheel();
                rotateNextWheel = _wheelIndex == 0;
            }

            int transformedValue = _incomingTable[c];

            return _outgoingWheel is not null
                ? _outgoingWheel.TransformIn(transformedValue, rotateNextWheel)
                : throw new InvalidOperationException("An outgoing wheel hasn't been connected to this Enigma wheel.");
        }

        throw new InvalidOperationException("The Enigma wheel must be initialized before the TransformIn method is called.");
    }

    public int TransformOut(int c)
    {
        if (_transformIsInProgress)
        {
            _transformIsInProgress = false;
            int index = GetIndex(c, TableSize, -_wheelIndex);
            int transformedValue = _outgoingTable[index];

            return _incomingWheel is not null ? _incomingWheel.TransformOut(transformedValue) : transformedValue;
        }

        throw new InvalidOperationException("The TransformOut method must not be called on the Enigma wheel before calling the TransformIn method.");
    }

    private void RotateWheel(int amount = 1)
    {
        if (amount == 0)
        {
            return;
        }

        int delta = TableSize - amount;

        for (int i = 0; i < TableSize; i++)
        {
            _incomingTable[i] = _incomingTable[i] >= delta ? _incomingTable[i] - delta : _incomingTable[i] + amount;
        }

        _wheelIndex = GetIndex(_wheelIndex, TableSize, amount);
    }
}