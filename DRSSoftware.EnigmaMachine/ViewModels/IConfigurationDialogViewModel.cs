namespace DRSSoftware.EnigmaMachine.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// Defines the property and methods for an Enigma machine configuration dialog view model.
/// </summary>
public interface IConfigurationDialogViewModel
{
    /// <summary>
    /// Gets an ICommand used for accepting the user-specified configuration settings and closing
    /// the associated dialog.
    /// </summary>
    public ICommand AcceptCommand
    {
        get;
    }

    /// <summary>
    /// Gets a list of available rotor index values.
    /// </summary>
    public ReadOnlyCollection<int> AvailableIndexValues
    {
        get;
    }

    /// <summary>
    /// Gets a list of available rotor counts.
    /// </summary>
    public ReadOnlyCollection<int> AvailableRotorCounts
    {
        get;
    }

    /// <summary>
    /// Gets an ICommand used for discarding the user-specified configuration settings and closing
    /// the associated dialog.
    /// </summary>
    public ICommand CancelCommand
    {
        get;
    }

    /// <summary>
    /// Gets or sets a value used for indicating whether the associated view should be closed.
    /// </summary>
    public bool CloseTrigger
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the user-specified Enigma machine configuration.
    /// </summary>
    public EnigmaConfiguration EnigmaConfiguration
    {
        get;
    }

    /// <summary>
    /// Gets or sets a value indicating whether automatic rotor indexes selection is enabled.
    /// </summary>
    public bool IsAutoIndexesSelected
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether automatic rotor count selection is enabled.
    /// </summary>
    public bool IsAutoRotorsSelected
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the seed value is automatically generated.
    /// </summary>
    public bool IsAutoSeedSelected
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether rotor indexes will be manually set.
    /// </summary>
    public bool IsManualIndexesSelected
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the rotor count will be manually set.
    /// </summary>
    public bool IsManualRotorsSelected
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the seed value will be manually entered.
    /// </summary>
    public bool IsManualSeedSelected
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #4 is visible.
    /// </summary>
    public bool IsRotor4Visible
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #5 is visible.
    /// </summary>
    public bool IsRotor5Visible
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #6 is visible.
    /// </summary>
    public bool IsRotor6Visible
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #7 is visible.
    /// </summary>
    public bool IsRotor7Visible
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #8 is visible.
    /// </summary>
    public bool IsRotor8Visible
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the maximum seed value length.
    /// </summary>
    int MaxSeedValueLength
    {
        get;
    }

    /// <summary>
    /// Gets the minimum seed value length.
    /// </summary>
    int MinSeedValueLength
    {
        get;
    }

    /// <summary>
    /// Gets or sets the selected index value of the reflector.
    /// </summary>
    public int ReflectorIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #1.
    /// </summary>
    public int RotorIndex1
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #2.
    /// </summary>
    public int RotorIndex2
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #3.
    /// </summary>
    public int RotorIndex3
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #4.
    /// </summary>
    public int RotorIndex4
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #5.
    /// </summary>
    public int RotorIndex5
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #6.
    /// </summary>
    public int RotorIndex6
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #7.
    /// </summary>
    public int RotorIndex7
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the selected index value of rotor #8.
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
    }

    /// <summary>
    /// Gets or sets the currently selected rotor count.
    /// </summary>
    public int SelectedRotorCount
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the Enigma machine configuration should be
    /// embedded into the encrypted text file.
    /// </summary>
    public bool UseEmbeddedConfiguration
    {
        get;
        set;
    }

    /// <summary>
    /// Initializes the view model with the current Enigma machine configuration.
    /// </summary>
    /// <param name="enigmaConfiguration">
    /// An <see cref="EnigmaConfiguration" /> object containing the current configuration of the
    /// Enigma machine.
    /// </param>
    public void Initialize(EnigmaConfiguration enigmaConfiguration);
}