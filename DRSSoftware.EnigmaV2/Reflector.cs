namespace DRSSoftware.EnigmaV2;

/// <summary>
/// This class models the "reflector" component of the Enigma V2 machine.
/// </summary>
/// <remarks>
/// This component comes after the last cipher wheel (or <see cref="Rotor" />). It takes the value
/// coming in from the cipher wheel, transforms that value to a new value, and sends the new value
/// back to the cipher wheel.
/// </remarks>
internal class Reflector : IReflector
{
    /// <summary>
    /// This table is used to transform the incoming value to a different outgoing value.
    /// </summary>
    /// <remarks>
    /// The transformation is always reversible. For example, if the transform of 'A' yields 'X',
    /// then the transform of 'X' will yield 'A'.
    /// </remarks>
    internal readonly int[] _reflectorTable = new int[TableSize];

    /// <summary>
    /// A boolean flag that gets set to <see langword="true" /> when this <see cref="Reflector" />
    /// instance has been initialized and is ready for use.
    /// </summary>
    internal bool _isInitialized;

    /// <summary>
    /// An integer that indicates how far this reflector has been rotated from its starting
    /// position.
    /// </summary>
    internal int _reflectorIndex;

    /// <summary>
    /// A reference to the <see cref="IRotor" /> object representing the last cipher wheel (rotor)
    /// in sequence that exists in the Enigma machine that this <see cref="Reflector" /> object is a
    /// part of.
    /// </summary>
    internal IRotor? _rotorOut;

    /// <summary>
    /// Connect the specified <paramref name="rotor" /> object to the outgoing side of this
    /// <see cref="Reflector" /> object.
    /// </summary>
    /// <param name="rotor">
    /// The <see cref="IRotor" /> object that is to be connected to this <see cref="Reflector" />
    /// object.
    /// </param>
    public void ConnectOutgoing(IRotor rotor) => _rotorOut = rotor;

    /// <summary>
    /// Initialize this <see cref="Reflector" /> object using the specified <paramref name="seed" />
    /// value.
    /// </summary>
    /// <remarks>
    /// The <paramref name="seed" /> value is used for randomizing the connections between the
    /// incoming and outgoing points on this <see cref="Reflector" /> object.
    /// </remarks>
    /// <param name="seed">
    /// A <see langword="string" /> value used for randomizing the connections within this
    /// <see cref="Reflector" /> object.
    /// </param>
    public void Initialize(string seed)
    {
        // This array is used to keep track of which index positions on this reflector object have
        // already been connected to some other index position.
        bool[] slotIsTaken = new bool[TableSize];
        char[] displacements = seed.ToCharArray();
        int slotsRemaining = TableSize;
        int seedIndex = 0;
        int index1 = 0;
        int index2 = TableSize / 2;

        while (slotsRemaining > 0)
        {
            // Determine the first connection point to be wired up.
            index1 = DisplaceIndex(index1, displacements[seedIndex]);
            index1 = FindAvailableSlot(index1, slotIsTaken);
            seedIndex = GetIndex(seedIndex, displacements.Length, 1);

            // Determine the second connection point to be wired up.
            index2 = DisplaceIndex(index2, displacements[seedIndex]);
            index2 = FindAvailableSlot(index2, slotIsTaken);
            seedIndex = GetIndex(seedIndex, displacements.Length, 1);

            // Here we "wire" two positions on the reflector together. If we connect point 3 to
            // point 17, then we also connect point 17 to point 3, for example.
            _reflectorTable[index1] = index2;
            _reflectorTable[index2] = index1;

            // Keep track of how many positions haven't been wired yet. Note that the total number
            // of positions must be an even number for this to work out correctly.
            slotsRemaining -= 2;
        }

        _reflectorIndex = 0;
        _isInitialized = true;
    }

    /// <summary>
    /// Set the reflector index to the desired <paramref name="indexValue" />.
    /// </summary>
    /// <param name="indexValue">
    /// The integer value that the reflector index is to be set to.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="indexValue" /> is less than zero or greater than or equal to
    /// the table size.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called prior to initializing the <see cref="Reflector" /> object.
    /// </exception>
    public void SetIndex(int indexValue)
    {
        if (_isInitialized)
        {
            if (indexValue is < 0 or > MaxIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(indexValue), $"The value passed into the SetIndex method must be greater than or equal to zero and less than {TableSize}, but it was {indexValue}.");
            }

            int rotateAmount = GetIndex(indexValue, TableSize, -_reflectorIndex);
            Rotate(rotateAmount);
        }
        else
        {
            throw new InvalidOperationException("The reflector must be initialized before the index can be set.");
        }
    }

    /// <summary>
    /// Transform the incoming cipher value <paramref name="c" /> and send the transformed value to
    /// the outgoing cipher wheel (or <see cref="Rotor" />).
    /// </summary>
    /// <remarks>
    /// The cipher value is an integer representation of a printable ASCII character which has been
    /// adjusted by subtracting the minimum character value from the cipher character value.
    /// </remarks>
    /// <param name="c">
    /// The cipher value that is to be transformed.
    /// </param>
    /// <param name="shouldRotate">
    /// A flag indicating whether or not the <see cref="Reflector" /> should be rotated one position
    /// before applying the transform.
    /// </param>
    /// <returns>
    /// The final transformed cipher value after it has been processed by all of the cipher wheels
    /// (rotors) and the reflector.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called prior to initializing the <see cref="Reflector" /> object.
    /// </exception>
    public int TransformIn(int c, bool shouldRotate)
    {
        if (_isInitialized)
        {
            if (_rotorOut is null)
            {
                throw new InvalidOperationException("The outgoing rotor hasn't been connected to the reflector.");
            }

            if (shouldRotate)
            {
                Rotate();
            }

            int transformedValue = _reflectorTable[c];
            return _rotorOut.TransformOut(transformedValue);
        }

        throw new InvalidOperationException("The reflector must be initialized before calling the TransformIn method.");
    }

    /// <summary>
    /// Rotate this <see cref="Reflector" /> object by the specified number of positions.
    /// </summary>
    /// <param name="amount">
    /// The number of positions that the <see cref="Reflector" /> is to be rotated. The default is
    /// 1.
    /// </param>
    private void Rotate(int amount = 1)
    {
        // Return without doing anything if the rotation amount is zero.
        if (amount == 0)
        {
            return;
        }

        int[] reflectorTable = new int[TableSize];
        _reflectorTable.CopyTo(reflectorTable, 0);

        // Re-wire the reflector table to account for the amount of rotation.
        for (int index = 0; index < TableSize; index++)
        {
            int rotatedIndex = GetIndex(index, TableSize, -amount);
            int rotatedValue = GetIndex(reflectorTable[rotatedIndex], TableSize, amount);
            _reflectorTable[index] = rotatedValue;
        }

        // Adjust the reflector index to match the amount of rotation.
        _reflectorIndex = GetIndex(_reflectorIndex, TableSize, amount);
    }
}