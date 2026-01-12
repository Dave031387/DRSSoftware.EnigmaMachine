namespace DRSSoftware.EnigmaMachine.Behaviors;

using System.Windows;
using Microsoft.Xaml.Behaviors;

/// <summary>
/// Provides a behavior that closes the associated window when the specified trigger property is set
/// to true.
/// </summary>
/// <remarks>
/// This behavior can be attached to a Window to enable closing the window in response to changes in
/// a bound property, such as from a view model. <br /> This is useful in MVVM scenarios where the
/// view model needs to initiate window closure without direct reference to the view.
/// </remarks>
public class CloseWindowBehavior : Behavior<Window>
{
    /// <summary>
    /// Registers the CloseTrigger dependency property.
    /// </summary>
    public static readonly DependencyProperty CloseTriggerProperty =
        DependencyProperty.Register(nameof(CloseTrigger),
                                    typeof(bool),
                                    typeof(CloseWindowBehavior),
                                    new PropertyMetadata(false, OnCloseTriggerChanged));

    /// <summary>
    /// Gets or sets a value indicating whether the associated window should be closed.
    /// </summary>
    public bool CloseTrigger
    {
        get
        {
            return (bool)GetValue(CloseTriggerProperty);
        }
        set
        {
            SetValue(CloseTriggerProperty, value);
        }
    }

    /// <summary>
    /// Event handler called when the CloseTrigger property changes.
    /// </summary>
    /// <param name="d">
    /// The dependency object that raised the event.
    /// </param>
    /// <param name="e">
    /// The dependency property changed event arguments.
    /// </param>
    private static void OnCloseTriggerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        CloseWindowBehavior? behavior = d as CloseWindowBehavior;
        behavior?.OnCloseTriggerChanged();
    }

    /// <summary>
    /// Closes the associated window if the trigger property is set.
    /// </summary>
    private void OnCloseTriggerChanged()
    {
        if (CloseTrigger)
        {
            AssociatedObject.Close();
        }
    }
}