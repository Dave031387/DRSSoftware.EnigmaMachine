namespace DRSSoftware.EnigmaMachine.Views;

using System.Windows;

/// <summary>
/// Interaction logic for ConfigurationDialog.xaml
/// </summary>
public partial class ConfigurationDialogView : Window
{
    /// <summary>
    /// Creates a new instance of the <see cref="ConfigurationDialogView" /> class.
    /// </summary>
    public ConfigurationDialogView()
    {
        InitializeComponent();
        Loaded += OnWindowLoaded;
    }

    /// <summary>
    /// Sets focus to the seed text box when the window is loaded.
    /// </summary>
    /// <param name="sender">
    /// The event sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void OnWindowLoaded(object sender, RoutedEventArgs e)
        => seedTextBox.Focus();
}