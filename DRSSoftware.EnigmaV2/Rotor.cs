namespace DRSSoftware.EnigmaV2;

/// <summary>
/// This class models the "rotor" component of the Enigma V2 machine.
/// </summary>
/// <remarks>
/// The rotor (also called "cipher wheel") takes an incoming cipher value, transforms it, and sends
/// it on to the next rotor (or the reflector if this is the last rotor in the series). <br /> The
/// rotor can also take a cipher value coming back from the outgoing component, transform it, and
/// send it back to the previous rotor in the series (or back to the user if this is the first
/// rotor).
/// </remarks>
internal class Rotor : IRotor
{
    /// <summary>
    /// This table is used to look up the transformed value for cipher values coming from the
    /// incoming component of the Enigma V2 machine. The transformed value is then sent on to the
    /// outgoing component.
    /// </summary>
    internal readonly int[] _incomingTable = new int[TableSize];

    /// <summary>
    /// This table is used to look up the transformed value for cipher values coming from the
    /// outgoing component of the Enigma V2 machine. The transformed value is then sent on to the
    /// incoming component.
    /// </summary>
    internal readonly int[] _outgoingTable = new int[TableSize];

    /// <summary>
    /// A boolean flag that gets set to <see langword="true" /> when this <see cref="Rotor" />
    /// instance has been initialized and is ready for use.
    /// </summary>
    internal bool _isInitialized;

    /// <summary>
    /// A reference to the <see cref="IRotor" /> object that comes before this <see cref="Rotor" />
    /// object, or <see langword="null" /> if this is the first <see cref="Rotor" /> in sequence.
    /// </summary>
    internal IRotor? _rotorIn;

    /// <summary>
    /// An integer that indicates how far this rotor has been rotated from its starting position.
    /// </summary>
    internal int _rotorIndex;

    /// <summary>
    /// A reference to the <see cref="ITransformer" /> object (either rotor or reflector) that comes
    /// after this <see cref="Rotor" /> object.
    /// </summary>
    internal ITransformer? _transformerOut;

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

        if (_rotorIn is not null)
        {
            throw new InvalidOperationException("Invalid attempt to add an incoming rotor when one is already defined for this rotor.");
        }

        _rotorIn = rotor;
    }

    /// <summary>
    /// Connect the specified <paramref name="transformer" /> object to the outgoing side of this
    /// <see cref="Rotor" /> object.
    /// </summary>
    /// <param name="transformer">
    /// The <see cref="ITransformer" /> object (rotor or reflector) that is to be connected to this
    /// <see cref="Rotor" /> object.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="transformer" /> parameter is <see langword="null" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called more than once for this <see cref="Rotor" /> object.
    /// </exception>
    public void ConnectOutgoing(ITransformer transformer)
    {
        ArgumentNullException.ThrowIfNull(transformer, nameof(transformer));

        if (_transformerOut is not null)
        {
            throw new InvalidOperationException("Invalid attempt to add an outgoing transformer when one is already defined for this rotor.");
        }

        _transformerOut = transformer;
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
    public void Initialize(string seed)
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

            seedIndex = GetIndex(seedIndex, displacements.Length, 1);
        }

        _rotorIndex = 0;
        _isInitialized = true;
    }

    /// <summary>
    /// Set the rotor index to the desired <paramref name="indexValue" />.
    /// </summary>
    /// <remarks>
    /// Note that this operation also involves rotating this <see cref="Rotor" /> object by the
    /// corresponding amount.
    /// </remarks>
    /// <param name="indexValue">
    /// The integer value that the rotor index is to be set to.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="indexValue" /> is less than zero or greater than or equal to
    /// the table size.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called prior to initializing this <see cref="Rotor" /> object.
    /// </exception>
    public void SetIndex(int indexValue)
    {
        if (_isInitialized)
        {
            if (indexValue is < 0 or > MaxIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(indexValue), $"The value passed into the SetIndex method must be greater than or equal to zero and less than {TableSize}, but it was {indexValue}.");
            }

            _rotorIndex = indexValue;
        }
        else
        {
            throw new InvalidOperationException("The rotor must be initialized before the index can be set.");
        }
    }

    /// <summary>
    /// Transform the incoming cipher value <paramref name="c" /> and send the transformed value to
    /// the outgoing <see cref="ITransformer" /> object (either another <see cref="Rotor" /> or a
    /// <see cref="Reflector" />).
    /// </summary>
    /// <remarks>
    /// The cipher value is an integer representation of a printable ASCII character which has been
    /// adjusted by subtracting the minimum character value from the cipher character value.
    /// </remarks>
    /// <param name="c">
    /// The cipher value that is to be transformed.
    /// </param>
    /// <param name="shouldRotate">
    /// A flag indicating whether or not this <see cref="Rotor" /> should be rotated one position
    /// before applying the transform.
    /// </param>
    /// <returns>
    /// The final transformed cipher value after it has been processed by all of the cipher wheels
    /// (rotors) and the reflector.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called prior to initializing this <see cref="Rotor" /> object, or
    /// if an outgoing <see cref="ITransformer" /> object hasn't been connected to this
    /// <see cref="Rotor" />, or if the <see cref="TransformOut(int)" /> method hasn't been called
    /// after the last call to this method.
    /// </exception>
    public int TransformIn(int c, bool shouldRotate)
    {
        if (_isInitialized)
        {
            if (_transformIsInProgress)
            {
                throw new InvalidOperationException("The TransformOut method wasn't called after the last call to this method.");
            }

            _transformIsInProgress = true;
            bool shouldRotateNext = false;

            if (shouldRotate)
            {
                _rotorIndex = GetIndex(_rotorIndex, TableSize, 1);
                shouldRotateNext = _rotorIndex == 0;
            }

            int index = GetIndex(c, TableSize, -_rotorIndex);
            int transformedValue = GetIndex(_incomingTable[index], TableSize, _rotorIndex);

            return _transformerOut is not null
                ? _transformerOut.TransformIn(transformedValue, shouldRotateNext)
                : throw new InvalidOperationException("An outgoing transformer hasn't been connected to this rotor.");
        }

        throw new InvalidOperationException("The rotor must be initialized before the TransformIn method is called.");
    }

    /// <summary>
    /// Transform the incoming cipher value <paramref name="c" /> and send the transformed value to
    /// the incoming <see cref="IRotor" /> object.
    /// </summary>
    /// <remarks>
    /// The cipher value is an integer representation of a printable ASCII character which has been
    /// adjusted by subtracting the minimum character value from the cipher character value.
    /// </remarks>
    /// <param name="c">
    /// The cipher value that is to be transformed.
    /// </param>
    /// <returns>
    /// The final transformed cipher value after it has been processed by all of the cipher wheels
    /// (rotors) and the reflector.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called before calling the <see cref="TransformIn(int, bool)" />
    /// method on this <see cref="Rotor" /> object.
    /// </exception>
    public int TransformOut(int c)
    {
        if (_transformIsInProgress)
        {
            _transformIsInProgress = false;
            int index = GetIndex(c, TableSize, -_rotorIndex);
            int transformedValue = GetIndex(_outgoingTable[index], TableSize, _rotorIndex);

            return _rotorIn is not null ? _rotorIn.TransformOut(transformedValue) : transformedValue;
        }

        throw new InvalidOperationException("The TransformOut method must not be called on the rotor before calling the TransformIn method.");
    }
}