namespace DRSSoftware.EnigmaMachine.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using DRSSoftware.EnigmaMachine.Commands;
using DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// View model used for design-time data binding in the ConfigurationDialogView.
/// </summary>
internal sealed class ConfigurationDialogViewModelDT : ViewModelBase, IConfigurationDialogViewModel
{
    /// <summary>
    /// Gets an ICommand used for accepting the user-specified configuration settings and closing
    /// the associated dialog.
    /// </summary>
    public ICommand AcceptCommand => new RelayCommand(static _ => { }, static _ => true);

    /// <summary>
    /// Gets a list of available rotor index values.
    /// </summary>
    public ReadOnlyCollection<int> AvailableIndexValues
    {
        get;
    } = new List<int>([.. Enumerable.Range(MinValue, MaxValue + 1)]).AsReadOnly();

    /// <summary>
    /// Gets a list of available rotor counts.
    /// </summary>
    public ReadOnlyCollection<int> AvailableRotorCounts
    {
        get;
    } = new List<int>([.. Enumerable.Range(MinRotorCount, MaxRotorCount - MinRotorCount + 1)]).AsReadOnly();

    /// <summary>
    /// Gets an ICommand used for discarding the user-specified configuration settings and closing
    /// the associated dialog.
    /// </summary>
    public ICommand CancelCommand => new RelayCommand(static _ => { }, static _ => true);

    /// <summary>
    /// Gets or sets a value indicating whether the associated view should be closed.
    /// </summary>
    public bool CloseTrigger
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the user-specified Enigma machine configuration.
    /// </summary>
    public EnigmaConfiguration EnigmaConfiguration
    {
        get;
    } = new();

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
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the rotor count will be manually set.
    /// </summary>
    public bool IsManualRotorsSelected
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not the seed value will be manually entered.
    /// </summary>
    public bool IsManualSeedSelected
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #4 is visible.
    /// </summary>
    public bool IsRotor4Visible
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #5 is visible.
    /// </summary>
    public bool IsRotor5Visible
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #6 is visible.
    /// </summary>
    public bool IsRotor6Visible
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #7 is visible.
    /// </summary>
    public bool IsRotor7Visible
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether or not rotor #8 is visible.
    /// </summary>
    public bool IsRotor8Visible
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets the maximum seed value length.
    /// </summary>
    public int MaxSeedValueLength
    {
        get;
    } = MaxSeedLength;

    /// <summary>
    /// Gets the minimum seed value length.
    /// </summary>
    public int MinSeedValueLength
    {
        get;
    } = MinStringLength;

    /// <summary>
    /// Gets or sets the selected index value of the reflector.
    /// </summary>
    public int ReflectorIndex
    {
        get;
        set;
    } = 43;

    /// <summary>
    /// Gets or sets the selected index value of rotor #1.
    /// </summary>
    public int RotorIndex1
    {
        get;
        set;
    } = 13;

    /// <summary>
    /// Gets or sets the selected index value of rotor #2.
    /// </summary>
    public int RotorIndex2
    {
        get;
        set;
    } = 27;

    /// <summary>
    /// Gets or sets the selected index value of rotor #3.
    /// </summary>
    public int RotorIndex3
    {
        get;
        set;
    } = 65;

    /// <summary>
    /// Gets or sets the selected index value of rotor #4.
    /// </summary>
    public int RotorIndex4
    {
        get;
        set;
    } = 91;

    /// <summary>
    /// Gets or sets the selected index value of rotor #5.
    /// </summary>
    public int RotorIndex5
    {
        get;
        set;
    } = 3;

    /// <summary>
    /// Gets or sets the selected index value of rotor #6.
    /// </summary>
    public int RotorIndex6
    {
        get;
        set;
    } = 61;

    /// <summary>
    /// Gets or sets the selected index value of rotor #7.
    /// </summary>
    public int RotorIndex7
    {
        get;
        set;
    } = 12;

    /// <summary>
    /// Gets or sets the selected index value of rotor #8.
    /// </summary>
    public int RotorIndex8
    {
        get;
        set;
    } = 88;

    /// <summary>
    /// Gets or sets the seed value used for randomizing the wiring of the reflector and rotors.
    /// </summary>
    public string SeedValue
    {
        get;
        set;
    } = "Design time seed value.";

    /// <summary>
    /// Gets or sets the currently selected rotor count.
    /// </summary>
    public int SelectedRotorCount
    {
        get;
        set;
    } = MaxRotorCount;

    /// <summary>
    /// Gets or sets a value indicating whether or not the Enigma machine configuration should be
    /// embedded into the encrypted text file.
    /// </summary>
    public bool UseEmbeddedConfiguration
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Initializes the view model with the current Enigma machine configuration.
    /// </summary>
    /// <param name="enigmaConfiguration">
    /// An <see cref="EnigmaConfiguration" /> object containing the current configuration of the
    /// Enigma machine.
    /// </param>
    public void Initialize(EnigmaConfiguration enigmaConfiguration)
    {
    }
}