namespace DRSSoftware.EnigmaMachine.Utility;

using System.Windows;
using DRSSoftware.DRSBasicDI.Interfaces;
using DRSSoftware.EnigmaMachine.ViewModels;
using DRSSoftware.EnigmaMachine.Views;

public class ConfigurationDialogServiceTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetConfiguration_ShouldReturnConfigurationRegardlessOfDialogResult(bool dialogResult)
    {
        // Arrange
        EnigmaConfiguration oldConfiguration = new()
        {
            NumberOfRotors = 3,
            ReflectorIndex = 1,
            RotorIndex1 = 2,
            RotorIndex2 = 3,
            RotorIndex3 = 4,
            SeedValue = "Old Seed Value",
            UseEmbeddedConfiguration = true
        };
        EnigmaConfiguration newConfiguration = new()
        {
            NumberOfRotors = 4,
            ReflectorIndex = 11,
            RotorIndex1 = 12,
            RotorIndex2 = 13,
            RotorIndex3 = 14,
            RotorIndex4 = 15,
            SeedValue = "New Seed Value",
            UseEmbeddedConfiguration = false
        };
        Mock<IConfigurationDialogViewModel> mockViewModel = new(MockBehavior.Strict);
        mockViewModel
            .Setup(m => m.Initialize(oldConfiguration))
            .Verifiable(Times.Once);
        mockViewModel
            .SetupGet(m => m.EnigmaConfiguration)
            .Returns(newConfiguration)
            .Verifiable(Times.Once);
        Mock<IDialogView> mockView = new(MockBehavior.Strict);
        mockView
            .SetupSet(m => m.WindowStartupLocation = WindowStartupLocation.CenterOwner)
            .Verifiable(Times.Once);
        mockView
            .SetupSet(m => m.DataContext = mockViewModel.Object)
            .Verifiable(Times.Once);
        mockView
            .Setup(m => m.ShowDialog())
            .Returns(dialogResult)
            .Verifiable(Times.Once);
        Mock<IContainer> mockContainer = new(MockBehavior.Strict);
        mockContainer
            .Setup(m => m.Resolve<IDialogView>(ConfigurationDialogKey))
            .Returns(mockView.Object)
            .Verifiable(Times.Once);
        mockContainer
            .Setup(m => m.Resolve<IConfigurationDialogViewModel>())
            .Returns(mockViewModel.Object)
            .Verifiable(Times.Once);
        ConfigurationDialogService configurationDialogService = new(mockContainer.Object);

        // Act
        EnigmaConfiguration actual = configurationDialogService.GetConfiguration(oldConfiguration);

        // Assert
        actual
            .Should()
            .BeEquivalentTo(newConfiguration);
        mockContainer.VerifyAll();
        mockView.VerifyAll();
        mockViewModel.VerifyAll();
    }
}