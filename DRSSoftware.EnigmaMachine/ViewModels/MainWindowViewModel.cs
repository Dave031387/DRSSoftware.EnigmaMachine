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
    /// Holds a reference to the service used for cloaking and decloaking text.
    /// </summary>
    private readonly ICloakingService _cloakingService;

    /// <summary>
    /// Holds a reference to the Enigma machine configuration dialog service.
    /// </summary>
    private readonly IConfigurationDialogService _configurationDialogService;

    /// <summary>
    /// Holds a reference to the service used for embedding the Enigma configuration into the
    /// encrypted text file.
    /// </summary>
    private readonly IEmbeddingService _embeddingService;

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
    /// Initializes a new instance of the MainWindowViewModel class with the specified Enigma
    /// machine builder and input/output service.
    /// </summary>
    /// <param name="cloakingService">
    /// The service used for cloaking and decloaking text.
    /// </param>
    /// <param name="configurationDialogService">
    /// The service responsible for configuring the Enigma machine.
    /// </param>
    /// <param name="embeddingService">
    /// The service responsible for embedding the Enigma machine configuration into the encrypted
    /// text file.
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
    internal MainWindowViewModel(ICloakingService cloakingService,
                                 IConfigurationDialogService configurationDialogService,
                                 IEmbeddingService embeddingService,
                                 IEnigmaMachineBuilder enigmaMachineBuilder,
                                 IInputOutputService inputOutputService,
                                 IStringDialogService stringDialogService)
    {
        // Services
        _cloakingService = cloakingService;
        _configurationDialogService = configurationDialogService;
        _embeddingService = embeddingService;
        _enigmaMachineBuilder = enigmaMachineBuilder;
        _inputOutputService = inputOutputService;
        _stringDialogService = stringDialogService;

        // Commands
        CloakCommand = new RelayCommand(_ => Cloak(), _ => CanCloak);
        ConfigureCommand = new RelayCommand(_ => Configure(), _ => true);
        DecloakCommand = new RelayCommand(_ => Decloak(), _ => CanDecloak);
        LoadCommand = new RelayCommand(_ => LoadFile(), _ => true);
        MoveCommand = new RelayCommand(_ => Move(), _ => OutputTextIsAvailable);
        ResetCommand = new RelayCommand(_ => Reset(), _ => IsTransformExecuted);
        SaveCommand = new RelayCommand(_ => SaveFile(), _ => OutputTextIsAvailable);
        TransformCommand = new RelayCommand(_ => Transform(), _ => CanTransform);

        // Initialization
        _enigmaConfiguration = new();
        EnigmaMachine = _enigmaMachineBuilder.Build(_enigmaConfiguration);
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
    public string ConfigurationStatusText
    {
        get;
        set
        {
            if (field.Equals(value, StringComparison.Ordinal) is false)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = NotConfiguredStatusText;

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
        get;
        set
        {
            if (field.Equals(value, StringComparison.Ordinal) is false)
            {
                field = value;
                IsInputTextCloaked = _cloakingService.HasIndicatorString(field);
                OnPropertyChanged();
            }
        }
    } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether or not the Enigma machine is fully configured.
    /// </summary>
    public bool IsConfigured
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #4 is shown in the UI.
    /// </summary>
    public bool IsRotor4Visible
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #5 is shown in the UI.
    /// </summary>
    public bool IsRotor5Visible
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #6 is shown in the UI.
    /// </summary>
    public bool IsRotor6Visible
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #7 is shown in the UI.
    /// </summary>
    public bool IsRotor7Visible
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether Rotor #8 is shown in the UI.
    /// </summary>
    public bool IsRotor8Visible
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
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
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the text that is output after encrypting or decrypting the input text.
    /// </summary>
    public string OutputText
    {
        get;
        set
        {
            if (field.Equals(value, StringComparison.Ordinal) is false)
            {
                field = value;
                OutputTextIsCloaked = _cloakingService.HasIndicatorString(field);
                OnPropertyChanged();
            }
        }
    } = string.Empty;

    /// <summary>
    /// Gets or sets the reflector index value.
    /// </summary>
    public int ReflectorIndex
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
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
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #2.
    /// </summary>
    public int RotorIndex2
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #3.
    /// </summary>
    public int RotorIndex3
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #4.
    /// </summary>
    public int RotorIndex4
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #5.
    /// </summary>
    public int RotorIndex5
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #6.
    /// </summary>
    public int RotorIndex6
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #7.
    /// </summary>
    public int RotorIndex7
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the index value for Rotor #8.
    /// </summary>
    public int RotorIndex8
    {
        get;
        set
        {
            if (field != value)
            {
                field = value;
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
    /// Gets the command used for applying the Enigma transform on the input text to product the
    /// output text.
    /// </summary>
    public ICommand TransformCommand
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
        set
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets a value that indicates whether or not the Cloak command can be executed.
    /// </summary>
    /// <returns>
    /// A value indicating whether or not the Cloak command can be executed.
    /// </returns>
    private bool CanCloak => OutputTextIsAvailable && !OutputTextIsCloaked;

    /// <summary>
    /// Gets a value that indicates whether or not the Decloak command can be executed.
    /// </summary>
    /// <returns>
    /// A value indicating whether or not the Decloak command can be executed.
    /// </returns>
    private bool CanDecloak => IsInputTextAvailable && IsInputTextCloaked;

    /// <summary>
    /// Gets a value that indicates whether or not the Transform command can be executed.
    /// </summary>
    /// <returns>
    /// A value indicating whether or not the Transform command can be executed.
    /// </returns>
    private bool CanTransform => IsConfigured && IsInputTextAvailable && !IsTransformExecuted && !CanDecloak;

    /// <summary>
    /// Gets a value indicating whether or not the input text is available for processing.
    /// </summary>
    private bool IsInputTextAvailable => !string.IsNullOrWhiteSpace(InputText);

    /// <summary>
    /// Gets or sets a value indicating whether or not the input text is cloaked.
    /// </summary>
    private bool IsInputTextCloaked
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the Transform command has been executed since the
    /// last execution of either the Configure or Reset commands.
    /// </summary>
    private bool IsTransformExecuted
    {
        get;
        set;
    }

    /// <summary>
    /// Gets a value indicating whether or not the output text is available for processing.
    /// </summary>
    private bool OutputTextIsAvailable => !string.IsNullOrWhiteSpace(OutputText);

    /// <summary>
    /// Gets or sets a value indicating whether or not the output text has been cloaked.
    /// </summary>
    private bool OutputTextIsCloaked
    {
        get;
        set;
    }

    /// <summary>
    /// Applies a cloaking transformation to the output text using a user-provided cloaking string.
    /// </summary>
    private void Cloak()
    {
        string title = CloakingStringDialogTitle;
        string header = CloakingStringDialogHeader;
        string cloakText = _stringDialogService.GetString(title, header);

        if (string.IsNullOrWhiteSpace(cloakText))
        {
            return;
        }

        OutputText = _cloakingService.ApplyCloak(OutputText, cloakText);
    }

    /// <summary>
    /// Configure the Enigma machine according to the user's specifications.
    /// </summary>
    private void Configure()
    {
        _enigmaConfiguration.Update(_configurationDialogService.GetConfiguration(_enigmaConfiguration));
        EnigmaMachine = _enigmaMachineBuilder.Build(_enigmaConfiguration);
        UpdateProperties();
        IsTransformExecuted = false;
    }

    /// <summary>
    /// Removes the cloaking transformation from the input text using a user-provided decloaking
    /// string.
    /// </summary>
    private void Decloak()
    {
        string title = DecloakingStringDialogTitle;
        string header = DecloakingStringDialogHeader;
        string cloakText = _stringDialogService.GetString(title, header);

        if (string.IsNullOrWhiteSpace(cloakText))
        {
            return;
        }

        LoadInputText(_cloakingService.RemoveCloak(InputText, cloakText));
    }

    /// <summary>
    /// Load the contents of the text file that is to be encrypted or decrypted by the Enigma
    /// machine.
    /// </summary>
    /// <remarks>
    /// The <see cref="InputText" /> property will be set to the contents of the text file, or it
    /// will be left unchanged if the operation is cancelled or an exception is thrown.
    /// <para>
    /// If the input text contains configuration data, then that data will be extracted and the
    /// Enigma machine configured accordingly.
    /// </para>
    /// </remarks>
    private void LoadFile() => LoadInputText(_inputOutputService.LoadTextFile());

    /// <summary>
    /// Load the given text string into the InputText property. Embedded configuration, if present,
    /// will automatically be extracted and used to configure the Enigma machine.
    /// </summary>
    /// <param name="inputText">
    /// The text to be loaded into the InputText property.
    /// </param>
    private void LoadInputText(string inputText)
    {
        if (!string.IsNullOrEmpty(inputText))
        {
            if (_embeddingService.HasIndicatorString(inputText))
            {
                InputText = _embeddingService.Extract(inputText, out EnigmaConfiguration? configuration);

                if (configuration is not null)
                {
                    _enigmaConfiguration.Update(configuration);
                    EnigmaMachine = _enigmaMachineBuilder.Build(_enigmaConfiguration);
                    UpdateProperties();
                    IsTransformExecuted = false;
                }
                else
                {
                    Reset();
                }
            }
            else
            {
                InputText = inputText;
                Reset();
            }

            OutputText = string.Empty;
        }
    }

    /// <summary>
    /// Moves the output text to the input text box and clears the output text box.
    /// </summary>
    /// <remarks>
    /// This method also calls the Reset method to reset the state of the Enigma machine after
    /// moving the text.
    /// </remarks>
    private void Move() => LoadInputText(OutputText);

    /// <summary>
    /// Resets the state of the Enigma machine back to its initial state before any text was
    /// encrypted.
    /// </summary>
    private void Reset()
    {
        EnigmaMachine.ResetCipherIndexes();
        UpdateProperties();
        IsTransformExecuted = false;
    }

    /// <summary>
    /// Saves the output text to a text file.
    /// </summary>
    private void SaveFile() => _inputOutputService.SaveTextFile(OutputText);

    /// <summary>
    /// Encrypts the input text using the configured Enigma machine settings and updates the output
    /// text.
    /// </summary>
    private void Transform()
    {
        string transformedText = EnigmaMachine.Transform(InputText);

        if (UseEmbeddedConfiguration)
        {
            OutputText = _embeddingService.Embed(transformedText, _enigmaConfiguration);
        }
        else
        {
            OutputText = transformedText;
        }

        RotorIndex1 = EnigmaMachine.Rotor1!.CipherIndex;
        RotorIndex2 = EnigmaMachine.Rotor2!.CipherIndex;
        RotorIndex3 = EnigmaMachine.Rotor3!.CipherIndex;

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

        ReflectorIndex = EnigmaMachine.Reflector.CipherIndex;
        IsTransformExecuted = true;
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
            RotorIndex4 = 0;
            _enigmaConfiguration.RotorIndex4 = 0;
        }
        else
        {
            IsRotor4Visible = true;
            RotorIndex4 = _enigmaConfiguration.RotorIndex4;
        }

        if (NumberOfRotors < 5)
        {
            IsRotor5Visible = false;
            RotorIndex5 = 0;
            _enigmaConfiguration.RotorIndex5 = 0;
        }
        else
        {
            IsRotor5Visible = true;
            RotorIndex5 = _enigmaConfiguration.RotorIndex5;
        }

        if (NumberOfRotors < 6)
        {
            IsRotor6Visible = false;
            RotorIndex6 = 0;
            _enigmaConfiguration.RotorIndex6 = 0;
        }
        else
        {
            IsRotor6Visible = true;
            RotorIndex6 = _enigmaConfiguration.RotorIndex6;
        }

        if (NumberOfRotors < 7)
        {
            IsRotor7Visible = false;
            RotorIndex7 = 0;
            _enigmaConfiguration.RotorIndex7 = 0;
        }
        else
        {
            IsRotor7Visible = true;
            RotorIndex7 = _enigmaConfiguration.RotorIndex7;
        }

        if (NumberOfRotors < 8)
        {
            IsRotor8Visible = false;
            RotorIndex8 = 0;
            _enigmaConfiguration.RotorIndex8 = 0;
        }
        else
        {
            IsRotor8Visible = true;
            RotorIndex8 = _enigmaConfiguration.RotorIndex8;
        }

        IsConfigured = !string.IsNullOrEmpty(_enigmaConfiguration.SeedValue);
        ConfigurationStatusText = IsConfigured ? ConfiguredStatusText : NotConfiguredStatusText;
        UseEmbeddedConfiguration = _enigmaConfiguration.UseEmbeddedConfiguration;
    }
}