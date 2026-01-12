namespace DRSSoftware.EnigmaMachine.ViewModels;

using System.Windows.Input;

/// <summary>
/// Defines a view model interface for obtaining a string input from the user.
/// </summary>
/// <remarks>
/// Implementations of this interface provide commands for accepting or canceling the user input, as
/// well as properties for configuring the dialog's title and header.
/// </remarks>
public interface IStringDialogViewModel
{
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
    bool CloseTrigger
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the header text to display in the view.
    /// </summary>
    public string HeaderText
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the input text entered by the user.
    /// </summary>
    public string InputText
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the window title.
    /// </summary>
    public string Title
    {
        get;
        set;
    }
}