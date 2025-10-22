namespace DRSSoftware.EnigmaV2;

/// <summary>
/// This interface defines methods that <see cref="IReflector" /> and <see cref="IRotor" /> share in
/// common.
/// </summary>
/// <remarks>
/// The general term "transformer" refers to either reflector or rotor objects. Both types of
/// objects are used for transforming an input value to yield an output value.
/// </remarks>
internal interface ITransformer
{
    /// <summary>
    /// Initialize the reflector or rotor object using the given <paramref name="seed" /> value.
    /// </summary>
    /// <param name="seed">
    /// A <see langword="string" /> value used for randomizing the connections within the reflector
    /// or rotor object.
    /// </param>
    public void Initialize(string seed);

    /// <summary>
    /// Set the reflector or rotor index to the desired <paramref name="indexValue" />.
    /// </summary>
    /// <param name="indexValue">
    /// The integer value that the reflector or rotor index is to be set to.
    /// </param>
    public void SetIndex(int indexValue);

    /// <summary>
    /// Transform the incoming cipher value <paramref name="c" /> and send the transformed value on
    /// to the next rotor in sequence, or to the reflector if this is the last rotor.
    /// </summary>
    /// <remarks>
    /// The cipher value is an integer representation of a printable ASCII character which has been
    /// adjusted by subtracting the minimum character value from the cipher character value.
    /// </remarks>
    /// <param name="c">
    /// The cipher value that is to be transformed.
    /// </param>
    /// <returns>
    /// The final transformed cipher value after it has been processed by all rotors and the
    /// reflector.
    /// </returns>
    public int TransformIn(int c);
}