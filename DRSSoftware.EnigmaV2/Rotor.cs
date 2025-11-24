namespace DRSSoftware.EnigmaV2;

/// <summary>
/// This class models the <see cref="Rotor" /> component of the <see cref="EnigmaMachine" />.
/// </summary>
/// <remarks>
/// The <see cref="Rotor" /> takes the value coming from the inbound component, transforms it, and
/// sends it on to the next <see cref="Rotor" /> in sequence (or to the <see cref="Reflector" /> if
/// this is the last <see cref="Rotor" /> in the series). <br /> The <see cref="Rotor" /> can also
/// take a value coming back from the outbound component, transform it, and send it back to the
/// previous <see cref="Rotor" /> in sequence (or back to the caller if this is the first
/// <see cref="Rotor" />).
/// </remarks>
/// <param name="cycleSize">
/// An integer value that specifies how many transforms should be performed between each rotation of
/// the <see cref="Rotor" />.
/// </param>
public sealed class Rotor(int cycleSize) : CipherWheel(cycleSize), IRotor
{
    /// <summary>
    /// This table is used to look up the transformed value corresponding to the value sent from the
    /// inbound component of this <see cref="Rotor" />. <br /> The transformed value is then sent on
    /// to the outbound component, or returned to the caller if the inbound component is
    /// <see langword="null" />.
    /// </summary>
    private readonly int[] _inboundTransformTable = new int[TableSize];

    /// <summary>
    /// This table is used to look up the transformed value corresponding to the value coming from
    /// the outbound component of this <see cref="Rotor" />. <br /> The transformed value is then
    /// sent on to the inbound component.
    /// </summary>
    private readonly int[] _outboundTransformTable = new int[TableSize];

    /// <summary>
    /// Gets a value indicating whether or not a transform is in progress.
    /// </summary>
    /// <remarks>
    /// If this property is <see langword="false" /> when the <c> "Transform(int)" </c> method is
    /// called, then the <see cref="Rotor" /> knows it is processing an inbound transformation.
    /// <br /> If this property is <see langword="true" /> then the <see cref="Rotor" /> knows it is
    /// processing an outbound transformation that is ultimately to be returned back to the caller.
    /// </remarks>
    public bool TransformIsInProgress
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets a copy of the inbound transform table.
    /// </summary>
    /// <remarks>
    /// This property is intended for unit testing purposes only.
    /// </remarks>
    internal int[] InboundTransformTable
    {
        get
        {
            int[] copyTable = new int[TableSize];
            _inboundTransformTable.CopyTo(copyTable, 0);
            return copyTable;
        }
    }

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
    /// Connect the specified <paramref name="rotor" /> object to the inbound side of this
    /// <see cref="Rotor" /> object.
    /// </summary>
    /// <param name="rotor">
    /// The inbound <see cref="Rotor" /> object that is to be connected to this <see cref="Rotor" />
    /// object.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="rotor" /> parameter is <see langword="null" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called more than once for this <see cref="Rotor" /> object.
    /// </exception>
    public void ConnectInboundComponent(IRotor rotor)
    {
        ArgumentNullException.ThrowIfNull(rotor, nameof(rotor));

        if (InboundCipherWheel is not null)
        {
            throw new InvalidOperationException("Invalid attempt to add an inbound rotor when one is already defined for this rotor.");
        }

        InboundCipherWheel = rotor;
    }

    /// <summary>
    /// Connect the specified <paramref name="cipherWheel" /> object (either a <see cref="Rotor" />
    /// or <see cref="Reflector" />) to the outbound side of this <see cref="Rotor" /> object.
    /// </summary>
    /// <param name="cipherWheel">
    /// The outbound <see cref="CipherWheel" /> object ( <see cref="Rotor" /> or
    /// <see cref="Reflector" />) that is to be connected to this <see cref="Rotor" /> object.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="cipherWheel" /> parameter is <see langword="null" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called more than once for this <see cref="Rotor" /> object.
    /// </exception>
    public void ConnectOutboundComponent(ICipherWheel cipherWheel)
    {
        ArgumentNullException.ThrowIfNull(cipherWheel, nameof(cipherWheel));

        if (OutboundCipherWheel is not null)
        {
            throw new InvalidOperationException("Invalid attempt to add an outbound cipher wheel when one is already defined for this rotor.");
        }

        OutboundCipherWheel = cipherWheel;
    }

    /// <summary>
    /// Initialize this <see cref="Rotor" /> object using the specified <paramref name="seed" />
    /// value.
    /// </summary>
    /// <remarks>
    /// The <paramref name="seed" /> value is used for randomizing the connections between the
    /// inbound and outbound sides of this <see cref="Rotor" /> object.
    /// </remarks>
    /// <param name="seed">
    /// A <see langword="string" /> value used for randomizing the connections within this
    /// <see cref="Rotor" /> object. Must not be <see langword="null" /> and must be at least 10
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

        int seedIndex = 0;
        int outboundIndex = 0;

