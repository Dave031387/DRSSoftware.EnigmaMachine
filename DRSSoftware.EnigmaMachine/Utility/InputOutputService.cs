namespace DRSSoftware.EnigmaMachine.Utility;

using DRSSoftware.DRSBasicDI.Interfaces;

/// <summary>
/// This class implements a service used for loading and saving text files.
/// </summary>
/// <param name="container">
/// A reference to the dependency injection container.
/// </param>
internal sealed class InputOutputService(IContainer container) : IInputOutputService
{
    /// <summary>
    /// A file filter string for text files.
    /// </summary>
    private const string FileFilter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

    /// <summary>
    /// Holds a reference to the dependency injection container.
    /// </summary>
    private readonly IContainer _container = container;

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
        IOpenFileService fileService = _container.Resolve<IOpenFileService>();
        fileService.Filter = FileFilter;
        fileService.ForcePreviewPane = true;
        fileService.InitialDirectory = _defaultDirectory;
        fileService.Multiselect = false;
        fileService.Title = "Load Text File";
        return fileService.ShowDialog() is true ? fileService.ReadAllText() : string.Empty;
    }

    /// <summary>
    /// Saves the provided string content to a text file.
    /// </summary>
    /// <param name="contents">
    /// The string contents to be saved to the text file.
    /// </param>
    public void SaveTextFile(string contents)
    {
        ISaveFileService saveFileService = _container.Resolve<ISaveFileService>();
        saveFileService.AddExtension = true;
        saveFileService.DefaultExt = ".txt";
        saveFileService.FileName = "encrypted.txt";
        saveFileService.Filter = FileFilter;
        saveFileService.InitialDirectory = _defaultDirectory;
        saveFileService.OverwritePrompt = true;
        saveFileService.Title = "Save Text File";

        if (saveFileService.ShowDialog() is true)
        {
            saveFileService.WriteAllText(contents);
        }
    }
}