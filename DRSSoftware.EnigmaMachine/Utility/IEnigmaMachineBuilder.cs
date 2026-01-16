namespace DRSSoftware.EnigmaMachine.Utility;

using DRSSoftware.EnigmaV2;

/// <summary>
/// Defines methods for building and configuring instances of the Enigma machine.
/// </summary>
public interface IEnigmaMachineBuilder
{
    /// <summary>
    /// Creates and configures a new Enigma machine instance based on the specified settings.
    /// </summary>
    /// <param name="configuration">
    /// The configuration parameters that define the initial state and wiring of the Enigma machine.
    /// </param>
    /// <returns>
    /// An instance of <see cref="IEnigmaMachine" /> initialized according to the provided
    /// configuration.
    /// </returns>
    public IEnigmaMachine Build(EnigmaConfiguration configuration);
}