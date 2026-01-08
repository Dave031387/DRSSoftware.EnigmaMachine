namespace DRSSoftware.EnigmaMachine.ViewModels;

using System.ComponentModel;
using System.Runtime.CompilerServices;

/// <summary>
/// Serves as a base class for view model objects in applications following the Model-View-ViewModel
/// (MVVM) pattern.
/// </summary>
public class ViewModelBase : INotifyPropertyChanged
{
    /// <summary>
    /// Event that occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged" /> event to notify listeners that a property value
    /// has changed.
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property that changed. If not specified, the caller member name is used. Can
    /// be <see langword="null" /> to indicate that all properties have changed.
    /// </param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}