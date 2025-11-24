namespace DRSSoftware.EnigmaV2;

/// <summary>
/// This interface defines the contract that is common to both <see cref="Reflector" /> and
/// <see cref="Rotor" /> objects.
/// </summary>
/// <remarks>
/// The general term "cipher wheel" refers to either the <see cref="Reflector" /> or
/// <see cref="Rotor" /> objects. Both types of objects are used for transforming an input value to
/// yield an output value.
/// </remarks>
public interface ICipherWheel
{
    /// <summary>
    /// Gets the cipher index value.
    /// </summary>
    /// <remarks>
    /// This value indicates the current rotational position of the <see cref="Reflector" /> or
    /// <see cref="Rotor" /> object. <br /> This value will get reset back to zero if an attempt is
    /// made to increment it past its maximum value.
    /// </remarks>
    public int CipherIndex
    {
        get;
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
    }

    /// <summary>
    /// Gets the cycle size value.
    /// </summary>
    /// <remarks>
    /// This value determines how large the <see cref="CycleCount" /> will get before incrementing
    /// the <see cref="CipherIndex" /> and resetting the <see cref="CycleCount" /> back to zero.
    /// </remarks>
    public int CycleSize
    {
        get;
    }

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
    }

    /// <summary>
    /// Initialize the <see cref="Reflector" /> or <see cref="Rotor" /> object using the given
    /// <paramref name="seed" /> value.
    /// </summary>
    /// <param name="seed">
    /// A <see langword="string" /> value used for randomizing the connections within the
    /// <see cref="Reflector" /> or <see cref="Rotor" /> object. Must not be <see langword="null" />
    /// or empty.
    /// </param>
    public void Initialize(string seed);

    /// <summary>
    /// Set the <see cref="Reflector" /> or <see cref="Rotor" /> index to the desired
    /// <paramref name="indexValue" />.
    /// </summary>
    /// <param name="indexValue">
    /// The integer value that the <see cref="Reflector" /> or <see cref="Rotor" /> index is to be
    /// set to.
    /// </param>
    public void SetIndex(int indexValue);

    /// <summary>
    /// Transform the given <paramref name="originalValue" /> and send the transformed value on to
    /// the next <see cref="Rotor" /> in sequence, or to the <see cref="Reflector" /> if this is the
    /// last <see cref="Rotor" />.
    /// </summary>
    /// <remarks>
    /// The <paramref name="originalValue" /> is an integer representation of a printable ASCII
    /// character which has been adjusted by subtracting the minimum character value from the
    /// original character value.
    /// </remarks>
    /// <param name="originalValue">
    /// The value that is to be transformed.
    /// </param>
    /// <returns>
    /// The final transformed value after it has been processed by each <see cref="Rotor" /> and the
    /// <see cref="Reflector" />.
    /// </returns>
    public int Transform(int originalValue);
}