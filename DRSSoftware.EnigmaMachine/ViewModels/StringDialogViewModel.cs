namespace DRSSoftware.EnigmaMachine.ViewModels;

using System.Windows.Input;
using DRSSoftware.EnigmaMachine.Commands;

/// <summary>
/// View model for obtaining a string input from the user.
/// </summary>
internal sealed class StringDialogViewModel : ViewModelBase, IStringDialogViewModel
{
    private bool _closeTrigger;
    private string _headerText = string.Empty;
    private string _inputText = string.Empty;
    private string _title = string.Empty;

    /// <summary>
    /// Creates a new instance of the <see cref="StringDialogViewModel" /> class.
    /// </summary>
    public StringDialogViewModel()
    {
        AcceptCommand = new RelayCommand(_ => Accept(), _ => CanAccept);
        CancelCommand = new RelayCommand(_ => Cancel(), _ => true);
    }

    /// <summary>
    /// Gets the command used for accepting the input string.
    /// </summary>
    public ICommand AcceptCommand
    {
        get;
    }

    /// <summary>
    /// Gets the command used for cancelling the string input.
    /// </summary>
    public ICommand CancelCommand
    {
        get;
    }

    /// <summary>
    /// Gets or sets a value indicating whether the associated view should be closed.
    /// </summary>
    public bool CloseTrigger
    {
        get => _closeTrigger;
        set
        {
            if (_closeTrigger != value)
            {
                _closeTrigger = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the header text to display in the view.
    /// </summary>
    public string HeaderText
    {
        get => _headerText;
        set
        {
            if (!_headerText.Equals(value, StringComparison.Ordinal))
            {
                _headerText = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the input text entered by the user.
    /// </summary>
    public string InputText
    {
        get => _inputText;
        set
        {
            if (!_inputText.Equals(value, StringComparison.Ordinal))
            {
                _inputText = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets or sets the window title.
    /// </summary>
    public string Title
    {
        get => _title;
        set
        {
            if (!_title.Equals(value, StringComparison.Ordinal))
            {
                _title = value;
                OnPropertyChanged();
            }
        }
    }

    /// <summary>
    /// Gets a value that indicates whether or not the Accept command can be executed.
    /// </summary>
    /// <returns>
    /// A value indicating whether or not the Accept command can be executed.
    /// </returns>
    private bool CanAccept => !string.IsNullOrWhiteSpace(_inputText) && _inputText.Length >= MinStringLength;

    /// <summary>
    /// Accept the user input and close the associated view.
    /// </summary>
    private void Accept() => CloseTrigger = true;

    /// <summary>
    /// Cancel the user input and close the associated view.
    /// </summary>
    private void Cancel()
    {
        InputText = string.Empty;
        CloseTrigger = true;
    }
}