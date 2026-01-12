namespace DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// Defines methods for handling input and output of text files.
/// </summary>
internal interface IInputOutputService
{
    /// <summary>
    /// Load the requested text file and return its contents as a string.
    /// </summary>
    /// <remarks>
    /// If the load operation was cancelled or failed, an empty string is returned.
    /// </remarks>
    /// <returns>
    /// The string contents of the requested file.
    /// </returns>
    public string LoadTextFile();
}