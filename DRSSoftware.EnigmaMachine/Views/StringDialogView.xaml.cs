namespace DRSSoftware.EnigmaMachine.Views;

using System.Windows;

/// <summary>
/// Interaction logic for GetStringView.xaml
/// </summary>
public partial class StringDialogView : Window
{
    /// <summary>
    /// Creates a new instance of the <see cref="StringDialogView" /> class.
    /// </summary>
    public StringDialogView()
    {
        InitializeComponent();
        Loaded += OnWindowLoaded;
    }

    /// <summary>
    /// Sets focus to the input text box when the window is loaded.
    /// </summary>
    /// <param name="sender">
    /// The event sender.
    /// </param>
    /// <param name="e">
    /// The event arguments.
    /// </param>
    private void OnWindowLoaded(object sender, RoutedEventArgs e)
        => inputTextBox.Focus();
}