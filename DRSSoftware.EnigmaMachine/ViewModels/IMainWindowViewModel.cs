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
    /// Gets the command used for saving the output text from the Enigma machine.
    /// </summary>
    public ICommand SaveCommand
    {
        get;
    }
}