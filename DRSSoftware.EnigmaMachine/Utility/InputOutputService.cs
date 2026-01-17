namespace DRSSoftware.EnigmaMachine.Utility;

using System.IO;
using System.Windows;
using Microsoft.Win32;

/// <summary>
/// This class implements a service used for loading and saving text files.
/// </summary>
internal sealed class InputOutputService : IInputOutputService
{
    /// <summary>
    /// A file filter string for text files.
    /// </summary>
    private const string FileFilter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

    /// <summary>
    /// The default directory for file dialogs.
    /// </summary>
    private readonly string _defaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    /// <summary>
    /// Loads a text file and returns its content as a string.
    /// </summary>
    /// <returns>
    /// The string contents of the selected text file. If no file is selected or an error occurs,
    /// returns an empty string.
    /// </returns>
    public string LoadTextFile()
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = FileFilter,
            ForcePreviewPane = true,
            InitialDirectory = _defaultDirectory,
            Multiselect = false,
            Title = "Load Text File"
        };

        if (openFileDialog.ShowDialog() == true)
        {
#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                return File.ReadAllText(openFileDialog.FileName);
            }
            catch (Exception ex)
            {
                string title = "File Load Error";
                string message = $"Failed to load file: {openFileDialog.FileName}\nReason: {ex.Message}";
                _ = MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        return string.Empty;
    }

    /// <summary>
    /// Saves the provided string content to a text file.
    /// </summary>
    /// <param name="content">
    /// The string content to be saved to the text file.
    /// </param>
    public void SaveTextFile(string content)
    {
        SaveFileDialog saveFileDialog = new()
        {
            AddExtension = true,
            DefaultExt = ".txt",
            FileName = "Untitled.txt",
            Filter = FileFilter,
            InitialDirectory = _defaultDirectory,
            OverwritePrompt = true,
            Title = "Save Text File"
        };

        bool? result = saveFileDialog.ShowDialog();

        if (result is true)
        {
#pragma warning disable CA1031 // Do not catch general exception types
            try
            {
                File.WriteAllText(saveFileDialog.FileName, content);
            }
            catch (Exception ex)
            {
                string title = "File Save Error";
                string message = $"Failed to save file: {saveFileDialog.FileName}\nReason: {ex.Message}";
                _ = MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }
    }
}