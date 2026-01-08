namespace DRSSoftware.EnigmaMachine.ViewModels;

using System.Windows.Input;
using DRSSoftware.EnigmaMachine.Commands;

/// <summary>
/// View model used for design-time data binding in the user interface.
/// </summary>
public class DesignTimeViewModel : ViewModelBase, IMainWindowViewModel
{
    private string _inputText = "Sample input text for design time. Some more text to make it longer so that the scroll bar shows up.";
    private string _outputText = "Sample output text for design time. Some more text to make it longer so that the scroll bar shows up.";

    /// <summary>
    /// Gets the command used for en-cloaking the output text in the Enigma machine.
    /// </summary>
    public ICommand CloakCommand
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
    /// Gets the command used for encrypting the input text to product the output text.
    /// </summary>
    public ICommand EncryptCommand
    {
        get;
    } = new RelayCommand(static _ => { }, static _ => true);

    /// <summary>
    /// Gets or sets the sample input text to be displayed during design time.
    /// </summary>
    public string InputText
    {
        get => _inputText;
        set
        {
            if (_inputText.Equals(value, StringComparison.Ordinal))
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
    } = new RelayCommand(static _ => { }, static _ => true);

    /// <summary>
    /// Gets or sets the sample output text to be displayed during design time.
    /// </summary>
    public string OutputText
    {
        get => _outputText;
        set
        {
            if (_outputText.Equals(value, StringComparison.Ordinal))
            {
                _outputText = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets the command used for reconfiguring the Enigma machine.
    /// </summary>
    public ICommand ReconfigureCommand
    {
        get;
    } = new RelayCommand(static _ => { }, static _ => true);

    /// <summary>
    /// Gets the command used for resetting the state of the Enigma machine.
    /// </summary>
    public ICommand ResetCommand
    {
        get;
    } = new RelayCommand(static _ => { }, static _ => true);

    /// <summary>
    /// Gets the command used for saving the output text from the Enigma machine.
    /// </summary>
    public ICommand SaveCommand
    {
        get;
    } = new RelayCommand(static _ => { }, static _ => true);
}