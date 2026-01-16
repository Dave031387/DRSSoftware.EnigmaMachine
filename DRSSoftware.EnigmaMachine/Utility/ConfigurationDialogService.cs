namespace DRSSoftware.EnigmaMachine.Utility;

using System.Windows;
using DRSSoftware.DRSBasicDI.Interfaces;
using DRSSoftware.EnigmaMachine.ViewModels;
using DRSSoftware.EnigmaMachine.Views;

internal sealed class ConfigurationDialogService(IContainer container) : IConfigurationDialogService
{
    /// <summary>
    /// Holds a reference to the dependency injection container.
    /// </summary>
    private readonly IContainer _container = container;

    /// <summary>
    /// Obtains a new Enigma machine configuration from the user.
    /// </summary>
    /// <param name="currentConfiguration">
    /// The current Enigma machine configuration. Used as a starting point for the new
    /// configuration.
    /// </param>
    /// <returns>
    /// The user-specified configuration of the Enigma machine.
    /// </returns>
    public EnigmaConfiguration GetConfiguration(EnigmaConfiguration currentConfiguration)
    {
        ConfigurationDialogView view = _container.Resolve<ConfigurationDialogView>();
        view.Owner = Application.Current.MainWindow;
        view.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        IConfigurationDialogViewModel viewModel = _container.Resolve<IConfigurationDialogViewModel>();
        viewModel.Initialize(currentConfiguration);
        view.DataContext = viewModel;
        _ = view.ShowDialog();
        return viewModel.EnigmaConfiguration;
    }
}
