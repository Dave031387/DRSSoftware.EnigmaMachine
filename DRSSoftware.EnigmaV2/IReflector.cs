namespace DRSSoftware.EnigmaV2;

/// <summary>
/// This interface defines the contract for the <see cref="Reflector" /> component of the
/// <see cref="EnigmaMachine" />.
/// </summary>
public interface IReflector : ICipherWheel
{
    /// <summary>
    /// Connect the specified <paramref name="rotor" /> object to the right side of this
    /// <see cref="Reflector" /> object.
    /// </summary>
    /// <param name="rotor">
    /// The <see cref="Rotor" /> object that is to be connected to the right side of this
    /// <see cref="Reflector" /> object.
    /// </param>
    public void ConnectRightComponent(IRotor rotor);
}