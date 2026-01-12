namespace DRSSoftware.EnigmaMachine.ViewModels;

using System.Windows.Input;
using DRSSoftware.EnigmaMachine.Commands;

/// <summary>
/// View model used for design-time data binding in the GetStringView.
/// </summary>
internal sealed class StringDialogViewModelDT : ViewModelBase, IStringDialogViewModel
{
    private bool _closeTrigger;
    private string _headerText = "Enter the decloaking string:";
    private string _inputText = "Sample input text";
    private string _title = "Decloak String Entry";

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
}