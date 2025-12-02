namespace DRSSoftware.EnigmaV2;

/// <summary>
/// The <see cref="CipherWheel" /> class provides a base implementation for both the
/// <see cref="Reflector" /> and <see cref="Rotor" /> classes.
/// </summary>
/// <remarks>
/// The general term "cipher wheel" refers to either the <see cref="Reflector" /> or
/// <see cref="Rotor" /> objects. Both types of objects are used for transforming an input value to
/// yield an output value.
/// </remarks>
/// <param name="cycleSize">
/// An integer that indicates how many transforms will be performed between each rotation of the
/// <see cref="CipherWheel" />.
/// </param>
public abstract class CipherWheel(int cycleSize) : ICipherWheel
{
    /// <summary>
    /// Get the cipher index value.
    /// </summary>
    /// <remarks>
    /// This value indicates the current rotational position of the <see cref="Reflector" /> or
    /// <see cref="Rotor" /> object. <br /> This value will get reset back to zero if an attempt is
    /// made to increment it past its maximum value.
    /// </remarks>
    public int CipherIndex
    {
        get;
        private protected set;
    }

    /// <summary>
    /// Gets the cycle count value.
    /// </summary>
    /// <remarks>
    /// This value indicates how many character transformations have been performed since the last
    /// time the <see cref="CipherIndex" /> value was incremented.
    /// </remarks>
    public int CycleCount
    {
        get;
        private protected set;
    }

    /// <summary>
    /// Gets the cycle size value.
    /// </summary>
    /// <remarks>
    /// This value determines how large the <see cref="CycleCount" /> will get before incrementing
    /// the <see cref="CipherIndex" /> and resetting the <see cref="CycleCount" /> back to zero.
    /// <br /> This value is constrained to be within the range from 0 to <see cref="MaxIndex" />. A
    /// value of 0 indicates that the cipher wheel should never be rotated.
    /// </remarks>
    public int CycleSize
    {
        get;
    } = cycleSize < 0 ? 0 : cycleSize > MaxIndex ? MaxIndex : cycleSize;

    /// <summary>
    /// Gets a value indicating whether this instance has been initialized or not.
    /// </summary>
    /// <remarks>
    /// This property is set to <see langword="true" /> once the <c> "Initialize(string)" </c>
    /// method has successfully completed.
    /// </remarks>
    public bool IsInitialized
    {
        get;
        private protected set;
    }

    /// <summary>
    /// Gets a reference to the <see cref="Rotor" /> or <see cref="Reflector" /> object that comes
    /// after this <see cref="CipherWheel" /> object.
    /// </summary>
    /// <remarks>
    /// This property will reference the <see cref="Reflector" /> object if this
    /// <see cref="CipherWheel" /> object is the last <see cref="Rotor" /> in sequence. <br /> Or,
    /// it will be <see langword="null" /> if this <see cref="CipherWheel" /> object is the
    /// <see cref="Reflector" />. <br /> Otherwise, this property will always reference the next
    /// <see cref="Rotor" /> object in sequence.
    /// </remarks>
    internal ICipherWheel? LeftCipherWheel
    {
        get;
        private protected set;
    }

    /// <summary>
    /// Gets a reference to the <see cref="Rotor" /> object that comes before this
    /// <see cref="CipherWheel" /> object.
    /// </summary>
    /// <remarks>
    /// This property will be <see langword="null" /> if this <see cref="CipherWheel" /> object is
    /// the first <see cref="Rotor" /> object in sequence. <br /> Or it will reference the last
    /// <see cref="Rotor" /> in sequence if this <see cref="CipherWheel" /> object is the
    /// <see cref="Reflector" /> object. <br /> Otherwise, this property will always reference the
    /// previous <see cref="Rotor" /> object in sequence.
    /// </remarks>
    internal ICipherWheel? RightCipherWheel
    {
        get;
        private protected set;
    }

