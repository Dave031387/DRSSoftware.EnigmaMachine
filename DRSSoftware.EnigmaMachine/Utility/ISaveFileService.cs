namespace DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// Defines the properties and methods required for saving the contents of a text file.
/// </summary>
public interface ISaveFileService
{
    /// <summary>
    /// Gets or sets a value indicating whether the Save File dialog automatically adds an extension
    /// to a file name if the user omits an extension.
    /// </summary>
    public bool AddExtension
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value that specifies the default extension string to use to filter the list
    /// of files that are displayed in the Save File dialog.
    /// </summary>
    public string DefaultExt
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a string containing the full file path of the file selected in the Save File
    /// dialog.
    /// </summary>
    public string FileName
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a filter that determines what types of files are displayed from the Save File
    /// dialog.
    /// </summary>
    public string Filter
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the initial directory displayed by the Save File dialog.
    /// </summary>
    public string InitialDirectory
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets a value indicating whether Save File dialog displays a warning if the user
    /// specifies the name of a file that already exists.
    /// </summary>
    public bool OverwritePrompt
    {
        get;
        set;
    }

    /// <summary>
    /// Gets or sets the text shown in the title bar of the Save File dialog.
    /// </summary>
    public string Title
    {
        get;
        set;
    }

    /// <summary>
    /// Displays the Save File dialog.
    /// </summary>
    /// <returns>
    /// If the user clicks the OK button of the Save File dialog, <see langword="true" /> is
    /// returned; otherwise, <see langword="false" />.
    /// </returns>
    public bool? ShowDialog();

    /// <summary>
    /// Creates a new text file, writes the <paramref name="contents" /> to the file, and then
    /// closes the file. If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="contents">
    /// The string contents to be written to the text file.
    /// </param>
    public void WriteAllText(string contents);
}