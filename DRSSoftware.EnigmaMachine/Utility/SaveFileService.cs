namespace DRSSoftware.EnigmaMachine.Utility;

using System.IO;
using System.Windows;
using Microsoft.Win32;

/// <summary>
/// This class is a wrapper around the <see cref="SaveFileDialog" /> class.
/// </summary>
internal sealed class SaveFileService : ISaveFileService
{
    /// <summary>
    /// Holds a reference to the <see cref="SaveFileDialog" /> instance.
    /// </summary>
    private readonly SaveFileDialog _saveFileDialog;

    /// <summary>
    /// Creates a new instance of the <see cref="SaveFileService" />.
    /// </summary>
    public SaveFileService() => _saveFileDialog = new SaveFileDialog();

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="SaveFileDialog" /> automatically adds
    /// an extension to a file name if the user omits an extension.
    /// </summary>
    public bool AddExtension
    {
        get => _saveFileDialog.AddExtension;
        set => _saveFileDialog.AddExtension = value;
    }

    /// <summary>
    /// Gets or sets a value that specifies the default extension string to use to filter the list
    /// of files that are displayed in the <see cref="SaveFileDialog" />.
    /// </summary>
    public string DefaultExt
    {
        get => _saveFileDialog.DefaultExt;
        set => _saveFileDialog.DefaultExt = value;
    }

    /// <summary>
    /// Gets or sets a string containing the full file path of the file selected in the
    /// <see cref="SaveFileDialog" />.
    /// </summary>
    public string FileName
    {
        get => _saveFileDialog.FileName;
        set => _saveFileDialog.FileName = value;
    }

    /// <summary>
    /// Gets or sets a filter that determines what types of files are displayed from the
    /// <see cref="SaveFileDialog" />.
    /// </summary>
    public string Filter
    {
        get => _saveFileDialog.Filter;
        set => _saveFileDialog.Filter = value;
    }

    /// <summary>
    /// Gets or sets the initial directory displayed by the <see cref="SaveFileDialog" />.
    /// </summary>
    public string InitialDirectory
    {
        get => _saveFileDialog.InitialDirectory;
        set => _saveFileDialog.InitialDirectory = value;
    }

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="SaveFileDialog" /> displays a warning if
    /// the user specifies the name of a file that already exists.
    /// </summary>
    public bool OverwritePrompt
    {
        get => _saveFileDialog.OverwritePrompt;
        set => _saveFileDialog.OverwritePrompt = value;
    }

    /// <summary>
    /// Gets or sets the text shown in the title bar of the <see cref="SaveFileDialog" />.
    /// </summary>
    public string Title
    {
        get => _saveFileDialog.Title;
        set => _saveFileDialog.Title = value;
    }

    /// <summary>
    /// Displays the <see cref="SaveFileDialog" />.
    /// </summary>
    /// <returns>
    /// If the user clicks the OK button of the <see cref="SaveFileDialog" />,
    /// <see langword="true" /> is returned; otherwise, <see langword="false" />.
    /// </returns>
    public bool? ShowDialog() => _saveFileDialog.ShowDialog();

    /// <summary>
    /// Creates a new text file, writes the <paramref name="contents" /> to the file, and then
    /// closes the file. If the target file already exists, it is overwritten.
    /// </summary>
    /// <param name="contents">
    /// The string contents to be written to the text file.
    /// </param>
    public void WriteAllText(string contents)
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            File.WriteAllText(_saveFileDialog.FileName, contents);
        }
        catch (Exception ex)
        {
            string title = "File Save Error";
            string message = $"Failed to save file: {_saveFileDialog.FileName}\nReason: {ex.Message}";
            _ = MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }
}