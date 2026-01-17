namespace DRSSoftware.EnigmaMachine.ViewModels;

using System.Windows.Input;
using DRSSoftware.EnigmaMachine.Commands;

/// <summary>
/// View model used for design-time data binding in the MainWindow view.
/// </summary>
public class MainWindowViewModelDT : ViewModelBase, IMainWindowViewModel
{
    /// <summary>
    /// Gets the command used for cloaking the output text in the Enigma machine.
    /// </summary>
    public ICommand CloakCommand
    {
        get;
    } = new RelayCommand(static _ => { }, static _ => true);

    /// <summary>
    /// Gets or sets the configuration status text.
    /// </summary>
    public string ConfigurationStatus
    {
        get;
        set;
    } = "Configured";

    /// <summary>
    /// Gets the command used for reconfiguring the Enigma machine.
    /// </summary>
    public ICommand ConfigureCommand
    {
        get;
    } = new RelayCommand(static _ => { }, static _ => true);

    /// <summary>
    /// Gets the command used for de-cloaking the input text in the Enigma machine.
    /// </summary>
    public ICommand DecloakCommand
    {
        get;
    } = new RelayCommand(static _ => { }, static _ => true);

    /// <summary>
    /// Gets or sets the sample input text to be displayed during design time.
    /// </summary>
    public string InputText
    {
        get;
        set;
    } = "Sample input text for design time. Some more text to make it longer so that the scroll bar shows up.";

    /// <summary>
    /// Gets or sets a value indicating whether or not the Enigma machine is fully configured.
    /// </summary>
    public bool IsConfigured
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #4 is shown in the UI.
    /// </summary>
    public bool IsRotor4Visible
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #5 is shown in the UI.
    /// </summary>
    public bool IsRotor5Visible
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #6 is shown in the UI.
    /// </summary>
    public bool IsRotor6Visible
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #7 is shown in the UI.
    /// </summary>
    public bool IsRotor7Visible
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #8 is shown in the UI.
    /// </summary>
    public bool IsRotor8Visible
    {
        get;
        set;
    } = true;

    /// <summary>
    /// Gets the command used for loading the input text into the Enigma machine.
    /// </summary>
    public ICommand LoadCommand
    {
        get;
    } = new RelayCommand(static _ => { }, static _ => true);

    /// <summary>
    /// Gets the command used for moving the output text to the input text box.
    /// </summary>
    public ICommand MoveCommand
    {
        get;
    } = new RelayCommand(static _ => { }, static _ => true);

    /// <summary>
    /// Gets or sets the number of rotors in the Enigma machine.
    /// </summary>
    public int NumberOfRotors
    {
        get;
        set;
    } = 8;

    /// <summary>
    /// Gets or sets the sample output text to be displayed during design time.
    /// </summary>
    public string OutputText
    {
        get;
        set;
    } = "Sample output text for design time. Some more text to make it longer so that the scroll bar shows up.";

    /// <summary>
    /// Gets or sets the reflector index value.
    /// </summary>
    public int ReflectorIndex
    {
        get;
        set;
    } = 23;

    /// <summary>
    /// Gets the command used for resetting the state of the Enigma machine.
    /// </summary>
    public ICommand ResetCommand
    {
        get;
    } = new RelayCommand(static _ => { }, static _ => true);

    /// <summary>
    /// Gets or sets the index value for Rotor #1.
    /// </summary>
    public int RotorIndex1
    {
        get;
        set;
    } = 23;

    /// <summary>
    /// Gets or sets the index value for Rotor #2.
    /// </summary>
    public int RotorIndex2
    {
        get;
        set;
    } = 2;

    /// <summary>
    /// Gets or sets the index value for Rotor #3.
    /// </summary>
    public int RotorIndex3
    {
        get;
        set;
    } = 51;

    /// <summary>
    /// Gets or sets the index value for Rotor #4.
    /// </summary>
    public int RotorIndex4
    {
        get;
        set;
    } = 42;

    /// <summary>
    /// Gets or sets the index value for Rotor #5.
    /// </summary>
    public int RotorIndex5
    {
        get;
        set;
    } = 95;

    /// <summary>
    /// Gets or sets the index value for Rotor #6.
    /// </summary>
    public int RotorIndex6
    {
        get;
        set;
    } = 27;

    /// <summary>
    /// Gets or sets the index value for Rotor #7.
    /// </summary>
    public int RotorIndex7
    {
        get;
        set;
    } = 4;

    /// <summary>
    /// Gets or sets the index value for Rotor #8.
    /// </summary>
    public int RotorIndex8
    {
        get;
        set;
    } = 88;

    /// <summary>
    /// Gets the command used for saving the output text from the Enigma machine.
    /// </summary>
    public ICommand SaveCommand
    {
        get;
    } = new RelayCommand(static _ => { }, static _ => true);

    /// <summary>
    /// Gets the command used for applying the Enigma transform on the input text to product the
    /// output text.
    /// </summary>
    public ICommand TransformCommand
    {
        get;
    } = new RelayCommand(static _ => { }, static _ => true);

    /// <summary>
    /// Gets or sets a value that determines whether or not the Enigma configuration will be
    /// embedded into the encrypted text file.
    /// </summary>
    public bool UseEmbeddedConfiguration
    {
        get;
        set;
    } = true;
}