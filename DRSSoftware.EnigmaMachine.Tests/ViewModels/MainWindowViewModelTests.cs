namespace DRSSoftware.EnigmaMachine.ViewModels;

using DRSSoftware.EnigmaMachine.Utility;

public class MainWindowViewModelTests
{
    [Fact]
    public void MainWindowViewModel_ShouldInitializePropertiesCorrectlyWhenConstructed()
    {
        // Arrange
        Mock<ICloakingService> mockCloakingService = new(MockBehavior.Strict);
        Mock<IConfigurationDialogService> mockConfigurationDialogService = new(MockBehavior.Strict);
        Mock<IEmbeddingService> mockEmbeddingService = new(MockBehavior.Strict);
        Mock<IEnigmaMachine> mockEnigmaMachine = new(MockBehavior.Strict);
        Mock<IEnigmaMachineBuilder> mockEnigmaMachineBuilder = new(MockBehavior.Strict);
        mockEnigmaMachineBuilder
            .Setup(m => m.Build(new()))
            .Returns(mockEnigmaMachine.Object)
            .Verifiable(Times.Once());
        Mock<IInputOutputService> mockInputOutputService = new(MockBehavior.Strict);
        Mock<IStringDialogService> mockStringDialogService = new(MockBehavior.Strict);

        // Act
        MainWindowViewModel viewModel = new(mockCloakingService.Object,
                                            mockConfigurationDialogService.Object,
                                            mockEmbeddingService.Object,
                                            mockEnigmaMachineBuilder.Object,
                                            mockInputOutputService.Object,
                                            mockStringDialogService.Object);

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
            .BeSameAs(mockEnigmaMachine.Object);
        viewModel.InputText
            .Should()
            .BeEmpty();
        viewModel.IsConfigured
            .Should()
            .BeFalse();
        viewModel.IsRotor4Visible
            .Should()
            .BeFalse();
        viewModel.IsRotor5Visible
            .Should()
            .BeFalse();
        viewModel.IsRotor6Visible
            .Should()
            .BeFalse();
        viewModel.IsRotor7Visible
            .Should()
            .BeFalse();
        viewModel.IsRotor8Visible
            .Should()
            .BeFalse();
        viewModel.LoadCommand
            .Should()
            .NotBeNull();
        viewModel.MoveCommand
            .Should()
            .NotBeNull();
        viewModel.NumberOfRotors
            .Should()
            .Be(MinRotorCount);
        viewModel.OutputText
            .Should()
            .BeEmpty();
        viewModel.ReflectorIndex
            .Should()
            .Be(0);
        viewModel.ResetCommand
            .Should()
            .NotBeNull();
        viewModel.RotorIndex1
            .Should()
            .Be(0);
        viewModel.RotorIndex2
            .Should()
            .Be(0);
        viewModel.RotorIndex3
            .Should()
            .Be(0);
        viewModel.RotorIndex4
            .Should()
            .Be(0);
        viewModel.RotorIndex5
            .Should()
            .Be(0);
        viewModel.RotorIndex6
            .Should()
            .Be(0);
        viewModel.RotorIndex7
            .Should()
            .Be(0);
        viewModel.RotorIndex8
            .Should()
            .Be(0);
        viewModel.SaveCommand
            .Should()
            .NotBeNull();
        viewModel.TransformCommand
            .Should()
            .NotBeNull();
        viewModel.UseEmbeddedConfiguration
            .Should()
            .BeFalse();
        VerifyMocks(mockCloakingService,
                    mockConfigurationDialogService,
                    mockEmbeddingService,
                    mockEnigmaMachine,
                    mockEnigmaMachineBuilder,
                    mockInputOutputService,
                    mockStringDialogService);
    }

    private static void VerifyMocks(Mock<ICloakingService> mockCloakingService,
                                    Mock<IConfigurationDialogService> mockConfigurationDialogService,
                                    Mock<IEmbeddingService> mockEmbeddingService,
                                    Mock<IEnigmaMachine> mockEnigmaMachine,
                                    Mock<IEnigmaMachineBuilder> mockEnigmaMachineBuilder,
                                    Mock<IInputOutputService> mockInputOutputService,
                                    Mock<IStringDialogService> mockStringDialogService)
    {
        mockCloakingService.VerifyAll();
        mockConfigurationDialogService.VerifyAll();
        mockEmbeddingService.VerifyAll();
        mockEnigmaMachine.VerifyAll();
        mockEnigmaMachineBuilder.VerifyAll();
        mockInputOutputService.VerifyAll();
        mockStringDialogService.VerifyAll();
    }
}