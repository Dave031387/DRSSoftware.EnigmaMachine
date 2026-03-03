namespace DRSSoftware.EnigmaMachine.ViewModels;

using System.Windows.Input;
using DRSSoftware.EnigmaMachine.Commands;

/// <summary>
/// View model for obtaining a string input from the user.
/// </summary>
internal sealed class StringDialogViewModel : ViewModelBase, IStringDialogViewModel
{
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
    /// Gets or sets the header text to display in the view.
    /// </summary>
    public string HeaderText
    {
        get;
        set
        {
            if (!field.Equals(value, StringComparison.Ordinal))
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = string.Empty;

    /// <summary>
    /// Gets or sets the input text entered by the user.
    /// </summary>
    public string InputText
    {
        get;
        set
        {
            if (!field.Equals(value, StringComparison.Ordinal))
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = string.Empty;

    /// <summary>
    /// Gets or sets the window title.
    /// </summary>
    public string Title
    {
        get;
        set
        {
            if (!field.Equals(value, StringComparison.Ordinal))
            {
                field = value;
                OnPropertyChanged();
            }
        }
    } = string.Empty;

    /// <summary>
    /// Gets a value that indicates whether or not the Accept command can be executed.
    /// </summary>
    /// <returns>
    /// A value indicating whether or not the Accept command can be executed.
    /// </returns>
    private bool CanAccept => !string.IsNullOrWhiteSpace(InputText) && InputText.Length >= MinStringLength;

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