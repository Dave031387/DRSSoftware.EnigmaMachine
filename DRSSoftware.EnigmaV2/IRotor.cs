namespace DRSSoftware.EnigmaV2;

/// <summary>
/// Defines all methods pertaining to the <see cref="Rotor" /> component of the Enigma V2 machine.
/// </summary>
internal interface IRotor : ICipherWheel
{
    /// <summary>
    /// Get a boolean value indicating whether or not a transform is in progress.
    /// </summary>
    public bool TransformIsInProgress
    {
        get;
    }

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
    /// The <see cref="ICipherWheel" /> object that is to be connected to the outgoing side of this
    /// rotor object.
    /// </param>
    public void ConnectOutgoing(ICipherWheel transformer);
}