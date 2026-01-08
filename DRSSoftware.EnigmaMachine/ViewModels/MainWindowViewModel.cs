namespace DRSSoftware.EnigmaMachine.ViewModels;

using DRSSoftware.EnigmaMachine.Utility;
using DRSSoftware.EnigmaV2;

/// <summary>
/// Represents the view model for the application's main window, providing data binding and command
/// logic for the user interface.
/// </summary>
public class MainWindowViewModel(IEnigmaMachineBuilder builder) : ViewModelBase
{
    /// <summary>
    /// Holds a reference to the Enigma machine builder.
    /// </summary>
    private readonly IEnigmaMachineBuilder _builder = builder;

    /// <summary>
    /// Gets a reference to the Enigma machine instance.
    /// </summary>
    public IEnigmaMachine EnigmaMachine
    {
        get;
        private set;
    } = builder.Build();
}