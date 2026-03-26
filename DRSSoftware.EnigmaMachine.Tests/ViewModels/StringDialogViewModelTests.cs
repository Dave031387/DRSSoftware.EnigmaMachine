namespace DRSSoftware.EnigmaMachine.ViewModels;

[ExcludeFromCodeCoverage]
public class StringDialogViewModelTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("test")]
    [InlineData("123456789")]
    public void AcceptCommandCanCanExecute_ShouldReturnFalseIfInputTextIsNullOrEmptyOrInvalidLength(string? inputText)
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
    public void AcceptCommandCanExecute_ShouldReturnTrueIfInputTextIsValidLength()
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
    public void AcceptCommandExecute_ShouldSetCloseTriggerPropertyToTrue()
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

    [Fact]
    public void CancelCommandCanExecute_ShouldAlwaysReturnTrue()
    {
        // Arrange
        StringDialogViewModel viewModel = new();

        // Act
        bool actual = viewModel.CancelCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeTrue();
    }

    [Fact]
    public void CancelCommandExecute_ShouldClearInputTextAndSetCloseTriggerToTrue()
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