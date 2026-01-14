namespace DRSSoftware.EnigmaMachine.Utility;

using DRSSoftware.DRSBasicDI.Interfaces;

internal sealed class ConfigurationDialogService(IContainer container) : IConfigurationDialogService
{
    private readonly IContainer _container = container;
}
