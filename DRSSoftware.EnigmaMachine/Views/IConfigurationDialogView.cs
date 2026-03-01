namespace DRSSoftware.EnigmaMachine.Views;

using System.Windows;

/// <summary>
/// Defines the public properties and methods of the <see cref="ConfigurationDialogView" /> class.
/// </summary>
public interface IConfigurationDialogView
{
    /// <summary>
    /// Gets or sets the data context for an element when it participates in a data binding.
    /// </summary>
    public object DataContext
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the window that owns this window.
    /// </summary>
    public Window Owner
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the position of the window when it is first shown.
    /// </summary>
    public WindowStartupLocation WindowStartupLocation
    {
        get;
        set;
    }

    /// <summary>
    /// Opens a window and returns only when the newly opened window is closed.
    /// </summary>
    /// <returns>
    /// A <see cref="Nullable{T}" /> value of type <see langword="bool" /> that specifies whether
    /// the activity was accepted ( <see langword="true" />) or cancelled (
    /// <see langword="false" />). The return value is the value of the
    /// <see cref="Window.DialogResult" /> property before a window closes.
    /// </returns>
    public bool? ShowDialog();
}