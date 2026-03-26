namespace DRSSoftware.EnigmaMachine.ViewModels;

using DRSSoftware.EnigmaMachine.Utility;

[ExcludeFromCodeCoverage]
public class MainWindowViewModelTests
{
    [Fact]
    public void CloakCommandCanExecute_ShouldReturnFalseWhenOutputTextIsAlreadyCloaked()
    {
        // Arrange
        string initialOutputText = "initial text";
        string cloakedOutputText = "cloaked text";
        string cloakingText = "cloaking text";
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(initialOutputText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(cloakedOutputText))
            .Returns(true)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.ApplyCloak(initialOutputText, cloakingText))
            .Returns(cloakedOutputText)
            .Verifiable(Times.Once);
        mocks.MockStringDialogService
            .Setup(m => m.GetString(CloakingStringDialogTitle, CloakingStringDialogHeader))
            .Returns(cloakingText)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.OutputText = initialOutputText;
        viewModel.CloakCommand.Execute(null);

        // Act
        bool actual = viewModel.CloakCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeFalse();
        mocks.VerifyAll();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\r \t\n")]
    public void CloakCommandCanExecute_ShouldReturnFalseWhenOutputTextIsNullOrEmptyOrWhiteSpace(string? outputText)
    {
        // Arrange
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(outputText!))
            .Returns(false)
            .Verifiable(Times.AtMost(1));
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.OutputText = outputText!;

