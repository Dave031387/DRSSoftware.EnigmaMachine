namespace DRSSoftware.EnigmaMachine.ViewModels;

using System.Windows.Input;

/// <summary>
/// Defines the properties and methods required for operating the Enigma machine.
/// </summary>
internal interface IMainWindowViewModel
{
    /// <summary>
    /// Gets the command used for cloaking the output text in the Enigma machine.
    /// </summary>
    public ICommand CloakCommand
    {
        get;
    }

    /// <summary>
    /// Gets or sets the configuration status text.
    /// </summary>
    public string ConfigurationStatus
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the command used for reconfiguring the Enigma machine.
    /// </summary>
    public ICommand ConfigureCommand
    {
        get;
    }

    /// <summary>
    /// Gets the command used for de-cloaking the input text in the Enigma machine.
    /// </summary>
    public ICommand DecloakCommand
    {
        get;
    }

    /// <summary>
    /// Gets the command used for encrypting the input text to product the output text.
    /// </summary>
    public ICommand EncryptCommand
    {
        get;
    }

    /// <summary>
    /// Gets or sets the input text to be encrypted or decrypted.
    /// </summary>
    public string InputText
    {
        get; set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the Enigma machine is fully configured.
    /// </summary>
    public bool IsConfigured
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #4 is shown in the UI.
    /// </summary>
    public bool IsRotor4Visible
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #5 is shown in the UI.
    /// </summary>
    public bool IsRotor5Visible
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #6 is shown in the UI.
    /// </summary>
    public bool IsRotor6Visible
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #7 is shown in the UI.
    /// </summary>
    public bool IsRotor7Visible
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #8 is shown in the UI.
    /// </summary>
    public bool IsRotor8Visible
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the command used for loading the input text into the Enigma machine.
    /// </summary>
    public ICommand LoadCommand
    {
        get;
    }

    /// <summary>
    /// Gets the command used for moving the output text to the input text box.
    /// </summary>
    public ICommand MoveCommand
    {
        get;
    }

    /// <summary>
    /// Gets or sets the number of rotors in the Enigma machine.
    /// </summary>
    int NumberOfRotors
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the text that is output after encrypting or decrypting the input text.
    /// </summary>
    public string OutputText
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the reflector index value.
    /// </summary>
    public int ReflectorIndex
    {
        get;
        set;
    }

    /// <summary>
    /// Gets the command used for resetting the state of the Enigma machine.
    /// </summary>
    public ICommand ResetCommand
    {
        get;
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
    /// Gets the command used for saving the output text from the Enigma machine.
    /// </summary>
    public ICommand SaveCommand
    {
        get;
    }

    /// <summary>
    /// Gets or sets a value that determines whether or not the Enigma configuration will be
    /// embedded into the encrypted text file.
    /// </summary>
    public bool UseEmbeddedConfiguration
    {
        get;
        set;
    }
}