        for (int inboundIndex = 0; inboundIndex < TableSize; inboundIndex++)
        {
            // Find an available outbound point to connect the inbound point to.
            outboundIndex = DisplaceIndex(outboundIndex, displacements[seedIndex]);
            outboundIndex = FindAvailableConnectionPoint(outboundIndex, slotIsTaken);

            // The inbound and outbound transform tables must be mirror images of each other. For
            // example, if index position 6 of the inbound transform table contains the value 57,
            // then index position 57 of the outbound transform table must contain the value 6.
            _inboundTransformTable[inboundIndex] = outboundIndex;
            _outboundTransformTable[outboundIndex] = inboundIndex;

            // Increment the index to the displacements array. Wrap back around to the beginning if
            // we go over the maximum index value.
            seedIndex = GetIndexValueWithOffset(seedIndex, arraySize, 1);
        }

        CipherIndex = 0;
        CycleCount = 0;
        IsInitialized = true;
        TransformIsInProgress = false;
    }

    /// <summary>
    /// Transform the given <paramref name="originalValue" /> using either the inbound transform
    /// table (if <c> TransformIsInProgress </c> is <see langword="true" />) or the outbound
    /// transform table (if <c> TransformIsInProgress </c> is <see langword="false" />).
    /// </summary>
    /// <remarks>
    /// The <paramref name="originalValue" /> is an integer representation of a printable ASCII
    /// character which has been adjusted by subtracting the minimum character value from the
    /// original character value.
    /// <para />
    /// The transformed value is returned to the caller if this is the first <see cref="Rotor" /> in
    /// sequence and <c> TransformIsInProgress </c> is <see langword="true" />. <br /> Otherwise,
    /// the transformed value is sent on to the next outbound component (if <c>
    /// TransformIsInProgress </c> is <see langword="false" />), or the next inbound component (if
    /// <c> TransformIsInProgress </c> is <see langword="true" />).
    /// </remarks>
    /// <param name="originalValue">
    /// The original value that is to be transformed.
    /// </param>
    /// <returns>
    /// The final transformed value after it has been processed by all of the cipher wheels (rotors
    /// and the reflector).
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called prior to initializing this <see cref="Rotor" /> object, or
    /// if an outbound <see cref="CipherWheel" /> object hasn't been connected to this
    /// <see cref="Rotor" />.
    /// </exception>
    public override int Transform(int originalValue)
    {
        if (IsInitialized)
        {
            if (OutboundCipherWheel is null)
            {
                throw new InvalidOperationException("An outbound cipher wheel hasn't been connected to this rotor.");
            }

            if (TransformIsInProgress)
            {
                TransformIsInProgress = false;
                int transformedValueOut = GetTransformedValue(_outboundTransformTable, originalValue);

                return InboundCipherWheel is not null ? InboundCipherWheel.Transform(transformedValueOut) : transformedValueOut;
            }

            TransformIsInProgress = true;
            Rotate();

            int transformedValueIn = GetTransformedValue(_inboundTransformTable, originalValue);

            return OutboundCipherWheel.Transform(transformedValueIn);
        }

        throw new InvalidOperationException("The rotor must be initialized before the Transform method is called.");
    }

    /// <summary>
    /// Gets the value from the inbound transform table at the specified <paramref name="index" />
    /// location.
    /// </summary>
    /// <remarks>
    /// This method is intended for unit testing purposes only.
    /// </remarks>
    /// <param name="index">
    /// The index of the value to be returned.
    /// </param>
    /// <returns>
    /// The value located at the specified <paramref name="index" /> location of the inbound
    /// transform table.
    /// </returns>
    internal int GetInboundValue(int index) => _inboundTransformTable[index];

    /// <summary>
    /// Gets the value from the outbound transform table at the specified <paramref name="index" />
    /// location.
    /// </summary>
    /// <remarks>
    /// This method is intended for unit testing purposes only.
    /// </remarks>
    /// <param name="index">
    /// The index of the value to be returned.
    /// </param>
    /// <returns>
    /// The value located at the specified <paramref name="index" /> location of the outbound
    /// transform table.
    /// </returns>
    internal int GetOutboundValue(int index) => _outboundTransformTable[index];

    /// <summary>
    /// This method allows a test method to set the initial state of this <see cref="Rotor" />
    /// object prior to executing the test.
    /// </summary>
    /// <remarks>
    /// This method is intended for unit testing purposes only.
    /// </remarks>
    /// <param name="cipherIndex">
    /// The value of the cipher index.
    /// </param>
    /// <param name="cycleCount">
    /// The value of the cycle count.
    /// </param>
    /// <param name="isInitialized">
    /// A value indicating whether or not the instance is initialized.
    /// </param>
    /// <param name="transformIsInProgress">
    /// A value indicating whether or not a transform is in progress.
    /// </param>
    internal void SetState(int? cipherIndex, int? cycleCount, bool? isInitialized, bool? transformIsInProgress)
    {
        if (transformIsInProgress.HasValue)
        {
            TransformIsInProgress = (bool)transformIsInProgress;
        }

        base.SetState(cipherIndex, cycleCount, isInitialized);
    }
}