namespace DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// This class holds the configuration settings for the Enigma machine.
/// </summary>
public sealed class EnigmaConfiguration
{
    /// <summary>
    /// Gets or sets the number of rotors to be used in the Enigma machine.
    /// </summary>
    public int NumberOfRotors
    {
        get;
        set;
    } = MinRotorCount;

    /// <summary>
    /// Gets or sets the reflector index value.
    /// </summary>
    public int ReflectorIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #1.
    /// </summary>
    public int RotorIndex1
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #2.
    /// </summary>
    public int RotorIndex2
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #3.
    /// </summary>
    public int RotorIndex3
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #4.
    /// </summary>
    public int RotorIndex4
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #5.
    /// </summary>
    public int RotorIndex5
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #6.
    /// </summary>
    public int RotorIndex6
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #7.
    /// </summary>
    public int RotorIndex7
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #8.
    /// </summary>
    public int RotorIndex8
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the seed value used for randomizing the wiring of the reflector and rotors.
    /// </summary>
    public string SeedValue
    {
        get;
        set;
    } = string.Empty;

    /// <summary>
    /// Gets or sets a value that determines whether or not the Enigma configuration will be
    /// embedded into the encrypted text file.
    /// </summary>
    public bool UseEmbeddedConfiguration
    {
        get;
        set;
    }

    /// <summary>
    /// Updates the current configuration with values from another configuration.
    /// </summary>
    /// <param name="newConfig">
    /// The new configuration whose values will be used to update the current configuration.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the new configuration passed into the method is null.
    /// </exception>
    public void Update(EnigmaConfiguration newConfig)
    {
        ArgumentNullException.ThrowIfNull(newConfig);

        NumberOfRotors = newConfig.NumberOfRotors;
        ReflectorIndex = newConfig.ReflectorIndex;
        RotorIndex1 = newConfig.RotorIndex1;
        RotorIndex2 = newConfig.RotorIndex2;
        RotorIndex3 = newConfig.RotorIndex3;
        RotorIndex4 = newConfig.RotorIndex4;
        RotorIndex5 = newConfig.RotorIndex5;
        RotorIndex6 = newConfig.RotorIndex6;
        RotorIndex7 = newConfig.RotorIndex7;
        RotorIndex8 = newConfig.RotorIndex8;
        SeedValue = newConfig.SeedValue;
        UseEmbeddedConfiguration = newConfig.UseEmbeddedConfiguration;
    }
}