        // Act
        bool actual = viewModel.CloakCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeFalse();
        mocks.VerifyAll();
    }

    [Fact]
    public void CloakCommandCanExecute_ShouldReturnTrueIfOutputTextIsNotCloaked()
    {
        // Arrange
        string outputText = "output text";
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(outputText))
            .Returns(false)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.OutputText = outputText;

        // Act
        bool actual = viewModel.CloakCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeTrue();
        mocks.VerifyAll();
    }

    [Fact]
    public void CloakCommandExecute_ShouldApplyCloakIfCloakingStringIsNotNullOrEmptyOrWhiteSpace()
    {
        // Arrange
        string initialText = "initial text";
        string cloakingText = "cloaking text";
        string expected = "cloaked text";
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(initialText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(true)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.ApplyCloak(initialText, cloakingText))
            .Returns(expected)
            .Verifiable(Times.Once);
        mocks.MockStringDialogService
            .Setup(m => m.GetString(CloakingStringDialogTitle, CloakingStringDialogHeader))
            .Returns(cloakingText)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.OutputText = initialText;

        // Act
        viewModel.CloakCommand.Execute(null);

        // Assert
        viewModel.OutputText
            .Should()
            .Be(expected);
        mocks.VerifyAll();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\t \r\n")]
    public void CloakCommandExecute_ShouldNotApplyCloakIfCloakStringIsNullOrEmptyOrWhiteSpace(string? cloakText)
    {
        // Arrange
        string expected = "sample text";
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.ApplyCloak(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(It.IsAny<string>())
            .Verifiable(Times.Never);
        mocks.MockStringDialogService
            .Setup(m => m.GetString(CloakingStringDialogTitle, CloakingStringDialogHeader))
            .Returns(cloakText!)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.OutputText = expected;

        // Act
        viewModel.CloakCommand.Execute(null);

        // Assert
        viewModel.OutputText
            .Should()
            .Be(expected);
        mocks.VerifyAll();
    }

    [Fact]
    public void ConfigureCommandCanExecute_ShouldAlwaysReturnTrue()
    {
        // Arrange
        StandardMocks mocks = new();
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);

        // Act
        bool actual = viewModel.ConfigureCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeTrue();
        mocks.VerifyAll();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void ConfigureCommandExecute_ShouldSetIsConfiguredPropertyToFalseIfSeedValueIsNullOrEmpty(string? seedValue)
    {
        // Arrange
        EnigmaConfiguration configuration = new()
        {
            NumberOfRotors = 4,
            SeedValue = seedValue!
        };
        StandardMocks mocks = new();
        mocks.MockConfigurationDialogService
            .Setup(m => m.GetConfiguration(mocks.EnigmaConfiguration))
            .Returns(configuration)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks, configuration);
        viewModel.IsConfigured = true;
        viewModel.ConfigurationStatusText = ConfiguredStatusText;

        // Act
        viewModel.ConfigureCommand.Execute(null);

        // Assert
        viewModel.IsConfigured
            .Should()
            .BeFalse();
        viewModel.ConfigurationStatusText
            .Should()
            .Be(NotConfiguredStatusText);
        VerifyConfiguration(viewModel, configuration, mocks);
        mocks.VerifyAll();
    }

    [Fact]
    public void ConfigureCommandExecute_ShouldSetIsConfiguredPropertyToTrueIfSeedValueIsNotNullOrEmpty()
    {
        // Arrange
        EnigmaConfiguration configuration = new()
        {
            NumberOfRotors = 4,
            ReflectorIndex = 10,
            RotorIndex1 = 11,
            RotorIndex2 = 12,
            RotorIndex3 = 13,
            RotorIndex4 = 14,
            RotorIndex5 = 15,
            RotorIndex6 = 16,
            RotorIndex7 = 17,
            RotorIndex8 = 18,
            SeedValue = "seed value",
            UseEmbeddedConfiguration = true
        };
        StandardMocks mocks = new();
        mocks.MockConfigurationDialogService
            .Setup(m => m.GetConfiguration(mocks.EnigmaConfiguration))
            .Returns(configuration)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks, configuration);

        // Act
        viewModel.ConfigureCommand.Execute(null);

        // Assert
        viewModel.IsConfigured
            .Should()
            .BeTrue();
        viewModel.ConfigurationStatusText
            .Should()
            .Be(ConfiguredStatusText);
        VerifyConfiguration(viewModel, configuration, mocks);
        mocks.VerifyAll();
    }

    [Fact]
    public void DecloakCommandCanExecute_ShouldReturnFalseWhenInputTextIsNotCloaked()
    {
        // Arrange
        string inputText = "input text";
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(inputText))
            .Returns(false)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.InputText = inputText;

        // Act
        bool actual = viewModel.DecloakCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeFalse();
        mocks.VerifyAll();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\r \t\n")]
    public void DecloakCommandCanExecute_ShouldReturnFalseWhenInputTextIsNullOrEmptyOrWhiteSpace(string? inputText)
    {
        // Arrange
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(inputText!))
            .Returns(true)
            .Verifiable(Times.AtMost(1));
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.InputText = inputText!;

        // Act
        bool actual = viewModel.DecloakCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeFalse();
        mocks.VerifyAll();
    }

    [Fact]
    public void DecloakCommandCanExecute_ShouldReturnTrueWhenInputTextIsCloaked()
    {
        // Arrange
        string inputText = "input text";
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(inputText))
            .Returns(true)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.InputText = inputText;

        // Act
        bool actual = viewModel.DecloakCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeTrue();
        mocks.VerifyAll();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\t \r\n")]
    public void DecloakCommandExecute_ShouldNotRemoveCloakIfCloakStringIsNullOrEmptyOrWhiteSpace(string? cloakText)
    {
        // Arrange
        string expected = "sample text";
        StandardMocks mocks = new();
        mocks.MockStringDialogService
            .Setup(m => m.GetString(DecloakingStringDialogTitle, DecloakingStringDialogHeader))
            .Returns(cloakText!)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(true)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.RemoveCloak(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(It.IsAny<string>())
            .Verifiable(Times.Never);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.InputText = expected;

        // Act
        viewModel.DecloakCommand.Execute(null);

        // Assert
        viewModel.InputText
            .Should()
            .Be(expected);
        mocks.VerifyAll();
    }

    [Fact]
    public void DecloakCommandExecute_ShouldRemoveCloakAndExtractEmbeddedConfigurationIfPresent()
    {
        // Arrange
        string initialText = "cloaked text";
        string cloakText = "cloaking text";
        string decloakedText = "decloaked text";
        string expected = "final output text";
        EnigmaConfiguration? embeddedConfiguration = new()
        {
            NumberOfRotors = 5,
            ReflectorIndex = 30,
            RotorIndex1 = 31,
            RotorIndex2 = 32,
            RotorIndex3 = 33,
            RotorIndex4 = 34,
            RotorIndex5 = 35,
            SeedValue = "seed value",
            UseEmbeddedConfiguration = false
        };
        StandardMocks mocks = new();
        mocks.MockStringDialogService
            .Setup(m => m.GetString(DecloakingStringDialogTitle, DecloakingStringDialogHeader))
            .Returns(cloakText)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(initialText))
            .Returns(true)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.RemoveCloak(initialText, cloakText))
            .Returns(decloakedText)
            .Verifiable(Times.Once);
        mocks.MockEmbeddingService
            .Setup(m => m.HasIndicatorString(decloakedText))
            .Returns(true)
            .Verifiable(Times.Once);
        mocks.MockEmbeddingService
            .Setup(m => m.Extract(decloakedText, out embeddedConfiguration))
            .Returns(expected)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks, embeddedConfiguration);
        viewModel.InputText = initialText;
        viewModel.ReflectorIndex = 10;
        viewModel.RotorIndex1 = 11;
        viewModel.RotorIndex2 = 12;
        viewModel.RotorIndex3 = 13;
        viewModel.RotorIndex4 = 14;
        viewModel.RotorIndex5 = 15;
        viewModel.RotorIndex6 = 16;
        viewModel.RotorIndex7 = 17;
        viewModel.RotorIndex8 = 18;

        // Act
        viewModel.DecloakCommand.Execute(null);

        // Assert
        viewModel.InputText
            .Should()
            .Be(expected);
        VerifyConfiguration(viewModel, embeddedConfiguration, mocks);
        mocks.VerifyAll();
    }

    [Fact]
    public void DecloakCommandExecute_ShouldRemoveCloakIfCloakStringIsNotNullOrEmptyOrWhiteSpace()
    {
        // Arrange
        string initialText = "cloaked text";
        string expected = "decloaked text";
        string cloakText = "cloaking text";
        StandardMocks mocks = new();
        mocks.MockStringDialogService
            .Setup(m => m.GetString(DecloakingStringDialogTitle, DecloakingStringDialogHeader))
            .Returns(cloakText)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(initialText))
            .Returns(true)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.RemoveCloak(initialText, cloakText))
            .Returns(expected)
            .Verifiable(Times.Once);
        mocks.MockEmbeddingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .Setup(m => m.ResetCipherIndexes())
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.InputText = initialText;
        viewModel.ReflectorIndex = 10;
        viewModel.RotorIndex1 = 11;
        viewModel.RotorIndex2 = 12;
        viewModel.RotorIndex3 = 13;
        viewModel.RotorIndex4 = 14;
        viewModel.RotorIndex5 = 15;
        viewModel.RotorIndex6 = 16;
        viewModel.RotorIndex7 = 17;
        viewModel.RotorIndex8 = 18;

        // Act
        viewModel.DecloakCommand.Execute(null);

        // Assert
        viewModel.InputText
            .Should()
            .Be(expected);
        VerifyConfiguration(viewModel, mocks.EnigmaConfiguration);
        mocks.VerifyAll();
    }

    [Fact]
    public void InputText_ShouldCheckForCloakingIndicatorStringWhenValueChanges()
    {
        // Arrange
        string inputText = "input text";
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(inputText))
            .Returns(true)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);

        // Act
        viewModel.InputText = inputText;

        // Assert
        mocks.VerifyAll();
    }

    [Fact]
    public void LoadCommandCanExecute_ShouldAlwaysReturnTrue()
    {
        // Arrange
        StandardMocks mocks = new();
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);

        // Act
        bool actual = viewModel.LoadCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeTrue();
        mocks.VerifyAll();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void LoadCommandExecute_ShouldDoNothingIfInputTextIsNullOrEmpty(string? inputText)
    {
        // Arrange
        string expectedOutputText = "output text";
        string expectedInputText = "input text";
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(expectedInputText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(expectedOutputText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockInputOutputService
            .Setup(m => m.LoadTextFile())
            .Returns(inputText!)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.InputText = expectedInputText;
        viewModel.OutputText = expectedOutputText;

        // Act
        viewModel.LoadCommand.Execute(null);

        // Assert
        viewModel.InputText
            .Should()
            .Be(expectedInputText);
        viewModel.OutputText
            .Should()
            .Be(expectedOutputText);
        mocks.VerifyAll();
    }

    [Fact]
    public void LoadCommandExecute_ShouldExtractEmbeddedConfigurationIfPresent()
    {
        // Arrange
        string initialOutputText = "output text";
        string embeddedText = "embedded text";
        string expected = "input text";
        EnigmaConfiguration embeddedConfiguration = new()
        {
            NumberOfRotors = 6,
            ReflectorIndex = 40,
            RotorIndex1 = 41,
            RotorIndex2 = 42,
            RotorIndex3 = 43,
            RotorIndex4 = 44,
            RotorIndex5 = 45,
            RotorIndex6 = 46,
            SeedValue = "seed value",
            UseEmbeddedConfiguration = true
        };
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(initialOutputText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(string.Empty))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockEmbeddingService
            .Setup(m => m.HasIndicatorString(embeddedText))
            .Returns(true)
            .Verifiable(Times.Once);
        mocks.MockEmbeddingService
            .Setup(m => m.Extract(embeddedText, out embeddedConfiguration!))
            .Returns(expected)
            .Verifiable(Times.Once);
        mocks.MockInputOutputService
            .Setup(m => m.LoadTextFile())
            .Returns(embeddedText)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks, embeddedConfiguration);
        viewModel.OutputText = initialOutputText;

        // Act
        viewModel.LoadCommand.Execute(null);

        // Assert
        viewModel.InputText
            .Should()
            .Be(expected);
        viewModel.OutputText
            .Should()
            .BeEmpty();
        VerifyConfiguration(viewModel, embeddedConfiguration, mocks);
        mocks.VerifyAll();
    }

    [Fact]
    public void LoadCommandExecute_ShouldIgnoreNullEmbeddedConfiguration()
    {
        // Arrange
        string initialOutputText = "output text";
        string expected = "input text";
        EnigmaConfiguration? embeddedConfiguration = null;
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(initialOutputText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(string.Empty))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .Setup(m => m.ResetCipherIndexes())
            .Verifiable(Times.Once);
        mocks.MockEmbeddingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(true)
            .Verifiable(Times.Once);
        mocks.MockEmbeddingService
            .Setup(m => m.Extract(expected, out embeddedConfiguration))
            .Returns(expected)
            .Verifiable(Times.Once);
        mocks.MockInputOutputService
            .Setup(m => m.LoadTextFile())
            .Returns(expected)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.OutputText = initialOutputText;

        // Act
        viewModel.LoadCommand.Execute(null);

        // Assert
        viewModel.InputText
            .Should()
            .Be(expected);
        viewModel.OutputText
            .Should()
            .BeEmpty();
        mocks.VerifyAll();
    }

    [Fact]
    public void LoadCommandExecute_ShouldLoadInputTextIfItIsNotNullOrEmpty()
    {
        // Arrange
        string initialOutputText = "output text";
        string expected = "input text";
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(initialOutputText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(string.Empty))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .Setup(m => m.ResetCipherIndexes())
            .Verifiable(Times.Once);
        mocks.MockEmbeddingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockInputOutputService
            .Setup(m => m.LoadTextFile())
            .Returns(expected)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.OutputText = initialOutputText;

        // Act
        viewModel.LoadCommand.Execute(null);

        // Assert
        viewModel.InputText
            .Should()
            .Be(expected);
        viewModel.OutputText
            .Should()
            .BeEmpty();
        mocks.VerifyAll();
    }

    [Fact]
    public void MainWindowViewModel_ShouldInitializePropertiesCorrectlyWhenConstructed()
    {
        // Arrange
        StandardMocks mocks = new();

        // Act
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);

        // Assert
        viewModel.CloakCommand
            .Should()
            .NotBeNull();
        viewModel.ConfigurationStatusText
            .Should()
            .Be(NotConfiguredStatusText);
        viewModel.ConfigureCommand
            .Should()
            .NotBeNull();
        viewModel.DecloakCommand
            .Should()
            .NotBeNull();
        viewModel.EnigmaMachine
            .Should()
            .NotBeNull()
            .And
            .BeSameAs(mocks.EnigmaMachine);
        viewModel.InputText
            .Should()
            .BeEmpty();
        viewModel.IsConfigured
            .Should()
            .BeFalse();
        viewModel.LoadCommand
            .Should()
            .NotBeNull();
        viewModel.MoveCommand
            .Should()
            .NotBeNull();
        viewModel.OutputText
            .Should()
            .BeEmpty();
        viewModel.ResetCommand
            .Should()
            .NotBeNull();
        viewModel.SaveCommand
            .Should()
            .NotBeNull();
        viewModel.TransformCommand
            .Should()
            .NotBeNull();
        VerifyConfiguration(viewModel, mocks.EnigmaConfiguration);
        mocks.VerifyAll();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\r \t\n")]
    public void MoveCommandCanExecute_ShouldReturnFalseWhenOutputTextIsNullOrEmptyOrWhiteSpace(string? outputText)
    {
        // Arrange
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(outputText!))
            .Returns(true)
            .Verifiable(Times.AtMost(1));
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.OutputText = outputText!;

        // Act
        bool actual = viewModel.MoveCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeFalse();
        mocks.VerifyAll();
    }

    [Fact]
    public void MoveCommandCanExecute_ShouldReturnTrueWhenOutputTextIsNotNullOrEmptyOrWhiteSpace()
    {
        // Arrange
        string outputText = "output text";
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(outputText))
            .Returns(true)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.OutputText = outputText;

        // Act
        bool actual = viewModel.MoveCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeTrue();
        mocks.VerifyAll();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void MoveCommandExecute_ShouldDoNothingIfOutputTextIsNullOrEmpty(string? outputText)
    {
        // Arrange
        string expectedInputText = "input text";
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(expectedInputText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(outputText!))
            .Returns(false)
            .Verifiable(Times.AtMost(1));
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.InputText = expectedInputText;
        viewModel.OutputText = outputText!;

        // Act
        viewModel.MoveCommand.Execute(null);

        // Assert
        viewModel.InputText
            .Should()
            .Be(expectedInputText);
        viewModel.OutputText
            .Should()
            .Be(outputText);
        mocks.VerifyAll();
    }

    [Fact]
    public void MoveCommandExecute_ShouldExtractEmbeddedConfigurationIfPresent()
    {
        // Arrange
        string outputText = "output text";
        string initialInputText = "input text";
        string expected = "extracted input text";
        EnigmaConfiguration embeddedConfiguration = new()
        {
            NumberOfRotors = 6,
            ReflectorIndex = 40,
            RotorIndex1 = 41,
            RotorIndex2 = 42,
            RotorIndex3 = 43,
            RotorIndex4 = 44,
            RotorIndex5 = 45,
            RotorIndex6 = 46,
            SeedValue = "seed value",
            UseEmbeddedConfiguration = true
        };
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(initialInputText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(outputText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(string.Empty))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockEmbeddingService
            .Setup(m => m.HasIndicatorString(outputText))
            .Returns(true)
            .Verifiable(Times.Once);
        mocks.MockEmbeddingService
            .Setup(m => m.Extract(outputText, out embeddedConfiguration!))
            .Returns(expected)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks, embeddedConfiguration);
        viewModel.InputText = initialInputText;
        viewModel.OutputText = outputText;

        // Act
        viewModel.MoveCommand.Execute(null);

        // Assert
        viewModel.InputText
            .Should()
            .Be(expected);
        viewModel.OutputText
            .Should()
            .BeEmpty();
        VerifyConfiguration(viewModel, embeddedConfiguration, mocks);
        mocks.VerifyAll();
    }

    [Fact]
    public void MoveCommandExecute_ShouldIgnoreNullEmbeddedConfiguration()
    {
        // Arrange
        string expected = "output text";
        string initialInputText = "input text";
        EnigmaConfiguration? embeddedConfiguration = null;
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(initialInputText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(false)
            .Verifiable(Times.Exactly(2));
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(string.Empty))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .Setup(m => m.ResetCipherIndexes())
            .Verifiable(Times.Once);
        mocks.MockEmbeddingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(true)
            .Verifiable(Times.Once);
        mocks.MockEmbeddingService
            .Setup(m => m.Extract(expected, out embeddedConfiguration))
            .Returns(expected)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.InputText = initialInputText;
        viewModel.OutputText = expected;

        // Act
        viewModel.MoveCommand.Execute(null);

        // Assert
        viewModel.InputText
            .Should()
            .Be(expected);
        viewModel.OutputText
            .Should()
            .BeEmpty();
        mocks.VerifyAll();
    }

    [Fact]
    public void MoveCommandExecute_ShouldMoveOutputTextIfItIsNotNullOrEmpty()
    {
        // Arrange
        string expected = "output text";
        string initialInputText = "input text";
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(initialInputText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(false)
            .Verifiable(Times.Exactly(2));
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(string.Empty))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .Setup(m => m.ResetCipherIndexes())
            .Verifiable(Times.Once);
        mocks.MockEmbeddingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(false)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.InputText = initialInputText;
        viewModel.OutputText = expected;

        // Act
        viewModel.MoveCommand.Execute(null);

        // Assert
        viewModel.InputText
            .Should()
            .Be(expected);
        viewModel.OutputText
            .Should()
            .BeEmpty();
        mocks.VerifyAll();
    }

    [Fact]
    public void OutputText_ShouldCheckForCloakingIndicatorStringWhenValueChanges()
    {
        // Arrange
        string outputText = "output text";
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(outputText))
            .Returns(true)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);

        // Act
        viewModel.OutputText = outputText;

        // Assert
        mocks.VerifyAll();
    }

    [Fact]
    public void ResetCommandCanExecute_ShouldReturnFalseIfTransformNotExecuted()
    {
        // Arrange
        StandardMocks mocks = new();
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);

        // Act
        bool actual = viewModel.ResetCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeFalse();
        mocks.VerifyAll();
    }

    [Fact]
    public void ResetCommandCanExecute_ShouldReturnTrueIfTransformHasBeenExecuted()
    {
        // Arrange
        string inputText = "input text";
        string transformedText = "transformed text";
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(inputText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(transformedText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .Setup(m => m.Transform(inputText))
            .Returns(transformedText)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .SetupGet(m => m.Reflector!.CipherIndex)
            .Returns(10)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .SetupGet(m => m.Rotor1!.CipherIndex)
            .Returns(11)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .SetupGet(m => m.Rotor2!.CipherIndex)
            .Returns(12)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .SetupGet(m => m.Rotor3!.CipherIndex)
            .Returns(13)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.InputText = inputText;
        viewModel.TransformCommand.Execute(null);

        // Act
        bool actual = viewModel.ResetCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeTrue();
        mocks.VerifyAll();
    }

    [Fact]
    public void ResetCommandExecute_ShouldResetIndexesAndSetIsTransformExecutedToFalse()
    {
        // Arrange
        string inputText = "input text";
        string transformedText = "transformed text";
        EnigmaConfiguration configuration = new()
        {
            NumberOfRotors = 3,
            ReflectorIndex = 40,
            RotorIndex1 = 41,
            RotorIndex2 = 42,
            RotorIndex3 = 43,
            SeedValue = "seed value",
            UseEmbeddedConfiguration = false
        };
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(inputText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(transformedText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockConfigurationDialogService
            .Setup(m => m.GetConfiguration(mocks.EnigmaConfiguration))
            .Returns(configuration)
            .Verifiable(Times.Once);
        mocks.MockNewEnigmaMachine
            .Setup(m => m.Transform(inputText))
            .Returns(transformedText)
            .Verifiable(Times.Once);
        mocks.MockNewEnigmaMachine
            .Setup(m => m.ResetCipherIndexes())
            .Verifiable(Times.Once);
        mocks.MockNewEnigmaMachine
            .SetupGet(m => m.Reflector.CipherIndex)
            .Returns(20);
        mocks.MockNewEnigmaMachine
            .SetupGet(m => m.Rotor1!.CipherIndex)
            .Returns(21);
        mocks.MockNewEnigmaMachine
            .SetupGet(m => m.Rotor2!.CipherIndex)
            .Returns(22);
        mocks.MockNewEnigmaMachine
            .SetupGet(m => m.Rotor3!.CipherIndex)
            .Returns(23);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks, configuration);
        viewModel.ConfigureCommand.Execute(null);
        viewModel.InputText = inputText;
        viewModel.TransformCommand.Execute(null);

        // Act
        viewModel.ResetCommand.Execute(null);

        // Assert
        viewModel.ResetCommand.CanExecute(null)
            .Should()
            .BeFalse();
        VerifyConfiguration(viewModel, configuration, mocks);
        mocks.VerifyAll();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("\r \t\n")]
    public void SaveCommandCanExecute_ShouldReturnFalseWhenOutputTextIsNullOrEmptyOrWhiteSpace(string? outputText)
    {
        // Arrange
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(outputText!))
            .Returns(false)
            .Verifiable(Times.AtMost(1));
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.OutputText = outputText!;

        // Act
        bool actual = viewModel.SaveCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeFalse();
        mocks.VerifyAll();
    }

    [Fact]
    public void SaveCommandCanExecute_ShouldReturnTrueWhenOutputTextIsNotNullOrEmptyOrWhiteSpace()
    {
        // Arrange
        string outputText = "output text";
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(outputText))
            .Returns(false)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.OutputText = outputText;

        // Act
        bool actual = viewModel.SaveCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeTrue();
        mocks.VerifyAll();
    }

    [Fact]
    public void SaveCommandExecute_ShouldSaveOutputText()
    {
        // Arrange
        string outputText = "output text";
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(outputText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockInputOutputService
            .Setup(m => m.SaveTextFile(outputText))
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.OutputText = outputText;

        // Act
        viewModel.SaveCommand.Execute(null);

        // Assert
        viewModel.OutputText
            .Should()
            .Be(outputText);
        mocks.VerifyAll();
    }

    [Fact]
    public void TransformCommandCanExecute_ShouldReturnFalseIfEnigmaMachineNotConfigured()
    {
        // Arrange
        string inputText = "sample input text";
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(inputText))
            .Returns(false)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.InputText = inputText;

        // Act
        bool actual = viewModel.TransformCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeFalse();
        mocks.VerifyAll();
    }

    [Fact]
    public void TransformCommandCanExecute_ShouldReturnFalseIfInputTextIsCloaked()
    {
        // Arrange
        string inputText = "cloaked input text";
        EnigmaConfiguration configuration = new()
        {
            SeedValue = "sample seed value"
        };
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(inputText))
            .Returns(true)
            .Verifiable(Times.Once);
        mocks.MockConfigurationDialogService
            .Setup(m => m.GetConfiguration(mocks.EnigmaConfiguration))
            .Returns(configuration)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks, configuration);
        viewModel.ConfigureCommand.Execute(null);
        viewModel.InputText = inputText;

        // Act
        bool actual = viewModel.TransformCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeFalse();
        mocks.VerifyAll();
    }

    [Fact]
    public void TransformCommandCanExecute_ShouldReturnFalseIfInputTextIsEmpty()
    {
        // Arrange
        EnigmaConfiguration configuration = new()
        {
            SeedValue = "sample seed value"
        };
        StandardMocks mocks = new();
        mocks.MockConfigurationDialogService
            .Setup(m => m.GetConfiguration(mocks.EnigmaConfiguration))
            .Returns(configuration)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks, configuration);
        viewModel.ConfigureCommand.Execute(null);
        viewModel.InputText = string.Empty;

        // Act
        bool actual = viewModel.TransformCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeFalse();
        mocks.VerifyAll();
    }

    [Theory]
    [InlineData(null)]
    [InlineData(" \r\n\t")]
    public void TransformCommandCanExecute_ShouldReturnFalseIfInputTextIsNullOrWhiteSpace(string? inputText)
    {
        // Arrange
        EnigmaConfiguration configuration = new()
        {
            SeedValue = "sample seed value"
        };
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(inputText!))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockConfigurationDialogService
            .Setup(m => m.GetConfiguration(mocks.EnigmaConfiguration))
            .Returns(configuration)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks, configuration);
        viewModel.ConfigureCommand.Execute(null);
        viewModel.InputText = inputText!;

        // Act
        bool actual = viewModel.TransformCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeFalse();
        mocks.VerifyAll();
    }

    [Fact]
    public void TransformCommandCanExecute_ShouldReturnFalseIfTransformAlreadyExecuted()
    {
        // Arrange
        string initialString = "initial string";
        string transformedString = "transfromed string";
        EnigmaConfiguration configuration = new()
        {
            SeedValue = "sample seed value"
        };
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(initialString))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(transformedString))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockConfigurationDialogService
            .Setup(m => m.GetConfiguration(mocks.EnigmaConfiguration))
            .Returns(configuration)
            .Verifiable(Times.Once);
        mocks.MockNewEnigmaMachine
            .Setup(m => m.Transform(initialString))
            .Returns(transformedString)
            .Verifiable(Times.Once);
        mocks.MockNewEnigmaMachine
            .SetupGet(m => m.Rotor1!.CipherIndex)
            .Returns(1)
            .Verifiable(Times.Once);
        mocks.MockNewEnigmaMachine
            .SetupGet(m => m.Rotor2!.CipherIndex)
            .Returns(2)
            .Verifiable(Times.Once);
        mocks.MockNewEnigmaMachine
            .SetupGet(m => m.Rotor3!.CipherIndex)
            .Returns(3)
            .Verifiable(Times.Once);
        mocks.MockNewEnigmaMachine
            .SetupGet(m => m.Reflector!.CipherIndex)
            .Returns(4)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks, configuration);
        viewModel.ConfigureCommand.Execute(null);
        viewModel.InputText = initialString;
        viewModel.TransformCommand.Execute(null);

        // Act
        bool actual = viewModel.TransformCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeFalse();
        mocks.VerifyAll();
    }

    [Fact]
    public void TransformCommandCanExecute_ShouldReturnTrueIfEnigmaMachineIsConfiguredAndInputTextIsAvailableAndNotCloakedAndTransformNotExecuted()
    {
        // Arrange
        string inputText = "input text";
        EnigmaConfiguration configuration = new()
        {
            SeedValue = "sample seed value"
        };
        StandardMocks mocks = new();
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(inputText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockConfigurationDialogService
            .Setup(m => m.GetConfiguration(mocks.EnigmaConfiguration))
            .Returns(configuration)
            .Verifiable(Times.Once);
        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks, configuration);
        viewModel.ConfigureCommand.Execute(null);
        viewModel.InputText = inputText;

        // Act
        bool actual = viewModel.TransformCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeTrue();
        mocks.VerifyAll();
    }

    [Fact]
    public void TransformCommandExecute_ShouldEmbedConfigurationWhenSelected()
    {
        // Arrange
        string inputText = "inputText";
        string transformedText = "transformed text";
        string expected = "embedded text";
        int expectedReflectorIndex = 10;
        int expectedRotorIndex1 = 11;
        int expectedRotorIndex2 = 12;
        int expectedRotorIndex3 = 13;
        int notUsed = 0;
        StandardMocks mocks = new();
        mocks.MockEnigmaMachine
            .Setup(m => m.Transform(inputText))
            .Returns(transformedText)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(inputText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockEmbeddingService
            .Setup(m => m.Embed(transformedText, mocks.EnigmaConfiguration))
            .Returns(expected)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .SetupGet(m => m.Rotor1!.CipherIndex)
            .Returns(expectedRotorIndex1)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .SetupGet(m => m.Rotor2!.CipherIndex)
            .Returns(expectedRotorIndex2)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .SetupGet(m => m.Rotor3!.CipherIndex)
            .Returns(expectedRotorIndex3)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .SetupGet(m => m.Reflector!.CipherIndex)
            .Returns(expectedReflectorIndex)
            .Verifiable(Times.Once);

        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.UseEmbeddedConfiguration = true;
        viewModel.InputText = inputText;

        // Act
        viewModel.TransformCommand.Execute(null);

        // Assert
        viewModel.OutputText
            .Should()
            .Be(expected);
        viewModel.RotorIndex1
            .Should()
            .Be(expectedRotorIndex1);
        viewModel.RotorIndex2
            .Should()
            .Be(expectedRotorIndex2);
        viewModel.RotorIndex3
            .Should()
            .Be(expectedRotorIndex3);
        viewModel.RotorIndex4
            .Should()
            .Be(notUsed);
        viewModel.RotorIndex5
            .Should()
            .Be(notUsed);
        viewModel.RotorIndex6
            .Should()
            .Be(notUsed);
        viewModel.RotorIndex7
            .Should()
            .Be(notUsed);
        viewModel.RotorIndex8
            .Should()
            .Be(notUsed);
        viewModel.ReflectorIndex
            .Should()
            .Be(expectedReflectorIndex);
        mocks.VerifyAll();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void TransformCommandExecute_ShouldTransformTextAndUpdateCipherIndexValues(int numberOfRotors)
    {
        // Arrange
        string inputText = "inputText";
        string expected = "transformed text";
        int expectedReflectorIndex = 10;
        int expectedRotorIndex1 = 11;
        int expectedRotorIndex2 = 12;
        int expectedRotorIndex3 = 13;
        int expectedRotorIndex4 = 0;
        int expectedRotorIndex5 = 0;
        int expectedRotorIndex6 = 0;
        int expectedRotorIndex7 = 0;
        int expectedRotorIndex8 = 0;
        StandardMocks mocks = new();
        mocks.MockEnigmaMachine
            .Setup(m => m.Transform(inputText))
            .Returns(expected)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(inputText))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockCloakingService
            .Setup(m => m.HasIndicatorString(expected))
            .Returns(false)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .SetupGet(m => m.Rotor1!.CipherIndex)
            .Returns(expectedRotorIndex1)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .SetupGet(m => m.Rotor2!.CipherIndex)
            .Returns(expectedRotorIndex2)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .SetupGet(m => m.Rotor3!.CipherIndex)
            .Returns(expectedRotorIndex3)
            .Verifiable(Times.Once);
        mocks.MockEnigmaMachine
            .SetupGet(m => m.Reflector!.CipherIndex)
            .Returns(expectedReflectorIndex)
            .Verifiable(Times.Once);

        if (numberOfRotors > 3)
        {
            expectedRotorIndex4 = 14;
            mocks.MockEnigmaMachine
                .SetupGet(m => m.Rotor4!.CipherIndex)
                .Returns(expectedRotorIndex4)
                .Verifiable(Times.Once);
        }

        if (numberOfRotors > 4)
        {
            expectedRotorIndex5 = 15;
            mocks.MockEnigmaMachine
                .SetupGet(m => m.Rotor5!.CipherIndex)
                .Returns(15)
                .Verifiable(Times.Once);
        }

        if (numberOfRotors > 5)
        {
            expectedRotorIndex6 = 16;
            mocks.MockEnigmaMachine
                .SetupGet(m => m.Rotor6!.CipherIndex)
                .Returns(16)
                .Verifiable(Times.Once);
        }

        if (numberOfRotors > 6)
        {
            expectedRotorIndex7 = 17;
            mocks.MockEnigmaMachine
                .SetupGet(m => m.Rotor7!.CipherIndex)
                .Returns(17)
                .Verifiable(Times.Once);
        }

        if (numberOfRotors > 7)
        {
            expectedRotorIndex8 = 18;
            mocks.MockEnigmaMachine
                .SetupGet(m => m.Rotor8!.CipherIndex)
                .Returns(18)
                .Verifiable(Times.Once);
        }

        MainWindowViewModel viewModel = GetMainWindowViewModel(mocks);
        viewModel.NumberOfRotors = numberOfRotors;
        viewModel.InputText = inputText;

        // Act
        viewModel.TransformCommand.Execute(null);

        // Assert
        viewModel.OutputText
            .Should()
            .Be(expected);
        viewModel.RotorIndex1
            .Should()
            .Be(expectedRotorIndex1);
        viewModel.RotorIndex2
            .Should()
            .Be(expectedRotorIndex2);
        viewModel.RotorIndex3
            .Should()
            .Be(expectedRotorIndex3);
        viewModel.RotorIndex4
            .Should()
            .Be(expectedRotorIndex4);
        viewModel.RotorIndex5
            .Should()
            .Be(expectedRotorIndex5);
        viewModel.RotorIndex6
            .Should()
            .Be(expectedRotorIndex6);
        viewModel.RotorIndex7
            .Should()
            .Be(expectedRotorIndex7);
        viewModel.RotorIndex8
            .Should()
            .Be(expectedRotorIndex8);
        viewModel.ReflectorIndex
            .Should()
            .Be(expectedReflectorIndex);
        mocks.VerifyAll();
    }

    private static MainWindowViewModel GetMainWindowViewModel(StandardMocks mocks, EnigmaConfiguration? newConfiguration = null)
    {
        mocks.MockEnigmaMachineBuilder
            .Setup(m => m.Build(mocks.EnigmaConfiguration))
            .Returns(mocks.EnigmaMachine)
            .Verifiable(Times.Once());

        if (newConfiguration is not null)
        {
            mocks.MockEnigmaMachineBuilder
                .Setup(m => m.Build(newConfiguration))
                .Returns(mocks.NewEnigmaMachine)
                .Verifiable(Times.Once());
        }

        return new(mocks.CloakingService,
                   mocks.ConfigurationDialogService,
                   mocks.EmbeddingService,
                   mocks.EnigmaMachineBuilder,
                   mocks.InputOutputService,
                   mocks.StringDialogService);
    }

    private static void VerifyConfiguration(MainWindowViewModel viewModel, EnigmaConfiguration configuration, StandardMocks? mocks = null)
    {
        if (mocks is not null)
        {
            viewModel.EnigmaMachine
                .Should()
                .Be(mocks.NewEnigmaMachine);
        }

        viewModel.NumberOfRotors
            .Should()
            .Be(configuration.NumberOfRotors);
        viewModel.ReflectorIndex
            .Should()
            .Be(configuration.ReflectorIndex);
        viewModel.RotorIndex1
            .Should()
            .Be(configuration.RotorIndex1);
        viewModel.RotorIndex2
            .Should()
            .Be(configuration.RotorIndex2);
        viewModel.RotorIndex3
            .Should()
            .Be(configuration.RotorIndex3);
        viewModel.RotorIndex4
            .Should()
            .Be(configuration.NumberOfRotors < 4 ? 0 : configuration.RotorIndex4);
        viewModel.RotorIndex5
            .Should()
            .Be(configuration.NumberOfRotors < 5 ? 0 : configuration.RotorIndex5);
        viewModel.RotorIndex6
            .Should()
            .Be(configuration.NumberOfRotors < 6 ? 0 : configuration.RotorIndex6);
        viewModel.RotorIndex7
            .Should()
            .Be(configuration.NumberOfRotors < 7 ? 0 : configuration.RotorIndex7);
        viewModel.RotorIndex8
            .Should()
            .Be(configuration.NumberOfRotors < 8 ? 0 : configuration.RotorIndex8);
        viewModel.IsRotor4Visible
            .Should()
            .Be(configuration.NumberOfRotors > 3);
        viewModel.IsRotor5Visible
            .Should()
            .Be(configuration.NumberOfRotors > 4);
        viewModel.IsRotor6Visible
            .Should()
            .Be(configuration.NumberOfRotors > 5);
        viewModel.IsRotor7Visible
            .Should()
            .Be(configuration.NumberOfRotors > 6);
        viewModel.IsRotor8Visible
            .Should()
            .Be(configuration.NumberOfRotors > 7);
        viewModel.UseEmbeddedConfiguration
            .Should()
            .Be(configuration.UseEmbeddedConfiguration);
    }
}

