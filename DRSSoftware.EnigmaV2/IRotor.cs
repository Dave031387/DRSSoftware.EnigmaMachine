namespace DRSSoftware.EnigmaV2;

/// <summary>
/// Defines all methods pertaining to the <see cref="Rotor" /> component of the Enigma V2 machine.
/// </summary>
internal interface IRotor : ITransformer
{
    /// <summary>
    /// Connect the specified <paramref name="rotor" /> object to the incoming side of this rotor
    /// object.
    /// </summary>
    /// <remarks>
    /// The first rotor in sequence won't have any incoming rotor object attached to it.
    /// </remarks>
    /// <param name="rotor">
    /// The <see cref="IRotor" /> object that is to be connected to the incoming side of this rotor
    /// object.
    /// </param>
    public void ConnectIncoming(IRotor rotor);

    /// <summary>
    /// Connect the specified <paramref name="transformer" /> object (either rotor or reflector) to
    /// the outgoing side of this rotor object.
    /// </summary>
    /// <remarks>
    /// The outgoing side of the last rotor in sequence will be connected to a reflector object. All
    /// other rotors will be connected to the next rotor in sequence.
    /// </remarks>
    /// <param name="transformer">
    /// The <see cref="ITransformer" /> object that is to be connected to the outgoing side of this
    /// rotor object.
    /// </param>
    public void ConnectOutgoing(ITransformer transformer);

    /// <summary>
    /// Transform the outgoing cipher value <paramref name="c" /> and send the transformed value on
    /// to the prior rotor in sequence, or return the final value if this is the first rotor.
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
    public int TransformOut(int c);
}