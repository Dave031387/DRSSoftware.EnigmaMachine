namespace DRSSoftware.EnigmaMachine.ViewModels;

public class StringDialogViewModelTests
{
    [Fact]
    public void AcceptCommand_ShouldSetCloseTriggerPropertyToTrue()
    {
        // Arrange
        string expected = "1234567890";
        StringDialogViewModel viewModel = new()
        {
            InputText = expected
        };

        // Act
        viewModel.AcceptCommand.Execute(null);

        // Assert
        viewModel.InputText
            .Should()
            .Be(expected);
        viewModel.CloseTrigger
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("test")]
    [InlineData("123456789")]
    public void CanAccept_ShouldReturnFalseIfInputTextIsNullOrEmptyOrInvalidLength(string? inputText)
    {
        // Arrange/Act
        StringDialogViewModel viewModel = new()
        {
            InputText = inputText!
        };

        // Assert
        viewModel.AcceptCommand.CanExecute(null)
            .Should()
            .BeFalse();
    }

    [Fact]
    public void CanAccept_ShouldReturnTrueIfInputTextIsValidLength()
    {
        // Arrange/Act
        StringDialogViewModel viewModel = new()
        {
            InputText = "1234567890"
        };

        // Assert
        viewModel.AcceptCommand.CanExecute(null)
            .Should()
            .BeTrue();
    }

    [Fact]
    public void CancelCommand_ShouldClearInputTextAndSetCloseTriggerToTrue()
    {
        // Arrange
        StringDialogViewModel viewModel = new()
        {
            InputText = "1234567890"
        };

        // Act
        viewModel.CancelCommand.Execute(null);

        // Assert
        viewModel.InputText
            .Should()
            .BeEmpty();
        viewModel.CloseTrigger
            .Should()
            .BeTrue();
    }

    [Fact]
    public void StringDialogViewModel_ShouldInitializePropertiesCorrectlyWhenConstructed()
    {
        // Arrange/Act
        StringDialogViewModel viewModel = new();

        // Assert
        viewModel.AcceptCommand
            .Should()
            .NotBeNull();
        viewModel.CancelCommand
            .Should()
            .NotBeNull();
        viewModel.CloseTrigger
            .Should()
            .BeFalse();
        viewModel.HeaderText
            .Should()
            .BeEmpty();
        viewModel.InputText
            .Should()
            .BeEmpty();
        viewModel.Title
            .Should()
            .BeEmpty();
    }
}