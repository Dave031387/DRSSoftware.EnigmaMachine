namespace DRSSoftware.EnigmaV2;

internal abstract class CipherWheel(int cycleSize) : ICipherWheel
{
    /// <summary>
    /// An integer that indicates how far this cipher wheel has been rotated from its starting
    /// position.
    /// </summary>
    protected int _cipherIndex;

    /// <summary>
    /// A reference to the <see cref="ICipherWheel" /> object that comes before this
    /// <see cref="CipherWheel" /> object, or <see langword="null" /> if this
    /// <see cref="CipherWheel" /> is the first <see cref="Rotor" /> in sequence.
    /// </summary>
    /// <remarks>
    /// This field, if not <see langword="null" />, will always reference an <see cref="IRotor" />
    /// object which derives from <see cref="ICipherWheel" />.
    /// </remarks>
    protected ICipherWheel? _cipherWheelIn;

    /// <summary>
    /// A reference to the <see cref="ICipherWheel" /> object (either <see cref="Rotor" /> or
    /// <see cref="Reflector" />) that comes after this <see cref="CipherWheel" /> object.
    /// </summary>
    /// <remarks>
    /// If this <see cref="CipherWheel" /> object is a <see cref="Reflector" /> object, then this
    /// field will always reference an <see cref="IRotor" /> object which derives from
    /// <see cref="ICipherWheel" />.
    /// </remarks>
    protected ICipherWheel? _cipherWheelOut;

    /// <summary>
    /// Contains the count of how many transforms have been performed since the last rotation of the
    /// cipher wheel.
    /// </summary>
    protected int _cycleCount;

    /// <summary>
    /// An integer that indicates how many transforms will be performed between each rotation of the
    /// cipher wheel.
    /// </summary>
    /// <remarks>
    /// This value is constrained to be within the range from 0 to <see cref="MaxIndex" />. A value
    /// of 0 indicates that the cipher wheel should never be rotated.
    /// </remarks>
    protected int _cycleSize = cycleSize < 0 ? 0 : cycleSize > MaxIndex ? MaxIndex : cycleSize;

    /// <summary>
    /// A boolean flag that gets set to <see langword="true" /> when this <see cref="CipherWheel" />
    /// instance has been initialized and is ready for use.
    /// </summary>
    protected bool _isInitialized;

    /// <summary>
    /// Get the cipher index value.
    /// </summary>
    public int CipherIndex => _cipherIndex;

    /// <summary>
    /// Get the cycle count value.
    /// </summary>
    public int CycleCount => _cycleCount;

    /// <summary>
    /// Get the cycle size value.
    /// </summary>
    public int CycleSize => _cycleSize;

    /// <summary>
    /// Get a boolean value indicating whether this instance has been initialized or not.
    /// </summary>
    public bool IsInitialized => _isInitialized;

    /// <summary>
    /// Get a reference to the incoming cipher wheel. This property is intended for unit testing
    /// purposes only.
    /// </summary>
    internal ICipherWheel? CipherWheelIn => _cipherWheelIn;

    /// <summary>
    /// Get a reference to the outgoing cipher wheel. This property is intended for unit testing
    /// purposes only.
    /// </summary>
    internal ICipherWheel? CipherWheelOut => _cipherWheelOut;

    /// <summary>
    /// Initialize this <see cref="CipherWheel" /> object using the specified
    /// <paramref name="seed" /> value.
    /// </summary>
    /// <remarks>
    /// The <paramref name="seed" /> value is used for randomizing the connections between the
    /// incoming and outgoing sides of this <see cref="CipherWheel" /> object.
    /// </remarks>
    /// <param name="seed">
    /// A <see langword="string" /> value used for randomizing the connections within this
    /// <see cref="CipherWheel" /> object.
    /// </param>
    public abstract void Initialize(string seed);

