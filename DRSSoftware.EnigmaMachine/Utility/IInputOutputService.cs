namespace DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// Defines methods for handling input and output of text files.
/// </summary>
internal interface IInputOutputService
{
    /// <summary>
    /// Loads a text file and returns its content as a string.
    /// </summary>
    /// <returns>
    /// The string contents of the selected text file. If no file is selected or an error occurs,
    /// returns an empty string.
    /// </returns>
    public string LoadTextFile();

    /// <summary>
    /// Saves the provided string content to a text file.
    /// </summary>
    /// <param name="content">
    /// The string content to be saved to the text file.
    /// </param>
    void SaveTextFile(string content);
}