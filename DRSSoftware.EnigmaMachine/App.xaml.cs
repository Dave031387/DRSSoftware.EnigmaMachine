namespace DRSSoftware.EnigmaMachine;

using System.Windows;
using DRSSoftware.DRSBasicDI;
using DRSSoftware.DRSBasicDI.Interfaces;
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

        IContainer container = ContainerBuilder.GetInstance()
            .AddSingleton<IEnigmaMachineBuilder, EnigmaMachineBuilder>()
            .AddSingleton<IInputOutputService, InputOutputService>()
            .AddSingleton<IMainWindowViewModel, MainWindowViewModel>()
            .AddSingleton<IStringDialogService, StringDialogService>()
            .AddTransient<IStringDialogViewModel, StringDialogViewModel>()
            .AddSingleton<MainWindow>()
            .AddTransient<StringDialogView>()
            .Build();
        MainWindow mainWindow = container.Resolve<MainWindow>();
        mainWindow.DataContext = container.Resolve<IMainWindowViewModel>();
        mainWindow.Show();
    }
}