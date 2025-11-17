namespace DRSSoftware.EnigmaV2;

/// <summary>
/// This class models the "reflector" component of the Enigma V2 machine.
/// </summary>
/// <remarks>
/// This component comes after the last cipher wheel (or <see cref="Rotor" />). It takes the value
/// coming in from the cipher wheel, transforms that value to a new value, and sends the new value
/// back to the cipher wheel.
/// </remarks>
/// <param name="cycleSize">
/// An integer value that specifies how many transforms should be performed between each rotation of
/// the reflector.
/// </param>
internal sealed class Reflector(int cycleSize) : CipherWheel(cycleSize), IReflector
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
    /// Connect the specified <paramref name="rotor" /> object to the outgoing side of this
    /// <see cref="Reflector" /> object.
    /// </summary>
    /// <param name="rotor">
    /// The <see cref="IRotor" /> object that is to be connected to this <see cref="Reflector" />
    /// object.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="rotor" /> parameter is <see langword="null" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called more than once for this <see cref="Reflector" /> object.
    /// </exception>
    public void ConnectOutgoing(IRotor rotor)
    {
        ArgumentNullException.ThrowIfNull(rotor, nameof(rotor));

        if (_cipherWheelOut is not null)
        {
            throw new InvalidOperationException("Invalid attempt to add an outgoing rotor when one is already defined for this reflector.");
        }

        _cipherWheelOut = rotor;
    }

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
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="seed" /> parameter is <see langword="null" />.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown if the <paramref name="seed" /> parameter is less than the minimum length specified
    /// by <see cref="MinSeedLength" />.
    /// </exception>
    public override void Initialize(string seed)
    {
        ArgumentNullException.ThrowIfNull(seed, nameof(seed));

        if (seed.Length < MinSeedLength)
        {
            throw new ArgumentException($"The seed string passed into the Initialize method must be at least {MinSeedLength} characters long, but it was {seed.Length}.", nameof(seed));
        }

        // This array is used to keep track of which index positions on this reflector object have
        // already been connected to some other index position.
        bool[] slotIsTaken = new bool[TableSize];
        char[] displacements = seed.ToCharArray();
        int arraySize = displacements.Length;
        int slotsRemaining = TableSize;
        int seedIndex = 0;
        int index1 = 0;
        int index2 = TableSize / 2;

        while (slotsRemaining > 0)
        {
            // Determine the first connection point to be wired up.
            index1 = DisplaceIndex(index1, displacements[seedIndex]);
            index1 = FindAvailableSlot(index1, slotIsTaken);
            seedIndex = GetValueWithOffset(seedIndex, arraySize, 1);

            // Determine the second connection point to be wired up.
            index2 = DisplaceIndex(index2, displacements[seedIndex]);
            index2 = FindAvailableSlot(index2, slotIsTaken);
            seedIndex = GetValueWithOffset(seedIndex, arraySize, 1);

            // Here we "wire" two positions on the reflector together. If we connect point 3 to
            // point 17, then we also connect point 17 to point 3, for example.
            _reflectorTable[index1] = index2;
            _reflectorTable[index2] = index1;

            // Keep track of how many positions haven't been wired yet. Note that the total number
            // of positions must be an even number for this to work out correctly.
            slotsRemaining -= 2;
        }

        _cipherIndex = 0;
        _cycleCount = 0;
        _isInitialized = true;
    }

    /// <summary>
    /// Transform the incoming value <paramref name="c" /> and send the transformed value to the
    /// outgoing cipher wheel (always a <see cref="Rotor" />).
    /// </summary>
    /// <remarks>
    /// The incoming value is an integer representation of a printable ASCII character which has
    /// been adjusted by subtracting the minimum character value from the incoming character value.
    /// </remarks>
    /// <param name="c">
    /// The incoming value that is to be transformed.
    /// </param>
    /// <returns>
    /// The final transformed value after it has been processed by all of the cipher wheels (rotors
    /// and the reflector).
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called prior to initializing this <see cref="Reflector" /> object
    /// or if an outgoing <see cref="Rotor" /> object hasn't been connected to this
    /// <see cref="Reflector" />.
    /// </exception>
    public override int Transform(int c)
    {
        if (_isInitialized)
        {
            if (_cipherWheelOut is null)
            {
                throw new InvalidOperationException("The outgoing rotor hasn't been connected to the reflector.");
            }

            Rotate();

            int transformedValue = GetTransformedValue(_reflectorTable, c);

            return _cipherWheelOut.Transform(transformedValue);
        }

        throw new InvalidOperationException("The reflector must be initialized before calling the Transform method.");
    }
}