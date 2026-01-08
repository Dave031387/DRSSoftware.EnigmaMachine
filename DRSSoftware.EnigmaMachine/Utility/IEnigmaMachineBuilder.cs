namespace DRSSoftware.EnigmaMachine.Utility;

using DRSSoftware.EnigmaV2;

/// <summary>
/// Defines methods for building and configuring instances of the Enigma machine.
/// </summary>
public interface IEnigmaMachineBuilder
{
    /// <summary>
    /// Create a new instance of an Enigma machine with default settings.
    /// </summary>
    /// <returns>
    /// A new <see cref="IEnigmaMachine" /> instance.
    /// </returns>
    public IEnigmaMachine Build();

    /// <summary>
    /// Creates a new instance of an Enigma machine with the specified number of rotors.
    /// </summary>
    /// <param name="numberOfRotors">
    /// The number of rotors to be configured into the Enigma machine.
    /// </param>
    /// <returns>
    /// A new <see cref="IEnigmaMachine" /> instance.
    /// </returns>
    public IEnigmaMachine Build(int numberOfRotors);
}