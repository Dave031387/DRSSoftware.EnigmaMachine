namespace DRSSoftware.EnigmaV2;

/// <summary>
/// This class models the "rotor" component of the Enigma V2 machine.
/// </summary>
/// <remarks>
/// The rotor takes an incoming cipher value, transforms it, and sends it on to the next rotor (or
/// the reflector if this is the last rotor in the series). <br /> The rotor can also take a cipher
/// value coming back from the outgoing component, transform it, and send it back to the previous
/// rotor in the series (or back to the user if this is the first rotor).
/// </remarks>
/// <param name="cycleSize">
/// An integer value that specifies how many transforms should be performed between each rotation of
/// the rotor.
/// </param>
internal sealed class Rotor(int cycleSize) : CipherWheel(cycleSize), IRotor
{
    /// <summary>
    /// This table is used to look up the transformed value corresponding to the value coming from
    /// the incoming component of the Enigma V2 machine. The transformed value is then sent on to
    /// the outgoing component.
    /// </summary>
    internal readonly int[] _incomingTable = new int[TableSize];

    /// <summary>
    /// This table is used to look up the transformed value corresponding to the value coming from
    /// the outgoing component of the Enigma V2 machine. The transformed value is then sent on to
    /// the incoming component.
    /// </summary>
    internal readonly int[] _outgoingTable = new int[TableSize];

    /// <summary>
    /// A boolean flag that is set to <see langword="true" /> when an incoming cipher value is
    /// transformed and sent on to the outgoing component. <br /> The flag is reset to
    /// <see langword="false" /> when the transformed cipher value makes the return trip back in
    /// from the outgoing component.
    /// </summary>
    internal bool _transformIsInProgress;

    /// <summary>
    /// Connect the specified <paramref name="rotor" /> object to the incoming side of this
    /// <see cref="Rotor" /> object.
    /// </summary>
    /// <param name="rotor">
    /// The <see cref="IRotor" /> object that is to be connected to this <see cref="Rotor" />
    /// object.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="rotor" /> parameter is <see langword="null" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called more than once for this <see cref="Rotor" /> object.
    /// </exception>
    public void ConnectIncoming(IRotor rotor)
    {
        ArgumentNullException.ThrowIfNull(rotor, nameof(rotor));

        if (_cipherWheelIn is not null)
        {
            throw new InvalidOperationException("Invalid attempt to add an incoming rotor when one is already defined for this rotor.");
        }

        _cipherWheelIn = rotor;
    }

    /// <summary>
    /// Connect the specified <paramref name="cipherWheel" /> object to the outgoing side of this
    /// <see cref="Rotor" /> object.
    /// </summary>
    /// <param name="cipherWheel">
    /// The <see cref="ICipherWheel" /> object (rotor or reflector) that is to be connected to this
    /// <see cref="Rotor" /> object.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="cipherWheel" /> parameter is <see langword="null" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called more than once for this <see cref="Rotor" /> object.
    /// </exception>
    public void ConnectOutgoing(ICipherWheel cipherWheel)
    {
        ArgumentNullException.ThrowIfNull(cipherWheel, nameof(cipherWheel));

        if (_cipherWheelOut is not null)
        {
            throw new InvalidOperationException("Invalid attempt to add an outgoing cipher wheel when one is already defined for this rotor.");
        }

        _cipherWheelOut = cipherWheel;
    }

    /// <summary>
    /// Initialize this <see cref="Rotor" /> object using the specified <paramref name="seed" />
    /// value.
    /// </summary>
    /// <remarks>
    /// The <paramref name="seed" /> value is used for randomizing the connections between the
    /// incoming and outgoing sides of this <see cref="Rotor" /> object.
    /// </remarks>
    /// <param name="seed">
    /// A <see langword="string" /> value used for randomizing the connections within this
    /// <see cref="Rotor" /> object.
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

        int seedIndex = 0;
        int index = 0;

        for (int i = 0; i < TableSize; i++)
        {
            // Find an available outgoing slot to connect this incoming position to.
            index = DisplaceIndex(index, displacements[seedIndex]);
            index = FindAvailableSlot(index, slotIsTaken);

            // The incoming and outgoing tables must be mirror images of each other. For example, if
            // we connect point 6 on the incoming side of the rotor to point 57 on the outgoing side
            // of the rotor, then we must also connect point 57 on the outgoing side of the rotor to
            // point 6 on the incoming side of the rotor.
            _incomingTable[i] = index;
            _outgoingTable[index] = i;

            seedIndex = GetValueWithOffset(seedIndex, arraySize, 1);
        }

        _cipherIndex = 0;
        _cycleCount = 0;
        _isInitialized = true;
        _transformIsInProgress = false;
    }

    /// <summary>
    /// Transform the incoming value <paramref name="c" /> and send the transformed value to the
    /// next cipher wheel object (either another <see cref="Rotor" /> or a
    /// <see cref="Reflector" />).
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
    /// Thrown if this method is called prior to initializing this <see cref="Rotor" /> object, or
    /// if an outgoing <see cref="ICipherWheel" /> object hasn't been connected to this
    /// <see cref="Rotor" />.
    /// </exception>
    public override int Transform(int c)
    {
        if (_isInitialized)
        {
            if (_cipherWheelOut is null)
            {
                throw new InvalidOperationException("An outgoing cipher wheel hasn't been connected to this rotor.");
            }

            if (_transformIsInProgress)
            {
                _transformIsInProgress = false;
                int transformOut = GetTransformedValue(_outgoingTable, c);

                return _cipherWheelIn is not null ? _cipherWheelIn.Transform(transformOut) : transformOut;
            }

            _transformIsInProgress = true;
            Rotate();

            int transformIn = GetTransformedValue(_incomingTable, c);

            return _cipherWheelOut.Transform(transformIn);
        }

        throw new InvalidOperationException("The rotor must be initialized before the Transform method is called.");
    }
}