    /// <summary>
    /// Initialize this <see cref="CipherWheel" /> object using the specified
    /// <paramref name="seed" /> value.
    /// </summary>
    /// <remarks>
    /// The <paramref name="seed" /> value is used for randomizing the connections between the right
    /// and left sides of this <see cref="CipherWheel" /> object.
    /// </remarks>
    /// <param name="seed">
    /// A <see langword="string" /> value used for randomizing the connections within this
    /// <see cref="CipherWheel" /> object. Must not be <see langword="null" />.
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
    /// Thrown if the <paramref name="indexValue" /> is less than zero or greater than the maximum
    /// index value.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this method is called prior to initializing this <see cref="CipherWheel" />
    /// object.
    /// </exception>
    public void SetCipherIndex(int indexValue)
    {
        if (IsInitialized)
        {
            if (indexValue is < 0 or > MaxIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(indexValue), $"The index value passed into the {nameof(SetCipherIndex)} method of the {GetType().Name} class must be greater than or equal to 0 and less than {TableSize}, but it was {indexValue}.");
            }

            CipherIndex = indexValue;
            CycleCount = CycleSize < 2 || CipherIndex < 2 ? 0 : CipherIndex % CycleSize;
        }
        else
        {
            throw new InvalidOperationException($"The {GetType().Name} must be initialized before calling the {nameof(SetCipherIndex)} method.");
        }
    }

    /// <summary>
    /// Transform the given <paramref name="originalValue" /> and send the transformed value to the
    /// <c> "Transform(int)" </c> method of the next <see cref="CipherWheel" /> object in sequence
    /// (either a <see cref="Rotor" /> or a <see cref="Reflector" />).
    /// </summary>
    /// <remarks>
    /// The <paramref name="originalValue" /> is an integer representation of a printable ASCII
    /// character which has been adjusted by subtracting the minimum character value from the
    /// original character value.
    /// <para />
    /// When the transformed value has passed through each <see cref="Rotor" />, the
    /// <see cref="Reflector" />, and back through each <see cref="Rotor" />, the final transformed
    /// value is returned to the caller.
    /// </remarks>
    /// <param name="originalValue">
    /// The original value that is to be transformed.
    /// </param>
    /// <returns>
    /// The final transformed value after it has made a complete circuit through all of the cipher
    /// wheels and back again.
    /// </returns>
    public abstract int Transform(int originalValue);

    /// <summary>
    /// This method allows a test method to set the initial state of the <see cref="CipherWheel" />
    /// before executing the test.
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
    internal void SetState(int? cipherIndex, int? cycleCount, bool? isInitialized)
    {
        if (cipherIndex.HasValue)
        {
            CipherIndex = (int)cipherIndex;
        }

        if (cycleCount.HasValue)
        {
            CycleCount = (int)cycleCount;
        }

        if (isInitialized.HasValue)
        {
            IsInitialized = (bool)isInitialized;
        }
    }

    /// <summary>
    /// Calculates the adjusted index value within the bounds of an array using the specified
    /// <paramref name="baseValue" /> and <paramref name="offset" />, wrapping around if the result
    /// exceeds the array size or is negative.
    /// </summary>
    /// <remarks>
    /// This method uses the C# modulus operator on potentially negative numbers. When this occurs,
    /// the result will be either negative or zero. <br /> If the result is negative, the method
    /// adjusts it by adding the <paramref name="arraySize" /> to ensure a positive index within the
    /// valid range.
    /// </remarks>
    /// <param name="baseValue">
    /// The base index value to start from. Should be a positive value or zero.
    /// </param>
    /// <param name="arraySize">
    /// The size of the array. Must be greater than zero.
    /// </param>
    /// <param name="offset">
    /// The offset that is to be applied to the <paramref name="baseValue" />. May be positive or
    /// negative or zero.
    /// </param>
    /// <returns>
    /// The adjusted value, guaranteed to be within the range [0, <paramref name="arraySize" /> -
    /// 1].
    /// </returns>
    protected static int GetIndexValueWithOffset(int baseValue, int arraySize, int offset)
    {
        int index = (baseValue + offset) % arraySize;
        return index < 0 ? arraySize + index : index;
    }

    /// <summary>
    /// Calculates a new index value by applying an offset derived from the specified
    /// <paramref name="seed" /> character.
    /// </summary>
    /// <param name="index">
    /// The original index to be displaced. Must be greater than or equal to 0 and less than the
    /// maximum index value.
    /// </param>
    /// <param name="seed">
    /// The character used to determine the displacement offset. If the character is outside the
    /// valid range, a default offset of 1 is used.
    /// </param>
    /// <returns>
    /// The displaced index, adjusted based on the specified <paramref name="seed" /> character and
    /// constrained by the maximum allowed index value.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <paramref name="index" /> is less than 0 or greater than the maximum allowable
    /// index value.
    /// </exception>
    protected int DisplaceIndex(int index, char seed)
    {
        if (index is < 0 or > MaxIndex)
        {
            throw new ArgumentOutOfRangeException(nameof(index), $"The index value passed into the {nameof(DisplaceIndex)} method of the {GetType().Name} class must be greater than or equal to 0 and less than {TableSize}, but it was {index}.");
        }

        int seedValue = CharToInt(seed);
        int offset = seedValue < 1 ? 1 : seedValue;
        return GetIndexValueWithOffset(index, TableSize, offset);
    }

    /// <summary>
    /// Finds the first unassigned connection point on a <see cref="Reflector" /> or
    /// <see cref="Rotor" />, starting from the specified index. <br /> Used during initialization
    /// when "wiring" the connection points of the <see cref="Reflector" /> or <see cref="Rotor" />
    /// based on a given seed value.
    /// </summary>
    /// <remarks>
    /// This method modifies the <paramref name="slotIsTaken" /> array by marking the returned
    /// connection point as taken. <br /> The search wraps around in a circular manner, ensuring
    /// that all connection points are checked exactly once.
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
    /// The index of the first available slot that was found, starting from
    /// <paramref name="startIndex" /> and wrapping around if necessary.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="slotIsTaken" /> array is <see langword="null" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no available slot can be found. (Should never occur if the method is used
    /// correctly.)
    /// </exception>
    protected int FindAvailableConnectionPoint(int startIndex, bool[] slotIsTaken)
    {
        ArgumentNullException.ThrowIfNull(slotIsTaken, nameof(slotIsTaken));

        int index = startIndex;

        while (slotIsTaken[index])
        {
            index = GetIndexValueWithOffset(index, TableSize, 1);

            if (index == startIndex)
            {
                throw new InvalidOperationException($"The {nameof(FindAvailableConnectionPoint)} method of the {GetType().Name} class is unable to find an available connection point.");
            }
        }

        slotIsTaken[index] = true;
        return index;
    }

    /// <summary>
    /// Gets the transformed value corresponding to the given <paramref name="originalValue" /> and
    /// taking into account the current cipher index value.
    /// </summary>
    /// <param name="transformTable">
    /// A table of integers used to look up the transformed value for the given
    /// <paramref name="originalValue" />.
    /// </param>
    /// <param name="originalValue">
    /// The original integer value that is being transformed.
    /// </param>
    /// <returns>
    /// The transformed integer value corresponding to the <paramref name="originalValue" />.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="transformTable" /> array is <see langword="null" />.
    /// </exception>
    protected int GetTransformedValue(int[] transformTable, int originalValue)
    {
        ArgumentNullException.ThrowIfNull(transformTable, nameof(transformTable));

        int index = GetIndexValueWithOffset(originalValue, transformTable.Length, -CipherIndex);
        return GetIndexValueWithOffset(transformTable[index], transformTable.Length, CipherIndex);
    }

    /// <summary>
    /// Increments the cycle count and determines if the new value has reached the cycle size. If
    /// so, the cipher index is advanced (i.e., the <see cref="CipherWheel" /> is rotated one
    /// position) and the cycle count is reset back to zero.
    /// </summary>
    /// <remarks>
    /// This method returns without doing anything if the cycle size is zero. <br /> If the cycle
    /// size is 1, the cipher index value is always advanced, and the cycle count is reset to 0.
    /// <br /> Otherwise, the cipher index value is advanced only if the cycle count reaches the
    /// cycle size value.
    /// </remarks>
    protected void Rotate()
    {
        if (CycleSize > 0)
        {
            if (++CycleCount == CycleSize)
            {
                CipherIndex = GetIndexValueWithOffset(CipherIndex, TableSize, 1);
                CycleCount = 0;
            }
        }
    }
}