namespace DRSSoftware.EnigmaMachine.Utility;

using System.IO;
using System.Windows;
using Microsoft.Win32;

/// <summary>
/// This class is a wrapper around the <see cref="OpenFileDialog" /> class.
/// </summary>
internal sealed class OpenFileService : IOpenFileService
{
    /// <summary>
    /// Holds a reference to the <see cref="OpenFileDialog" /> instance.
    /// </summary>
    private readonly OpenFileDialog _openFileDialog;

    /// <summary>
    /// Creates a new instance of the <see cref="OpenFileService" /> class.
    /// </summary>
    public OpenFileService() => _openFileDialog = new OpenFileDialog();

    /// <summary>
    /// Gets or sets a string containing the full file path of the file selected in the
    /// <see cref="OpenFileDialog" />.
    /// </summary>
    public string FileName
    {
        get => _openFileDialog.FileName;
        set => _openFileDialog.FileName = value;
    }

    /// <summary>
    /// Gets or sets a filter that determines what types of files are displayed from the
    /// <see cref="OpenFileDialog" />.
    /// </summary>
    public string Filter
    {
        get => _openFileDialog.Filter;
        set => _openFileDialog.Filter = value;
    }

    /// <summary>
    /// Gets or sets an option flag indicating whether the <see cref="OpenFileDialog" /> forces the
    /// preview pane on.
    /// </summary>
    public bool ForcePreviewPane
    {
        get => _openFileDialog.ForcePreviewPane;
        set => _openFileDialog.ForcePreviewPane = value;
    }

    /// <summary>
    /// Gets or sets the initial directory displayed by the <see cref="OpenFileDialog" />.
    /// </summary>
    public string InitialDirectory
    {
        get => _openFileDialog.InitialDirectory;
        set => _openFileDialog.InitialDirectory = value;
    }

    /// <summary>
    /// Gets or sets an option indicating whether <see cref="OpenFileDialog" /> allows users to
    /// select multiple files.
    /// </summary>
    public bool Multiselect
    {
        get => _openFileDialog.Multiselect;
        set => _openFileDialog.Multiselect = value;
    }

    /// <summary>
    /// Gets or sets the text shown in the title bar of the <see cref="OpenFileDialog" />.
    /// </summary>
    public string Title
    {
        get => _openFileDialog.Title;
        set => _openFileDialog.Title = value;
    }

    /// <summary>
    /// Opens the selected file, reads all the text in the file, and then closes the file.
    /// </summary>
    /// <returns>
    /// A string containing all the text in the selected file, or an empty string if any exceptions
    /// are thrown.
    /// </returns>
    public string ReadAllText()
    {
#pragma warning disable CA1031 // Do not catch general exception types
        try
        {
            return File.ReadAllText(FileName);
        }
        catch (Exception ex)
        {
            string title = "File Load Error";
            string message = $"Failed to load file: {FileName}\nReason: {ex.Message}";
            _ = MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            return string.Empty;
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    /// <summary>
    /// Displays the <see cref="OpenFileDialog" />.
    /// </summary>
    /// <returns>
    /// If the user clicks the OK button of the <see cref="OpenFileDialog" />,
    /// <see langword="true" /> is returned; otherwise, <see langword="false" />.
    /// </returns>
    public bool? ShowDialog() => _openFileDialog.ShowDialog();
}