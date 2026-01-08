namespace DRSSoftware.EnigmaMachine.Commands;

using System.Windows.Input;

/// <summary>
/// Used in an MVVM application to relay command logic to view models.
/// </summary>
/// <param name="execute">
/// The action to be executed when the command is invoked.
/// </param>
/// <param name="canExecute">
/// A function that determines whether the command can execute in its current state.
/// </param>
public class RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute) : ICommand
{
    private readonly Func<object?, bool>? _canExecute = canExecute;
    private readonly Action<object?> _execute = execute ?? throw new ArgumentNullException(nameof(execute));

    /// <summary>
    /// An event that occurs when changes occur that affect whether or not the command should
    /// execute.
    /// </summary>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <summary>
    /// Determines whether the command can execute in its current state.
    /// </summary>
    /// <param name="parameter">
    /// An optional parameter to be used by the command.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the command can execute; otherwise, <see langword="false" />.
    /// </returns>
    public bool CanExecute(object? parameter)
        => _canExecute?.Invoke(parameter) ?? true;

    /// <summary>
    /// Executes the associated action using the specified parameter.
    /// </summary>
    /// <param name="parameter">
    /// An optional parameter to be used by the command.
    /// </param>
    public void Execute(object? parameter)
        => _execute(parameter);
}