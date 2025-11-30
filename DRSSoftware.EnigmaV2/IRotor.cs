namespace DRSSoftware.EnigmaV2;

/// <summary>
/// This interface defines the contract for the <see cref="Rotor" /> component of the
/// <see cref="EnigmaMachine" />.
/// </summary>
public interface IRotor : ICipherWheel
{
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
    }

    /// <summary>
    /// Connect the specified <paramref name="cipherWheel" /> object (either <see cref="Rotor" /> or
    /// <see cref="Reflector" />) to the left side of this <see cref="Rotor" /> object.
    /// </summary>
    /// <remarks>
    /// The left side of the last <see cref="Rotor" /> in sequence will be connected to a
    /// <see cref="Reflector" /> object. All other rotors will be connected to the next
    /// <see cref="Rotor" /> in sequence.
    /// </remarks>
    /// <param name="cipherWheel">
    /// The <see cref="CipherWheel" /> object ( <see cref="Rotor" /> or <see cref="Reflector" />)
    /// that is to be connected to the left side of this <see cref="Rotor" /> object.
    /// </param>
    public void ConnectLeftComponent(ICipherWheel cipherWheel);

    /// <summary>
    /// Connect the specified <paramref name="rotor" /> object to the right side of this
    /// <see cref="Rotor" /> object.
    /// </summary>
    /// <remarks>
    /// This property is <see langword="null" /> for the first <see cref="Rotor" /> in sequence
    /// since that rotor doesn't have any <see cref="Rotor" /> object attached to its right side.
    /// </remarks>
    /// <param name="rotor">
    /// The <see cref="Rotor" /> object that is to be connected to the right side of this
    /// <see cref="Rotor" /> object.
    /// </param>
    public void ConnectRightComponent(IRotor rotor);
}