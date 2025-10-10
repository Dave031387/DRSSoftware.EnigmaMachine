namespace DRSSoftware.EnigmaV2;

internal class EnigmaReflector : IEnigmaReflector
{
    internal bool _isInitialized;
    internal IEnigmaWheel? _outgoingWheel;
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

    public int TransformIn(int c, bool _)
    {
        if (_isInitialized)
        {
            int transformedValue = _reflectorTable[c];
            return _outgoingWheel is not null
                ? _outgoingWheel.TransformOut(transformedValue)
                : throw new InvalidOperationException("An outgoing wheel hasn't been connected to the Enigma reflector.");
        }

        throw new InvalidOperationException("The Enigma reflector must be initialized before calling the TransformIn method.");
    }
}