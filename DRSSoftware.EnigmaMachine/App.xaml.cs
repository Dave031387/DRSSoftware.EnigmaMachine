namespace DRSSoftware.EnigmaMachine;

using System.Windows;
using DRSSoftware.DRSBasicDI;
using DRSSoftware.DRSBasicDI.Interfaces;
using DRSSoftware.EnigmaMachine.Utility;
using DRSSoftware.EnigmaMachine.ViewModels;
using DRSSoftware.EnigmaMachine.Views;
using DRSSoftware.EnigmaV2;

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

        IContainer container = ContainerBuilder.GetInstance("Enigma Machine")
            .AddSingleton<ICloakingService, CloakingService>()
            .AddSingleton<IConfigurationDialogService, ConfigurationDialogService>()
            .AddTransient<IConfigurationDialogViewModel, ConfigurationDialogViewModel>()
            .AddTransient<IDialogView, ConfigurationDialogView>(static builder => builder.WithResolvingKey(ConfigurationDialogKey))
            .AddTransient<IDialogView, StringDialogView>(static builder => builder.WithResolvingKey(StringDialogKey))
            .AddSingleton<IEmbeddingService, EmbeddingService>()
            .AddTransient<IEnigmaMachine, EnigmaMachine>()
            .AddSingleton<IEnigmaMachineBuilder, EnigmaMachineBuilder>()
            .AddSingleton<IIndicatorStringGenerator, IndicatorStringGenerator>()
            .AddSingleton<IInputOutputService, InputOutputService>()
            .AddSingleton<IMainWindowViewModel, MainWindowViewModel>()
            .AddTransient<IOpenFileService, OpenFileService>()
            .AddTransient<ISaveFileService, SaveFileService>()
            .AddSingleton<ISecureNumberGenerator, SecureNumberGenerator>()
            .AddSingleton<IStringDialogService, StringDialogService>()
            .AddTransient<IStringDialogViewModel, StringDialogViewModel>()
            .AddSingleton<MainWindow>()
            .Build();
        MainWindow mainWindow = container.Resolve<MainWindow>();
        mainWindow.DataContext = container.Resolve<IMainWindowViewModel>();
        mainWindow.Show();
    }
}