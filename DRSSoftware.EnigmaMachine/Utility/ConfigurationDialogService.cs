namespace DRSSoftware.EnigmaMachine.Utility;

using DRSSoftware.DRSBasicDI.Interfaces;
using DRSSoftware.EnigmaMachine.ViewModels;
using DRSSoftware.EnigmaMachine.Views;

/// <summary>
/// Provides a service for displaying a dialog used for setting and retrieving the configuration for
/// the Enigma machine.
/// </summary>
/// <param name="container">
/// The dependency injection container used to resolve required view and view model instances.
/// </param>
internal sealed class ConfigurationDialogService(IContainer container) : DialogServiceBase(container), IConfigurationDialogService
{
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
        IConfigurationDialogViewModel viewModel = _container.Resolve<IConfigurationDialogViewModel>();
        viewModel.Initialize(currentConfiguration);
        IDialogView view = GetDialogView(ConfigurationDialogKey, viewModel);
        _ = view.ShowDialog();
        return viewModel.EnigmaConfiguration;
    }
}