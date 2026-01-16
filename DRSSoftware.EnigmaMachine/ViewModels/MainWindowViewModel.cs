namespace DRSSoftware.EnigmaMachine.ViewModels;

using System.Windows.Input;
using DRSSoftware.EnigmaMachine.Commands;
using DRSSoftware.EnigmaMachine.Utility;
using DRSSoftware.EnigmaV2;

/// <summary>
/// Represents the view model for the application's main window, providing data binding and command
/// logic for the user interface.
/// </summary>
internal sealed class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
{
    /// <summary>
    /// Holds a reference to the Enigma machine configuration dialog service.
    /// </summary>
    private readonly IConfigurationDialogService _configurationDialogService;

    /// <summary>
    /// Holds a reference to the Enigma machine configuration.
    /// </summary>
    private readonly EnigmaConfiguration _enigmaConfiguration;

    /// <summary>
    /// Holds a reference to the Enigma machine builder.
    /// </summary>
    private readonly IEnigmaMachineBuilder _enigmaMachineBuilder;

    /// <summary>
    /// Holds a reference to the file input/output service.
    /// </summary>
    private readonly IInputOutputService _inputOutputService;

    /// <summary>
    /// Holds a reference to the GetStringService.
    /// </summary>
    private readonly IStringDialogService _stringDialogService;

    /// <summary>
    /// Backing field for the ConfigurationStatus property.
    /// </summary>
    private string _configurationStatus = "Not Configured";

    /// <summary>
    /// Indicates whether or not the output text file has been cloaked.
    /// </summary>
    private bool _fileIsCloaked;

    /// <summary>
    /// Indicates whether or not the input text file has been decloaked.
    /// </summary>
    private bool _fileIsDecloaked;

    /// <summary>
    /// Indicates whether or not the input text file has been loaded.
    /// </summary>
    private bool _fileIsLoaded;

    /// <summary>
    /// Backing field for the InputText property.
    /// </summary>
    private string _inputText = string.Empty;

    /// <summary>
    /// Backing field for the IsConfigured property.
    /// </summary>
    private bool _isConfigured;

    /// <summary>
    /// Backing field for the IsRotor4Visible property.
    /// </summary>
    private bool _isRotor4Visible;

    /// <summary>
    /// Backing field for the IsRotor5Visible property.
    /// </summary>
    private bool _isRotor5Visible;

    /// <summary>
    /// Backing field for the IsRotor6Visible property.
    /// </summary>
    private bool _isRotor6Visible;

    /// <summary>
    /// Backing field for the IsRotor7Visible property.
    /// </summary>
    private bool _isRotor7Visible;

    /// <summary>
    /// Backing field for the IsRotor8Visible property.
    /// </summary>
    private bool _isRotor8Visible;

    /// <summary>
    /// Backing field for the NumberOfRotors property.
    /// </summary>
    private int _numberOfRotors;

    /// <summary>
    /// Backing field for the OutputText property.
    /// </summary>
    private string _outputText = string.Empty;

    /// <summary>
    /// Backing field for the ReflectorIndex property.
    /// </summary>
    private int _reflectorIndex;

    /// <summary>
    /// Backing field for the RotorIndex1 property.
    /// </summary>
    private int _rotorIndex1;

    /// <summary>
    /// Backing field for the RotorIndex2 property.
    /// </summary>
    private int _rotorIndex2;

    /// <summary>
    /// Backing field for the RotorIndex3 property.
    /// </summary>
    private int _rotorIndex3;

    /// <summary>
    /// Backing field for the RotorIndex4 property.
    /// </summary>
    private int _rotorIndex4;

    /// <summary>
    /// Backing field for the RotorIndex5 property.
    /// </summary>
    private int _rotorIndex5;

    /// <summary>
    /// Backing field for the RotorIndex6 property.
    /// </summary>
    private int _rotorIndex6;

    /// <summary>
    /// Backing field for the RotorIndex7 property.
    /// </summary>
    private int _rotorIndex7;

    /// <summary>
    /// Backing field for the RotorIndex8 property.
    /// </summary>
    private int _rotorIndex8;

