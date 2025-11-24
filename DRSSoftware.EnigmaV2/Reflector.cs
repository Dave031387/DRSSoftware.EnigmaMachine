namespace DRSSoftware.EnigmaV2;

/// <summary>
/// This class models the <see cref="Reflector" /> component of the <see cref="EnigmaMachine" />.
/// </summary>
/// <remarks>
/// This component comes after the last <see cref="Rotor" /> component. It takes the value coming in
/// from the last <see cref="Rotor" />, transforms that value to a new value, and sends the new
/// value back to the <see cref="Rotor" />.
/// </remarks>
/// <param name="cycleSize">
/// An integer value that specifies how many transforms should be performed between each rotation of
/// the <see cref="Reflector" />.
/// </param>
internal sealed class Reflector(int cycleSize) : CipherWheel(cycleSize), IReflector
{
    /// <summary>
    /// This table is used to look up the transformed value corresponding to the value that was sent
    /// from the last <see cref="Rotor" /> object. <br /> The transformed value is then sent back to
    /// the last <see cref="Rotor" />.
    /// </summary>
    /// <remarks>
    /// The transformation is always reversible. For example, if the transform of 'A' yields 'X',
    /// then the transform of 'X' will yield 'A'.
    /// </remarks>
    private readonly int[] _outboundTransformTable = new int[TableSize];

    /// <summary>
    /// Gets a copy of the outbound transform table.
    /// </summary>
    /// <remarks>
    /// This property is intended for unit testing purposes only.
    /// </remarks>
    internal int[] OutboundTransformTable
    {
        get
        {
            int[] copyTable = new int[TableSize];
            _outboundTransformTable.CopyTo(copyTable, 0);
            return copyTable;
        }
    }

    /// <summary>
    /// Connect the specified <paramref name="rotor" /> object to the outbound side of this
    /// <see cref="Reflector" /> object.
    /// </summary>
    /// <param name="rotor">
    /// The <see cref="Rotor" /> object that is to be connected to this <see cref="Reflector" />
    /// object.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="rotor" /> parameter is <see langword="null" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called more than once for this <see cref="Reflector" /> object.
    /// </exception>
    public void ConnectOutboundComponent(IRotor rotor)
    {
        ArgumentNullException.ThrowIfNull(rotor, nameof(rotor));

        if (OutboundCipherWheel is not null)
        {
            throw new InvalidOperationException("Invalid attempt to add an outbound rotor when one is already defined for this reflector.");
        }

        OutboundCipherWheel = rotor;
    }

    /// <summary>
    /// Initialize this <see cref="Reflector" /> object using the specified <paramref name="seed" />
    /// value.
    /// </summary>
    /// <remarks>
    /// The <paramref name="seed" /> value is used for randomizing the connections in the outbound
    /// transform table.
    /// </remarks>
    /// <param name="seed">
    /// A <see langword="string" /> value used for randomizing the connections within this
    /// <see cref="Reflector" /> object. Must not be <see langword="null" /> and must be at least 10
    /// characters in length.
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
            index1 = FindAvailableConnectionPoint(index1, slotIsTaken);
            seedIndex = GetIndexValueWithOffset(seedIndex, arraySize, 1);

            // Determine the second connection point to be wired up.
            index2 = DisplaceIndex(index2, displacements[seedIndex]);
            index2 = FindAvailableConnectionPoint(index2, slotIsTaken);
            seedIndex = GetIndexValueWithOffset(seedIndex, arraySize, 1);

            // Here we "wire" two positions on the reflector together. If we connect point 3 to
            // point 17, then we also connect point 17 to point 3, for example.
            _outboundTransformTable[index1] = index2;
            _outboundTransformTable[index2] = index1;

            // Keep track of how many positions haven't been wired yet. Note that the total number
            // of positions must be an even number for this to work out correctly.
            slotsRemaining -= 2;
        }

        CipherIndex = 0;
        CycleCount = 0;
        IsInitialized = true;
    }

    /// <summary>
    /// Transform the given <paramref name="originalValue" /> and send the transformed value back to
    /// the last <see cref="Rotor" />.
    /// </summary>
    /// <remarks>
    /// The <paramref name="originalValue" /> is an integer representation of a printable ASCII
    /// character which has been adjusted by subtracting the minimum character value from the
    /// original character value.
    /// </remarks>
    /// <param name="originalValue">
    /// The original value that is to be transformed.
    /// </param>
    /// <returns>
    /// The final transformed value after it has been processed by all of the cipher wheels (rotors
    /// and the reflector).
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called prior to initializing this <see cref="Reflector" /> object
    /// or if an outbound <see cref="Rotor" /> object hasn't been connected to this
    /// <see cref="Reflector" />.
    /// </exception>
    public override int Transform(int originalValue)
    {
        if (IsInitialized)
        {
            if (OutboundCipherWheel is null)
            {
                throw new InvalidOperationException("The outbound rotor hasn't been connected to the reflector.");
            }

            Rotate();

            int transformedValue = GetTransformedValue(_outboundTransformTable, originalValue);

            return OutboundCipherWheel.Transform(transformedValue);
        }

        throw new InvalidOperationException("The reflector must be initialized before calling the Transform method.");
    }
}