    /// <summary>
    /// Set the cipher index to the desired <paramref name="indexValue" />. Also, set the cycle
    /// count to its initial value based on the value of the cipher index and cycle size.
    /// </summary>
    /// <param name="indexValue">
    /// The integer value that the cipher index is to be set to.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="indexValue" /> is less than zero or greater than or equal to
    /// the table size.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called prior to initializing this <see cref="CipherWheel" />
    /// object.
    /// </exception>
    public void SetIndex(int indexValue)
    {
        if (_isInitialized)
        {
            if (indexValue is < 0 or > MaxIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(indexValue), $"The value passed into the SetIndex method must be greater than or equal to zero and less than {TableSize}, but it was {indexValue}.");
            }

            _cipherIndex = indexValue;
            _cycleCount = _cycleSize < 2 || _cipherIndex < 2 ? 0 : _cipherIndex % _cycleSize;
        }
        else
        {
            throw new InvalidOperationException("The cipher wheel must be initialized before the cipher index can be set.");
        }
    }

    /// <summary>
    /// Transform the incoming value <paramref name="c" /> and send the transformed value to the
    /// outgoing cipher wheel object (either ar <see cref="Rotor" /> or a <see cref="Reflector" />).
    /// </summary>
    /// <remarks>
    /// The incoming value is an integer representation of a printable ASCII character which has
    /// been adjusted by subtracting the minimum character value from the character value.
    /// </remarks>
    /// <param name="c">
    /// The incoming value that is to be transformed.
    /// </param>
    /// <returns>
    /// The final transformed value after it has been processed by all of the cipher wheels.
    /// </returns>
    public abstract int Transform(int c);

    /// <summary>
    /// This method is used strictly for unit testing. It allows the test method to set the initial
    /// state of the cipher wheel.
    /// </summary>
    /// <param name="cipherIndex">
    /// The value of the cipher index.
    /// </param>
    /// <param name="cycleCount">
    /// The value of the cycle count.
    /// </param>
    /// <param name="isInitialized">
    /// A boolean value indicating whether or not the instance is initialized.
    /// </param>
    internal void SetState(int? cipherIndex, int? cycleCount, bool? isInitialized)
    {
        if (cipherIndex is not null)
        {
            _cipherIndex = (int)cipherIndex;
        }

        if (cycleCount is not null)
        {
            _cycleCount = (int)cycleCount;
        }

        if (isInitialized is not null)
        {
            _isInitialized = (bool)isInitialized;
        }
    }

    /// <summary>
    /// Calculates a new index value by applying an offset derived from the specified seed
    /// character.
    /// </summary>
    /// <param name="index">
    /// The original index to be displaced. Must be greater than or equal to 0 and less than
    /// <see cref="MaxIndex" />.
    /// </param>
    /// <param name="seedChar">
    /// The character used to determine the displacement offset. If the character is outside the
    /// valid range, a default offset of 1 is used.
    /// </param>
    /// <returns>
    /// The displaced index, adjusted based on the specified seed character and constrained by
    /// <see cref="TableSize" />.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="index" /> is less than 0 or greater than the maximum allowable
    /// index.
    /// </exception>
    protected static int DisplaceIndex(int index, char seedChar)
    {
        if (index is < 0 or > MaxIndex)
        {
            throw new ArgumentOutOfRangeException(nameof(index), $"The index passed into the DisplaceIndex method must be greater than or equal to zero and less than {TableSize}, but it was {index}.");
        }

        int offset = seedChar is < MinChar or > MaxChar ? 1 : seedChar == MinChar ? 1 : CharToInt(seedChar);
        return GetValueWithOffset(index, TableSize, offset);
    }

    /// <summary>
    /// Finds the first unassigned slot on a <see cref="Reflector" /> or <see cref="Rotor" />,
    /// starting from the specified index. <br /> Used during initialization when "wiring" the
    /// <see cref="Reflector" /> or <see cref="Rotor" /> based on a given seed value.
    /// </summary>
    /// <remarks>
    /// This method modifies the <paramref name="slotIsTaken" /> array by marking the returned slot
    /// as taken. The search wraps around in a circular manner, ensuring that all slots are checked
    /// exactly once.
    /// </remarks>
    /// <param name="startIndex">
    /// The index from which to begin the search for an available slot.
    /// </param>
    /// <param name="slotIsTaken">
    /// A boolean array representing the availability of slots. Each element indicates whether the
    /// corresponding slot is taken ( <see langword="true" />) or available (
    /// <see langword="false" />).
    /// </param>
    /// <returns>
    /// The index of the first available slot, starting from <paramref name="startIndex" /> and
    /// wrapping around if necessary.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no available slot can be found.
    /// </exception>
    protected static int FindAvailableSlot(int startIndex, bool[] slotIsTaken)
    {
        int index = startIndex;

        while (slotIsTaken[index])
        {
            index = GetValueWithOffset(index, TableSize, 1);

            if (index == startIndex)
            {
                throw new InvalidOperationException("The FindAvailableSlot method is unable to find an available slot.");
            }
        }

        slotIsTaken[index] = true;
        return index;
    }

    /// <summary>
    /// Calculates the adjusted value within the bounds of an array using the specified
    /// <paramref name="offset" />, wrapping around if the result exceeds the array size or is
    /// negative.
    /// </summary>
    /// <param name="baseValue">
    /// The base value to start from.
    /// </param>
    /// <param name="arraySize">
    /// The size of the array. Must be greater than zero.
    /// </param>
    /// <param name="offset">
    /// The offset to apply to the base value.
    /// </param>
    /// <returns>
    /// The adjusted value, guaranteed to be within the range [0, <paramref name="arraySize" /> -
    /// 1].
    /// </returns>
    protected static int GetValueWithOffset(int baseValue, int arraySize, int offset)
    {
        int index = baseValue + offset;

        return index >= arraySize ? index - arraySize : index < 0 ? index + arraySize : index;
    }

    /// <summary>
    /// Gets the transformed value corresponding to the given <paramref name="valueIn" /> and taking
    /// into account the current cipher index value.
    /// </summary>
    /// <param name="table">
    /// A table of integers used to look up the transformed value for the given
    /// <paramref name="valueIn" />.
    /// </param>
    /// <param name="valueIn">
    /// The integer value that is being transformed.
    /// </param>
    /// <returns>
    /// The integer value corresponding to the transformed <paramref name="valueIn" />.
    /// </returns>
    protected int GetTransformedValue(int[] table, int valueIn)
    {
        int index = GetValueWithOffset(valueIn, table.Length, -_cipherIndex);
        return GetValueWithOffset(table[index], table.Length, _cipherIndex);
    }

    /// <summary>
    /// Calculates the updated cipher index and cycle count after applying a rotation within a
    /// specified cycle size.
    /// </summary>
    /// <remarks>
    /// If the cycle size is 0, the method returns the input values unchanged. If the cycle size is
    /// 1, the cipher index value is always incremented, and the cycle count is reset to 0. <br />
    /// Otherwise, the cipher index value is incremented whenever the cycle count reaches the cycle
    /// size value.
    /// </remarks>
    protected void Rotate()
    {
        if (_cycleSize > 0)
        {
            if (_cycleSize is 1 || ++_cycleCount == _cycleSize)
            {
                _cipherIndex = GetValueWithOffset(_cipherIndex, TableSize, 1);
                _cycleCount = 0;
            }
        }
    }
}