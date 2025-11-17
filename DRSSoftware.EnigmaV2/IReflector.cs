namespace DRSSoftware.EnigmaV2;

/// <summary>
/// Defines all methods pertaining to the <see cref="Reflector" /> component of the Enigma V2
/// machine.
/// </summary>
internal interface IReflector : ICipherWheel
{
    /// <summary>
    /// Connect the specified <paramref name="rotor" /> object to the outgoing side of this
    /// reflector object.
    /// </summary>
    /// <param name="rotor">
    /// The <see cref="IRotor" /> object that is to be connected to the outgoing side of this
    /// reflector object.
    /// </param>
    public void ConnectOutgoing(IRotor rotor);
}