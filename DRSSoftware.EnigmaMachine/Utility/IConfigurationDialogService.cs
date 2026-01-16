namespace DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// Defines the methods for configuring the Enigma machine settings.
/// </summary>
public interface IConfigurationDialogService
{
    /// <summary>
    /// Obtains a new Enigma machine configuration from the user.
    /// </summary>
    /// <param name="currentConfiguration">
    /// The current Enigma machine configuration. Used as a starting point for the new
    /// configuration.
    /// </param>
    /// <returns>
    /// The user-specified configuration of the Enigma machine.
    /// </returns>
    public EnigmaConfiguration GetConfiguration(EnigmaConfiguration currentConfiguration);
}