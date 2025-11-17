namespace DRSSoftware.EnigmaV2;

public class CommonTests
{
    [Theory]
    [InlineData(' ', 0)]
    [InlineData('+', 11)]
    [InlineData('U', 53)]
    [InlineData('~', 94)]
    public void CharToInt_ShouldReturnCorrectValue(char c, int expected)
    {
        // Arrange/Act
        int actual = CharToInt(c);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(0, ' ')]
    [InlineData(10, '*')]
    [InlineData(59, '[')]
    [InlineData(81, 'q')]
    [InlineData(94, '~')]
    public void IntToChar_ShouldReturnExpectedCharacter(int value, char expected)
    {
        // Arrange/Act
        char actual = IntToChar(value);

        // Assert
        actual
            .Should()
            .Be(expected);
    }
}