[ExcludeFromCodeCoverage]
public class StandardMocks
{
    public ICloakingService CloakingService => MockCloakingService.Object;

    public IConfigurationDialogService ConfigurationDialogService => MockConfigurationDialogService.Object;

    public IEmbeddingService EmbeddingService => MockEmbeddingService.Object;

    public EnigmaConfiguration EnigmaConfiguration
    {
        get;
    } = new();

    public IEnigmaMachine EnigmaMachine => MockEnigmaMachine.Object;

    public IEnigmaMachineBuilder EnigmaMachineBuilder => MockEnigmaMachineBuilder.Object;

    public IInputOutputService InputOutputService => MockInputOutputService.Object;

    public Mock<ICloakingService> MockCloakingService
    {
        get;
    } = new(MockBehavior.Strict);

    public Mock<IConfigurationDialogService> MockConfigurationDialogService
    {
        get;
    } = new(MockBehavior.Strict);

    public Mock<IEmbeddingService> MockEmbeddingService
    {
        get;
    } = new(MockBehavior.Strict);

    public Mock<IEnigmaMachine> MockEnigmaMachine
    {
        get;
    } = new(MockBehavior.Strict);

    public Mock<IEnigmaMachineBuilder> MockEnigmaMachineBuilder
    {
        get;
    } = new(MockBehavior.Strict);

    public Mock<IInputOutputService> MockInputOutputService
    {
        get;
    } = new(MockBehavior.Strict);

    public Mock<IEnigmaMachine> MockNewEnigmaMachine
    {
        get;
    } = new(MockBehavior.Strict);

    public Mock<IStringDialogService> MockStringDialogService
    {
        get;
    } = new(MockBehavior.Strict);

    public IEnigmaMachine NewEnigmaMachine => MockNewEnigmaMachine.Object;

    public IStringDialogService StringDialogService => MockStringDialogService.Object;

    public void VerifyAll()
    {
        MockCloakingService.VerifyAll();
        MockConfigurationDialogService.VerifyAll();
        MockEmbeddingService.VerifyAll();
        MockEnigmaMachine.VerifyAll();
        MockEnigmaMachineBuilder.VerifyAll();
        MockInputOutputService.VerifyAll();
        MockNewEnigmaMachine.VerifyAll();
        MockStringDialogService.VerifyAll();
    }
}