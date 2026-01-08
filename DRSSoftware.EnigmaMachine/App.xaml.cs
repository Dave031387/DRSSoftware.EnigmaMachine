namespace DRSSoftware.EnigmaMachine;

using System.Windows;
using DRSSoftware.EnigmaMachine.Utility;
using DRSSoftware.EnigmaMachine.ViewModels;
using DRSSoftware.EnigmaMachine.Views;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Raises the <see cref="Application.Startup" /> event.
    /// </summary>
    /// <param name="e">
    /// A <see cref="StartupEventArgs" /> that contains the event data.
    /// </param>
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        MainWindow mainWindow = new()
        {
            DataContext = new MainWindowViewModel(new EnigmaMachineBuilder())
        };
        mainWindow.Show();
    }
}