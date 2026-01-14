namespace DRSSoftware.EnigmaMachine.ViewModels;

using System.Windows.Input;
using DRSSoftware.DRSBasicDI.Interfaces;
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
    /// Holds a reference to the Enigma machine builder.
    /// </summary>
    private readonly IEnigmaMachineBuilder _enigmaMachineBuilder;

    /// <summary>
    /// Holds a reference to the dependency injection container.
    /// </summary>
    private readonly IContainer _container;

    /// <summary>
    /// Holds a reference to the GetStringService.
    /// </summary>
    private readonly IStringDialogService _stringDialogService;

    /// <summary>
    /// Holds a reference to the file input/output service.
    /// </summary>
    private readonly IInputOutputService _inputOutputService;

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
    /// Backing field for the OutputText property.
    /// </summary>
    private string _outputText = string.Empty;

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
    /// <param name="container">
    /// A reference to the dependency injection container.
    /// </param>
    internal MainWindowViewModel(IConfigurationDialogService configurationDialogService,
                                 IEnigmaMachineBuilder enigmaMachineBuilder,
                                 IInputOutputService inputOutputService,
                                 IStringDialogService stringDialogService,
                                 IContainer container)
    {
        _container = container;
        _configurationDialogService = configurationDialogService;
        _enigmaMachineBuilder = enigmaMachineBuilder;
        _stringDialogService = stringDialogService;
        _inputOutputService = inputOutputService;
        EnigmaMachine = _enigmaMachineBuilder.Build();

        CloakCommand = new RelayCommand(_ => Cloak(), _ => !(string.IsNullOrEmpty(_outputText) || _fileIsCloaked));
        ConfigureCommand = new RelayCommand(_ => { }, _ => true);
        DecloakCommand = new RelayCommand(_ => Decloak(), _ => _fileIsLoaded && !_fileIsDecloaked);
        EncryptCommand = new RelayCommand(_ => { }, _ => _fileIsLoaded);
        LoadCommand = new RelayCommand(_ => LoadFile(), _ => true);
        ResetCommand = new RelayCommand(_ => { }, _ => true);
        SaveCommand = new RelayCommand(_ => { }, _ => !string.IsNullOrEmpty(_outputText));

        InputText = string.Empty;
        OutputText = string.Empty;
    }

    /// <summary>
    /// Gets the command used for cloaking the output text in the Enigma machine.
    /// </summary>
    public ICommand CloakCommand
    {
        get;
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
    /// Gets the command used for loading the input text into the Enigma machine.
    /// </summary>
    public ICommand LoadCommand
    {
        get;
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
    /// Gets the command used for resetting the state of the Enigma machine.
    /// </summary>
    public ICommand ResetCommand
    {
        get;
    }

    /// <summary>
    /// Gets the command used for saving the output text from the Enigma machine.
    /// </summary>
    public ICommand SaveCommand
    {
        get;
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
}