    /// <summary>
    /// Backing field for the UseEmbeddedConfiguration property.
    /// </summary>
    private bool _useEmbeddedConfiguration;

    /// <summary>
    /// Initializes a new instance of the MainWindowViewModel class with the specified Enigma
    /// machine builder and input/output service.
    /// </summary>
    /// <param name="configurationDialogService">
    /// The service responsible for configuring the Enigma machine.
    /// </param>
    /// <param name="enigmaMachineBuilder">
    /// The builder used to construct the Enigma machine instance for this view model. Cannot be
    /// null.
    /// </param>
    /// <param name="inputOutputService">
    /// The service responsible for handling input and output operations within the view model.
    /// Cannot be null.
    /// </param>
    /// <param name="stringDialogService">
    /// The service used to obtain string input from the user.
    /// </param>
    internal MainWindowViewModel(IConfigurationDialogService configurationDialogService,
                                 IEnigmaMachineBuilder enigmaMachineBuilder,
                                 IInputOutputService inputOutputService,
                                 IStringDialogService stringDialogService)
    {
        // Services
        _configurationDialogService = configurationDialogService;
        _enigmaMachineBuilder = enigmaMachineBuilder;
        _stringDialogService = stringDialogService;
        _inputOutputService = inputOutputService;

        // Commands
        CloakCommand = new RelayCommand(_ => Cloak(), _ => !(string.IsNullOrEmpty(_outputText) || _fileIsCloaked));
        ConfigureCommand = new RelayCommand(_ => Configure(), _ => true);
        DecloakCommand = new RelayCommand(_ => Decloak(), _ => _fileIsLoaded && !_fileIsDecloaked);
        EncryptCommand = new RelayCommand(_ => Encrypt(), _ => !string.IsNullOrWhiteSpace(_inputText));
        LoadCommand = new RelayCommand(_ => LoadFile(), _ => true);
        MoveCommand = new RelayCommand(_ => Move(), _ => !string.IsNullOrEmpty(_outputText));
        ResetCommand = new RelayCommand(_ => Reset(), _ => true);
        SaveCommand = new RelayCommand(_ => { }, _ => !string.IsNullOrEmpty(_outputText));

        // Initialization
        _enigmaConfiguration = new();
        EnigmaMachine = _enigmaMachineBuilder.Build(_enigmaConfiguration);
        InputText = string.Empty;
        OutputText = string.Empty;
        UpdateProperties();
    }

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
        get => _configurationStatus;
        set
        {
            if (_configurationStatus.Equals(value, StringComparison.Ordinal) is false)
            {
                _configurationStatus = value;
                OnPropertyChanged();
            }
        }
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
    /// Gets a reference to the Enigma machine instance.
    /// </summary>
    public IEnigmaMachine EnigmaMachine
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets or sets the input text to be encrypted or decrypted.
    /// </summary>
    public string InputText
    {
        get => _inputText;
        set
        {
            if (_inputText.Equals(value, StringComparison.Ordinal) is false)
            {
                _inputText = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether or not the Enigma machine is fully configured.
    /// </summary>
    public bool IsConfigured
    {
        get => _isConfigured;
        set
        {
            if (_isConfigured != value)
            {
                _isConfigured = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #4 is shown in the UI.
    /// </summary>
    public bool IsRotor4Visible
    {
        get => _isRotor4Visible;
        set
        {
            if (_isRotor4Visible != value)
            {
                _isRotor4Visible = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #5 is shown in the UI.
    /// </summary>
    public bool IsRotor5Visible
    {
        get => _isRotor5Visible;
        set
        {
            if (_isRotor5Visible != value)
            {
                _isRotor5Visible = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #6 is shown in the UI.
    /// </summary>
    public bool IsRotor6Visible
    {
        get => _isRotor6Visible;
        set
        {
            if (_isRotor6Visible != value)
            {
                _isRotor6Visible = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #7 is shown in the UI.
    /// </summary>
    public bool IsRotor7Visible
    {
        get => _isRotor7Visible;
        set
        {
            if (_isRotor7Visible != value)
            {
                _isRotor7Visible = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #8 is shown in the UI.
    /// </summary>
    public bool IsRotor8Visible
    {
        get => _isRotor8Visible;
        set
        {
            if (_isRotor8Visible != value)
            {
                _isRotor8Visible = value;
                OnPropertyChanged();
            }
        }
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
    public int NumberOfRotors
    {
        get => _numberOfRotors;
        set
        {
            if (_numberOfRotors != value)
            {
                _numberOfRotors = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the text that is output after encrypting or decrypting the input text.
    /// </summary>
    public string OutputText
    {
        get => _outputText;
        set
        {
            if (_outputText.Equals(value, StringComparison.Ordinal) is false)
            {
                _outputText = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the reflector index value.
    /// </summary>
    public int ReflectorIndex
    {
        get => _reflectorIndex;
        set
        {
            if (_reflectorIndex != value)
            {
                _reflectorIndex = value;
                OnPropertyChanged();
            }
        }
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
        get => _rotorIndex1;
        set
        {
            if (_rotorIndex1 != value)
            {
                _rotorIndex1 = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #2.
    /// </summary>
    public int RotorIndex2
    {
        get => _rotorIndex2;
        set
        {
            if (_rotorIndex2 != value)
            {
                _rotorIndex2 = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #3.
    /// </summary>
    public int RotorIndex3
    {
        get => _rotorIndex3;
        set
        {
            if (_rotorIndex3 != value)
            {
                _rotorIndex3 = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #4.
    /// </summary>
    public int RotorIndex4
    {
        get => _rotorIndex4;
        set
        {
            if (_rotorIndex4 != value)
            {
                _rotorIndex4 = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #5.
    /// </summary>
    public int RotorIndex5
    {
        get => _rotorIndex5;
        set
        {
            if (_rotorIndex5 != value)
            {
                _rotorIndex5 = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #6.
    /// </summary>
    public int RotorIndex6
    {
        get => _rotorIndex6;
        set
        {
            if (_rotorIndex6 != value)
            {
                _rotorIndex6 = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #7.
    /// </summary>
    public int RotorIndex7
    {
        get => _rotorIndex7;
        set
        {
            if (_rotorIndex7 != value)
            {
                _rotorIndex7 = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #8.
    /// </summary>
    public int RotorIndex8
    {
        get => _rotorIndex8;
        set
        {
            if (_rotorIndex8 != value)
            {
                _rotorIndex8 = value;
                OnPropertyChanged();
            }
        }
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
        get => _useEmbeddedConfiguration;
        set
        {
            if (_useEmbeddedConfiguration != value)
            {
                _useEmbeddedConfiguration = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Applies a cloaking transformation to the output text using a user-provided cloaking string.
    /// </summary>
    private void Cloak()
    {
        string title = "Cloak Text";
        string header = "Enter the text string for cloaking the output text:";
        string cloakText = _stringDialogService.GetString(title, header);

        if (string.IsNullOrWhiteSpace(cloakText))
        {
            return;
        }

        InputText = CloakingService.ApplyCloak(OutputText, cloakText);
        _fileIsCloaked = true;
    }

    /// <summary>
    /// Configure the Enigma machine according to the user's specifications.
    /// </summary>
    private void Configure()
    {
        _enigmaConfiguration.Update(_configurationDialogService.GetConfiguration(_enigmaConfiguration));
        EnigmaMachine = _enigmaMachineBuilder.Build(_enigmaConfiguration);
        UpdateProperties();
    }

    /// <summary>
    /// Removes the cloaking transformation from the input text using a user-provided decloaking
    /// string.
    /// </summary>
    private void Decloak()
    {
        string title = "Decloak Text";
        string header = "Enter the text string for decloaking the input text:";
        string cloakText = _stringDialogService.GetString(title, header);

        if (string.IsNullOrWhiteSpace(cloakText))
        {
            return;
        }

        InputText = CloakingService.RemoveCloak(InputText, cloakText);
        _fileIsDecloaked = true;
    }

    /// <summary>
    /// Encrypts the input text using the configured Enigma machine settings and updates the output
    /// text.
    /// </summary>
    private void Encrypt()
    {
        OutputText = EnigmaMachine.Transform(InputText);

        RotorIndex1 = EnigmaMachine.Rotor1!.CipherIndex;
        RotorIndex2 = EnigmaMachine.Rotor2!.CipherIndex;
        RotorIndex3 = EnigmaMachine.Rotor3!.CipherIndex;
        ReflectorIndex = EnigmaMachine.MyReflector.CipherIndex;

        if (NumberOfRotors > 3)
        {
            RotorIndex4 = EnigmaMachine.Rotor4!.CipherIndex;
        }

        if (NumberOfRotors > 4)
        {
            RotorIndex5 = EnigmaMachine.Rotor5!.CipherIndex;
        }

        if (NumberOfRotors > 5)
        {
            RotorIndex6 = EnigmaMachine.Rotor6!.CipherIndex;
        }

        if (NumberOfRotors > 6)
        {
            RotorIndex7 = EnigmaMachine.Rotor7!.CipherIndex;
        }

        if (NumberOfRotors > 7)
        {
            RotorIndex8 = EnigmaMachine.Rotor8!.CipherIndex;
        }
    }

    /// <summary>
    /// Load the contents of the text file that is to be encrypted or decrypted by the Enigma
    /// machine.
    /// </summary>
    /// <remarks>
    /// The <see cref="InputText" /> property will be set to the contents of the text file, or it
    /// will be left unchanged if the operation is cancelled or an exception is thrown.
    /// </remarks>
    private void LoadFile()
    {
        string inputText = _inputOutputService.LoadTextFile();

        if (string.IsNullOrEmpty(inputText) is false)
        {
            InputText = inputText;
            _fileIsLoaded = true;
            _fileIsDecloaked = false;
        }
    }

    /// <summary>
    /// Moves the output text to the input text box and clears the output text box.
    /// </summary>
    private void Move()
    {
        InputText = OutputText;
        OutputText = string.Empty;
    }

    /// <summary>
    /// Resets the state of the Enigma machine back to its initial state before any text was
    /// encrypted.
    /// </summary>
    private void Reset()
    {
        EnigmaMachine.ResetCipherIndexes();
        UpdateProperties();
    }

    /// <summary>
    /// Update the Enigma properties with the new configuration.
    /// </summary>
    private void UpdateProperties()
    {
        NumberOfRotors = _enigmaConfiguration.NumberOfRotors;
        RotorIndex1 = _enigmaConfiguration.RotorIndex1;
        RotorIndex2 = _enigmaConfiguration.RotorIndex2;
        RotorIndex3 = _enigmaConfiguration.RotorIndex3;
        ReflectorIndex = _enigmaConfiguration.ReflectorIndex;

        if (NumberOfRotors < 4)
        {
            IsRotor4Visible = false;
        }
        else
        {
            IsRotor4Visible = true;
            RotorIndex4 = _enigmaConfiguration.RotorIndex4;
        }

        if (NumberOfRotors < 5)
        {
            IsRotor5Visible = false;
        }
        else
        {
            IsRotor5Visible = true;
            RotorIndex5 = _enigmaConfiguration.RotorIndex5;
        }

        if (NumberOfRotors < 6)
        {
            IsRotor6Visible = false;
        }
        else
        {
            IsRotor6Visible = true;
            RotorIndex6 = _enigmaConfiguration.RotorIndex6;
        }

        if (NumberOfRotors < 7)
        {
            IsRotor7Visible = false;
        }
        else
        {
            IsRotor7Visible = true;
            RotorIndex7 = _enigmaConfiguration.RotorIndex7;
        }

        if (NumberOfRotors < 8)
        {
            IsRotor8Visible = false;
        }
        else
        {
            IsRotor8Visible = true;
            RotorIndex8 = _enigmaConfiguration.RotorIndex8;
        }

        IsConfigured = !string.IsNullOrWhiteSpace(_enigmaConfiguration.SeedValue);
        ConfigurationStatus = IsConfigured ? "Configured" : "Not Configured";
        UseEmbeddedConfiguration = _enigmaConfiguration.UseEmbeddedConfiguration;
    }
}