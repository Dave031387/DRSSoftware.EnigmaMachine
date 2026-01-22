namespace DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// Defines the properties and methods required for opening and reading the contents of a text file.
/// </summary>
public interface IOpenFileService
{
    /// <summary>
    /// Gets or sets a string containing the full file path of the file selected in the Open File
    /// dialog.
    /// </summary>
    public string FileName
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a filter that determines what types of files are displayed from the Open File
    /// dialog.
    /// </summary>
    public string Filter
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets an option flag indicating whether the Open File dialog forces the preview pane
    /// on.
    /// </summary>
    public bool ForcePreviewPane
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the initial directory displayed by the Open File dialog.
    /// </summary>
    public string InitialDirectory
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets an option indicating whether Open File dialog allows users to select multiple
    /// files.
    /// </summary>
    public bool Multiselect
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the text shown in the title bar of the Open File dialog.
    /// </summary>
    public string Title
    {
        get;
        set;
    }

    /// <summary>
    /// Opens the selected file, reads all the text in the file, and then closes the file.
    /// </summary>
    /// <returns>
    /// A string containing all the text in the selected file, or an empty string if any exceptions
    /// are thrown.
    /// </returns>
    public string ReadAllText();

    /// <summary>
    /// Displays the Open File dialog.
    /// </summary>
    /// <returns>
    /// If the user clicks the OK button of the Open File dialog, <see langword="true" /> is
    /// returned; otherwise, <see langword="false" />.
    /// </returns>
    public bool? ShowDialog();
}