namespace DRSSoftware.EnigmaMachine.Utility;

using System.Windows;
using DRSSoftware.DRSBasicDI.Interfaces;
using DRSSoftware.EnigmaMachine.ViewModels;
using DRSSoftware.EnigmaMachine.Views;

[ExcludeFromCodeCoverage]
public class StringDialogServiceTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void GetString_ShouldReturnStringRegardlessOfDialogResult(bool dialogResult)
    {
        // Arrange
        string title = "Title";
        string headerText = "Header";
        string expected = "sample text";
        Mock<IStringDialogViewModel> mockViewModel = new(MockBehavior.Strict);
        mockViewModel
            .SetupSet(m => m.Title = title)
            .Verifiable(Times.Once);
        mockViewModel
            .SetupSet(m => m.HeaderText = headerText)
            .Verifiable(Times.Once);
        mockViewModel
            .Setup(m => m.InputText)
            .Returns(expected)
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
            .Setup(m => m.Resolve<IDialogView>(StringDialogKey))
            .Returns(mockView.Object)
            .Verifiable(Times.Once);
        mockContainer
            .Setup(m => m.Resolve<IStringDialogViewModel>())
            .Returns(mockViewModel.Object)
            .Verifiable(Times.Once);
        StringDialogService stringDialogService = new(mockContainer.Object);

        // Act
        string actual = stringDialogService.GetString(title, headerText);

        // Assert
        actual
            .Should()
            .Be(expected);
        mockViewModel.VerifyAll();
        mockView.VerifyAll();
        mockContainer.VerifyAll();
    }
}