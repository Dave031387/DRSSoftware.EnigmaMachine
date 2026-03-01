namespace DRSSoftware.EnigmaMachine.Utility;

using System.Windows;
using DRSSoftware.DRSBasicDI.Interfaces;
using DRSSoftware.EnigmaMachine.ViewModels;
using DRSSoftware.EnigmaMachine.Views;

/// <summary>
/// Provides a service for displaying a dialog to prompt the user for a string input.
/// </summary>
/// <param name="container">The dependency injection container used to resolve required view and view model instances.</param>
internal sealed class StringDialogService(IContainer container) : IStringDialogService
{
    private readonly IContainer _container = container;

    /// <summary>
    /// Get string input from the user via the GetStringView.
    /// </summary>
    /// <param name="title">
    /// The title to be displayed in the GetStringView.
    /// </param>
    /// <param name="header">
    /// The header text to be displayed in the GetStringView.
    /// </param>
    /// <returns>
    /// The text string that was entered by the user, or an empty string if the user cancelled the
    /// input.
    /// </returns>
    public string GetString(string title, string header)
    {
        IStringDialogView view = _container.Resolve<IStringDialogView>();
        view.Owner = Application.Current.MainWindow;
        view.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        IStringDialogViewModel viewModel = _container.Resolve<IStringDialogViewModel>();
        viewModel.Title = title;
        viewModel.HeaderText = header;
        view.DataContext = viewModel;
        _ = view.ShowDialog();
        return viewModel.InputText;
    }
}
