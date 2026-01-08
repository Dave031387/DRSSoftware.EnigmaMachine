namespace DRSSoftware.EnigmaMachine.Utility;

using DRSSoftware.EnigmaV2;

/// <summary>
/// Provides factory methods for creating and configuring instances of the Enigma machine.
/// </summary>
/// <remarks>
/// This static class is intended to simplify the construction of Enigma machine objects by
/// encapsulating common setup patterns. All members are static and thread-safe.
/// </remarks>
public sealed class EnigmaMachineBuilder : IEnigmaMachineBuilder
{
    /// <summary>
    /// Create a new instance of an Enigma machine with default settings.
    /// </summary>
    /// <returns>
    /// A new <see cref="IEnigmaMachine" /> instance.
    /// </returns>
    public IEnigmaMachine Build() => new EnigmaMachine();

    /// <summary>
    /// Creates a new instance of an Enigma machine with the specified number of rotors.
    /// </summary>
    /// <param name="numberOfRotors">
    /// The number of rotors to be configured into the Enigma machine.
    /// </param>
    /// <returns>
    /// A new <see cref="IEnigmaMachine" /> instance.
    /// </returns>
    public IEnigmaMachine Build(int numberOfRotors) => new EnigmaMachine(numberOfRotors);
}