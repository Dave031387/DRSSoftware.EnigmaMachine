namespace DRSSoftware.EnigmaMachine.Converters;

using System.Globalization;
using System.Windows.Data;

/// <summary>
/// Converts a boolean value to a System.Windows.Visibility enumeration value.
/// </summary>
[ValueConversion(typeof(bool), typeof(System.Windows.Media.Brush))]
public class BoolToStatusBrushConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean value to a System.Windows.Media.Brush value.
    /// </summary>
    /// <param name="value">
    /// The boolean value to be converted.
    /// </param>
    /// <param name="targetType">
    /// The type of the binding target property.
    /// </param>
    /// <param name="parameter">
    /// An optional parameter to be used in the conversion. (Not used in this implementation.)
    /// </param>
    /// <param name="culture">
    /// The culture to be used in the conversion. (Not used in this implementation.)
    /// </param>
    /// <returns>
    /// Visibility.Visible if the input value is true; otherwise, Visibility.Collapsed.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if the input value is not of type bool.
    /// </exception>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool boolValue)
        {
            throw new ArgumentException("The bound value must be of type bool.", nameof(value));
        }

        return boolValue
            ? System.Windows.Media.Brushes.Green
            : System.Windows.Media.Brushes.Red;
    }

    /// <summary>
    /// Converts a value from the target type back to the source type.
    /// </summary>
    /// <remarks>
    /// This method is not implemented and will always throw a NotImplementedException.
    /// </remarks>
    /// <param name="value">
    /// The value produced by the binding target to be converted back to the source type.
    /// </param>
    /// <param name="targetType">
    /// The type to convert the value to, usually the source type of the binding.
    /// </param>
    /// <param name="parameter">
    /// An optional parameter to be used in the conversion logic. This value can be null.
    /// </param>
    /// <param name="culture">
    /// The culture to use in the conversion, which may affect formatting and parsing operations.
    /// </param>
    /// <returns>
    /// An object that represents the converted value to be passed back to the source binding.
    /// </returns>
    /// <exception cref="NotImplementedException">
    /// Always thrown, as this method is not implemented.
    /// </exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}