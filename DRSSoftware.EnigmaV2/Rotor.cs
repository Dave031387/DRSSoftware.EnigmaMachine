namespace DRSSoftware.EnigmaV2;

internal class Rotor : IRotor
{
    internal readonly int[] _incomingTable = new int[TableSize];
    internal readonly int[] _outgoingTable = new int[TableSize];
    internal bool _isInitialized;
    internal IRotor? _rotorIn;
    internal int _rotorIndex;
    internal ITransformer? _transformerOut;
    internal bool _transformIsInProgress;

    public void ConnectIncoming(IRotor rotor)
    {
        ArgumentNullException.ThrowIfNull(rotor, nameof(rotor));

        if (_rotorIn is not null)
        {
            throw new InvalidOperationException("Invalid attempt to add an outgoing rotor when one is already defined.");
        }

        _rotorIn = rotor;
    }

    public void ConnectOutgoing(ITransformer transformer)
    {
        ArgumentNullException.ThrowIfNull(transformer, nameof(transformer));

        if (_transformerOut is not null)
        {
            throw new InvalidOperationException("Invalid attempt to add an incoming transformer when one is already defined.");
        }

        _transformerOut = transformer;
    }

    public void Initialize(string seed)
    {
        if (seed.Length < 10)
        {
            throw new ArgumentException($"The seed string passed into the Initialize method must be at least 10 characters long, but it was {seed.Length}.", nameof(seed));
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

        _rotorIndex = 0;
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

            int delta = indexValue - _rotorIndex;
            int rotateAmount = delta < 0 ? TableSize + delta : delta;
            Rotate(rotateAmount);
        }
        else
        {
            throw new InvalidOperationException("The rotor must be initialized before the index can be set.");
        }
    }

    public int TransformIn(int c, bool shouldRotate)
    {
        if (_isInitialized)
        {
            _transformIsInProgress = true;
            bool shouldRotateNext = false;

            if (shouldRotate)
            {
                Rotate();
                shouldRotateNext = _rotorIndex == 0;
            }

            int transformedValue = _incomingTable[c];

            return _transformerOut is not null
                ? _transformerOut.TransformIn(transformedValue, shouldRotateNext)
                : throw new InvalidOperationException("An outgoing transformer hasn't been connected to this rotor.");
        }

        throw new InvalidOperationException("The rotor must be initialized before the TransformIn method is called.");
    }

    public int TransformOut(int c)
    {
        if (_transformIsInProgress)
        {
            _transformIsInProgress = false;
            int index = GetIndex(c, TableSize, -_rotorIndex);
            int transformedValue = _outgoingTable[index];

            return _rotorIn is not null ? _rotorIn.TransformOut(transformedValue) : transformedValue;
        }

        throw new InvalidOperationException("The TransformOut method must not be called on the rotor before calling the TransformIn method.");
    }

    private void Rotate(int amount = 1)
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

        _rotorIndex = GetIndex(_rotorIndex, TableSize, amount);
    }
}