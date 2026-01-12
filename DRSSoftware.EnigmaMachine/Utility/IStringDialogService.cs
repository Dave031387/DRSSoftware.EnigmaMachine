namespace DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// Defines the method used for obtain string input from the user.
/// </summary>
public interface IStringDialogService
{
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
    public string GetString(string title, string header);
}