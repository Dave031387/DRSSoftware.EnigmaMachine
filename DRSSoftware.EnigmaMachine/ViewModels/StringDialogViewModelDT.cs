namespace DRSSoftware.EnigmaMachine.ViewModels;

using System.Windows.Input;
using DRSSoftware.EnigmaMachine.Commands;

/// <summary>
/// View model used for design-time data binding in the StringDialogView.
/// </summary>
internal sealed class StringDialogViewModelDT : ViewModelBase, IStringDialogViewModel
{
    /// <summary>
    /// Gets the command used for accepting the input string.
    /// </summary>
    public ICommand AcceptCommand => new RelayCommand(static _ => { }, static _ => true);

    /// <summary>
    /// Gets the command used for cancelling the string input.
    /// </summary>
    public ICommand CancelCommand => new RelayCommand(static _ => { }, static _ => true);

    /// <summary>
    /// Gets or sets a value indicating whether the associated view should be closed.
    /// </summary>
    public bool CloseTrigger
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
    } = "Enter a sample string:";

    /// <summary>
    /// Gets or sets the input text entered by the user.
    /// </summary>
    public string InputText
    {
        get;
        set;
    } = "Sample input text";

    /// <summary>
    /// Gets or sets the window title.
    /// </summary>
    public string Title
    {
        get;
        set;
    } = "Sample String Entry";
}