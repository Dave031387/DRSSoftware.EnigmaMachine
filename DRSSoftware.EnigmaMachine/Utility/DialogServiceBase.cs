namespace DRSSoftware.EnigmaMachine.Utility;

using System.Windows;
using DRSSoftware.DRSBasicDI.Interfaces;
using DRSSoftware.EnigmaMachine.Views;

/// <summary>
/// This is the base class for dialog service classes. It comprises a single method for retrieving
/// an instance of the dialog view and assigning the view model to its data context.
/// </summary>
/// <param name="container">
/// </param>
internal class DialogServiceBase(IContainer container)
{
    /// <summary>
    /// Holds a reference to the dependency injection container.
    /// </summary>
    protected readonly IContainer _container = container;

    /// <summary>
    /// Gets the dialog view corresponding to the given <paramref name="resolvingKey" /> value and
    /// assigns the given <paramref name="viewModel" /> to its DataContext property.
    /// </summary>
    /// <param name="resolvingKey">
    /// The key value used for resolving the correct implementation type for the
    /// <see cref="IDialogView" /> dependency type.
    /// </param>
    /// <param name="viewModel">
    /// The view model object that is to be assigned to the DataContext of the dialog view.
    /// </param>
    /// <returns>
    /// An <see cref="IDialogView" /> object corresponding to the given
    /// <paramref name="resolvingKey" /> value.
    /// </returns>
    protected IDialogView GetDialogView(string resolvingKey, object viewModel)
    {
        IDialogView view = _container.Resolve<IDialogView>(resolvingKey);

        // The following if statement is required because Application.Current returns null during
        // unit testing. Therefore we can't set or test the Owner property during a unit test.
        if (Application.Current?.MainWindow is not null)
        {
            view.Owner = Application.Current.MainWindow;
        }

        view.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        view.DataContext = viewModel;

        return view;
    }
}