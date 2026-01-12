namespace DRSSoftware.EnigmaMachine.Utility;

using System.IO;
using System.Windows;
using Microsoft.Win32;

internal sealed class InputOutputService : IInputOutputService
{
    public string LoadTextFile()
    {
        OpenFileDialog openFileDialog = new()
        {
            Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
            Title = "Open Text File",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            ForcePreviewPane = true,
            Multiselect = false
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
}