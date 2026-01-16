namespace DRSSoftware.EnigmaMachine.Utility;

using DRSSoftware.EnigmaV2;

/// <summary>
/// Provides factory methods for creating and configuring instances of the Enigma machine.
/// </summary>
/// <remarks>
/// This static class is intended to simplify the construction of Enigma machine objects by
/// encapsulating common setup patterns. All members are static and thread-safe.
/// </remarks>
internal sealed class EnigmaMachineBuilder : IEnigmaMachineBuilder
{
    /// <summary>
    /// Creates and configures a new Enigma machine instance based on the specified settings.
    /// </summary>
    /// <remarks>
    /// The reflector and rotor index values are ignored if a seed value isn't specified.
    /// </remarks>
    /// <param name="configuration">
    /// The configuration parameters that define the initial state and wiring of the Enigma machine.
    /// </param>
    /// <returns>
    /// An instance of <see cref="IEnigmaMachine" /> initialized according to the provided
    /// configuration.
    /// </returns>
    public IEnigmaMachine Build(EnigmaConfiguration configuration)
    {
        EnigmaMachine enigmaMachine = new(configuration.NumberOfRotors);

        if (string.IsNullOrWhiteSpace(configuration.SeedValue))
        {
            return enigmaMachine;
        }

        enigmaMachine.Initialize(configuration.SeedValue);

        int[] cipherIndexes = new int[configuration.NumberOfRotors + 1];
        int[] newIndexes =
        [
            configuration.RotorIndex1,
            configuration.RotorIndex2,
            configuration.RotorIndex3,
            configuration.RotorIndex4,
            configuration.RotorIndex5,
            configuration.RotorIndex6,
            configuration.RotorIndex7,
            configuration.RotorIndex8
        ];
        bool mustSetIndexes = false;

        for (int i = 0; i < configuration.NumberOfRotors; i++)
        {
            cipherIndexes[i] = newIndexes[i];
            mustSetIndexes |= newIndexes[i] != 0;
        }

        cipherIndexes[^1] = configuration.ReflectorIndex;
        mustSetIndexes |= configuration.ReflectorIndex != 0;

        if (mustSetIndexes)
        {
            enigmaMachine.SetCipherIndexes(cipherIndexes);
        }

        return enigmaMachine;
    }
}