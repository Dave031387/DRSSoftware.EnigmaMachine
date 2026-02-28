namespace DRSSoftware.EnigmaMachine.Utility;

public class InputOutputServiceTests
{
    private readonly string _defaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    private readonly string _defaultFileExtension = ".txt";
    private readonly string _defaultFileName = "encrypted.txt";
    private readonly string _fileFilter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
    private readonly string _openFileDialogTitle = "Load Text File";
    private readonly string _sampleText = "This is some sample text.";
    private readonly string _saveFileDialogTitle = "Save Text File";

    [Fact]
    public void LoadTextFile_ShouldReturnEmptyStringIfOpenFileServiceReturnsFalse()
    {
        // Arrange
        Mock<IOpenFileService> mockFileService = new(MockBehavior.Strict);
        mockFileService
            .SetupSet(m => m.Filter = _fileFilter)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.ForcePreviewPane = true)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.InitialDirectory = _defaultDirectory)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.Multiselect = false)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.Title = _openFileDialogTitle)
            .Verifiable(Times.Once);
        mockFileService
            .Setup(m => m.ShowDialog())
            .Returns(false)
            .Verifiable(Times.Once);
        Mock<IContainer> mockContainer = new(MockBehavior.Strict);
        mockContainer
            .Setup(m => m.Resolve<IOpenFileService>())
            .Returns(mockFileService.Object)
            .Verifiable(Times.Once);
        InputOutputService inputOutputService = new(mockContainer.Object);

        // Act
        string actual = inputOutputService.LoadTextFile();

        // Assert
        actual
            .Should()
            .BeEmpty();
        mockFileService.VerifyAll();
        mockContainer.VerifyAll();
    }

    [Fact]
    public void LoadTextFile_ShouldReturnInputTextIfOpenFileServiceReturnsTrue()
    {
        // Arrange
        Mock<IOpenFileService> mockFileService = new(MockBehavior.Strict);
        mockFileService
            .SetupSet(m => m.Filter = _fileFilter)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.ForcePreviewPane = true)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.InitialDirectory = _defaultDirectory)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.Multiselect = false)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.Title = _openFileDialogTitle)
            .Verifiable(Times.Once);
        mockFileService
            .Setup(m => m.ShowDialog())
            .Returns(true)
            .Verifiable(Times.Once);
        mockFileService
            .Setup(m => m.ReadAllText())
            .Returns(_sampleText)
            .Verifiable(Times.Once);
        Mock<IContainer> mockContainer = new(MockBehavior.Strict);
        mockContainer
            .Setup(m => m.Resolve<IOpenFileService>())
            .Returns(mockFileService.Object)
            .Verifiable(Times.Once);
        InputOutputService inputOutputService = new(mockContainer.Object);

        // Act
        string actual = inputOutputService.LoadTextFile();

        // Assert
        actual
            .Should()
            .Be(_sampleText);
        mockFileService.VerifyAll();
        mockContainer.VerifyAll();
    }

    [Fact]
    public void SaveTextFile_ShouldDoNothingIfSaveFileServiceReturnsFalse()
    {
        // Arrange
        Mock<ISaveFileService> mockFileService = new(MockBehavior.Strict);
        mockFileService
            .SetupSet(m => m.AddExtension = true)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.DefaultExt = _defaultFileExtension)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.FileName = _defaultFileName)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.Filter = _fileFilter)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.InitialDirectory = _defaultDirectory)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.OverwritePrompt = true)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.Title = _saveFileDialogTitle)
            .Verifiable(Times.Once);
        mockFileService
            .Setup(m => m.ShowDialog())
            .Returns(false)
            .Verifiable(Times.Once);
        Mock<IContainer> mockContainer = new(MockBehavior.Strict);
        mockContainer
            .Setup(m => m.Resolve<ISaveFileService>())
            .Returns(mockFileService.Object)
            .Verifiable(Times.Once);
        InputOutputService inputOutputService = new(mockContainer.Object);

        // Act
        inputOutputService.SaveTextFile(_sampleText);

        // Assert
        mockFileService.VerifyAll();
        mockContainer.VerifyAll();
    }

    [Fact]
    public void SaveTextFile_ShouldWriteTextIfSaveFileServiceReturnsTrue()
    {
        // Arrange
        Mock<ISaveFileService> mockFileService = new(MockBehavior.Strict);
        mockFileService
            .SetupSet(m => m.AddExtension = true)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.DefaultExt = _defaultFileExtension)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.FileName = _defaultFileName)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.Filter = _fileFilter)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.InitialDirectory = _defaultDirectory)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.OverwritePrompt = true)
            .Verifiable(Times.Once);
        mockFileService
            .SetupSet(m => m.Title = _saveFileDialogTitle)
            .Verifiable(Times.Once);
        mockFileService
            .Setup(m => m.ShowDialog())
            .Returns(true)
            .Verifiable(Times.Once);
        mockFileService
            .Setup(m => m.WriteAllText(_sampleText))
            .Verifiable(Times.Once);
        Mock<IContainer> mockContainer = new(MockBehavior.Strict);
        mockContainer
            .Setup(m => m.Resolve<ISaveFileService>())
            .Returns(mockFileService.Object)
            .Verifiable(Times.Once);
        InputOutputService inputOutputService = new(mockContainer.Object);

        // Act
        inputOutputService.SaveTextFile(_sampleText);

        // Assert
        mockFileService.VerifyAll();
        mockContainer.VerifyAll();
    }
}