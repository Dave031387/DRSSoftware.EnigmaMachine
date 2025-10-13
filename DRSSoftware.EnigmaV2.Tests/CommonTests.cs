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
    [InlineData(-1)]
    [InlineData(TableSize)]
    public void DisplaceIndexWhenIndexOutOfRange_ShouldThrowException(int index)
    {
        // Arrange/Act
        Action action = () => DisplaceIndex(index, ' ');
        string expected = $"The index passed into the DisplaceIndex method must be greater than or equal to zero and less than {TableSize}, but it was {index}. (Parameter '{nameof(index)}')";

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>()
            .WithMessage(expected);
    }

    [Theory]
    [InlineData(MinChar + 1, 11)]
    [InlineData(MinChar + 2, 12)]
    [InlineData(MinChar + 44, 54)]
    [InlineData(MaxChar, 9)]
    [InlineData(MaxChar - 1, 8)]
    [InlineData(MaxChar - 37, 68)]
    public void DisplaceIndexWhenSeedCharIsInRangeAndNotMinChar_ShouldReturnCorrectIndexValue(char seedChar, int expected)
    {
        // Arrange
        int index = 10;

        // Act
        int actual = DisplaceIndex(index, seedChar);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(MinChar)]
    [InlineData(MinChar - 1)]
    [InlineData(MaxChar + 1)]
    [InlineData(MinChar - 10)]
    [InlineData(MaxChar + 10)]
    public void DisplaceIndexWhenSeedCharIsMinCharOrOutOfRange_ShouldDisplaceByOne(char seedChar)
    {
        // Arrange
        int index = 10;
        int expected = index + 1;

        // Act
        int actual = DisplaceIndex(index, seedChar);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void FindAvailableSlotWhenNoneAreAvailable_ShouldThrowException()
    {
        // Arrange
        bool[] slotIsTaken = new bool[TableSize];
        int startIndex = 32;
        string expected = "The FindAvailableSlot method is unable to find an available slot.";

        for (int i = 0; i < TableSize; i++)
        {
            slotIsTaken[i] = true;
        }

        // Act
        Action action = () => FindAvailableSlot(startIndex, slotIsTaken);

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(10, 10)]
    [InlineData(11, 32)]
    [InlineData(36, 44)]
    [InlineData(44, 44)]
    [InlineData(45, 10)]
    public void FindAvailableSlotWhenSlotsAreAvailable_ShouldReturnIndexToNextAvailableSlot(int startIndex, int expected)
    {
        // Arrange
        bool[] slotIsTaken = new bool[TableSize];

        for (int i = 0; i < TableSize; i++)
        {
            // In this test only slots 10, 32, and 44 are available. All other slots are marked as
            // taken.
            slotIsTaken[i] = i is not 10 and not 32 and not 44;
        }

        // Act
        int actual = FindAvailableSlot(startIndex, slotIsTaken);

        // Assert
        actual
            .Should()
            .Be(expected);
        slotIsTaken[expected]
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(0, 10, 10)]
    [InlineData(0, -3, 47)]
    [InlineData(-10, 5, 45)]
    [InlineData(10, -10, 0)]
    [InlineData(5, -15, 40)]
    [InlineData(40, 9, 49)]
    [InlineData(-33, 33, 0)]
    [InlineData(74, -25, 49)]
    [InlineData(45, 12, 7)]
    [InlineData(0, 49, 49)]
    [InlineData(57, 13, 20)]
    public void GetIndex_ShouldReturnExpectedIndexValue(int indexBase, int offset, int expected)
    {
        // Arrange/Act
        int actual = GetIndex(indexBase, 50, offset);

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