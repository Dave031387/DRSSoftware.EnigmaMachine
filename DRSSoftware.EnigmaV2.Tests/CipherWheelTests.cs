namespace DRSSoftware.EnigmaV2;

public class CipherWheelTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(TableSize)]
    public void DisplaceIndexWhenIndexOutOfRange_ShouldThrowException(int index)
    {
        // Arrange/Act
        Action action = () => CipherWheel.DisplaceIndex(index, ' ');
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
        int actual = CipherWheel.DisplaceIndex(index, seedChar);

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
        int actual = CipherWheel.DisplaceIndex(index, seedChar);

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
        Action action = () => CipherWheel.FindAvailableSlot(startIndex, slotIsTaken);

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
        int actual = CipherWheel.FindAvailableSlot(startIndex, slotIsTaken);

        // Assert
        actual
            .Should()
            .Be(expected);
        slotIsTaken[expected]
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(0, 0, 12)]
    [InlineData(1, 1, 13)]
    [InlineData(5, 10, 15)]
    [InlineData(12, 6, 7)]
    [InlineData(19, 12, 0)]
    [InlineData(10, 2, 1)]
    [InlineData(8, 19, 10)]
    [InlineData(0, 6, 19)]
    [InlineData(16, 4, 4)]
    public void GetTransformedValue_ShouldReturnExpectedValue(int baseValue, int cipherIndex, int expected)
    {
        // Arrange
        MyCipherWheel myCipherWheel = new(13)
        {
            _cipherIndex = cipherIndex
        };

        int[] table = [12, 4, 9, 18, 10, 16, 1, 8, 19, 11, 2, 17, 0, 3, 13, 5, 14, 6, 15, 7];

        // Act
        int actual = myCipherWheel.GetTransformedValue(table, baseValue);

        // Assert
        actual
            .Should()
            .Be(expected);
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
    public void GetValueWithOffset_ShouldReturnExpectedValue(int baseValue, int offset, int expected)
    {
        // Arrange/Act
        int actual = CipherWheel.GetValueWithOffset(baseValue, 50, offset);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(0, 0, 0, 0, 0)]
    [InlineData(2, 5, 0, 2, 5)]
    [InlineData(3, 2, 1, 4, 0)]
    [InlineData(TableSize - 1, 1, 1, 0, 0)]
    [InlineData(5, 0, 2, 5, 1)]
    [InlineData(5, 1, 2, 6, 0)]
    [InlineData(10, 11, 13, 10, 12)]
    [InlineData(TableSize - 1, 12, 13, 0, 0)]
    public void Rotate_ShouldReturnCorrectIndexAndCycleCount(int indexValue, int cycleCount, int cycleSize, int expectedIndexValue, int expectedCycleCount)
    {
        // Arrange/Act
        MyCipherWheel myCipherWheel = new(cycleSize)
        {
            _cipherIndex = indexValue,
            _cycleCount = cycleCount,
            _isInitialized = true
        };

        // Act
        myCipherWheel.Rotate();

        // Assert
        myCipherWheel._cipherIndex
            .Should()
            .Be(expectedIndexValue);
        myCipherWheel._cycleCount
            .Should()
            .Be(expectedCycleCount);
    }

    [Fact]
    public void SetIndexWhenRotorNotInitialized_ShouldThrowException()
    {
        // Arrange
        MyCipherWheel myCipherWheel = new(1);
        string expected = "The cipher wheel must be initialized before the cipher index can be set.";

        // Act
        Action action = () => myCipherWheel.SetIndex(5);

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
    }

    [Theory]
    [InlineData(-5)]
    [InlineData(-1)]
    [InlineData(TableSize)]
    [InlineData(TableSize + 10)]
    public void SetIndexWhenValueIsOutOfRange_ShouldThrowException(int value)
    {
        // Arrange
        MyCipherWheel myCipherWheel = new(1)
        {
            _isInitialized = true
        };
        string expected = $"The value passed into the SetIndex method must be greater than or equal to zero and less than {TableSize}, but it was {value}. (Parameter 'indexValue')";

        // Act
        Action action = () => myCipherWheel.SetIndex(value);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>()
            .WithMessage(expected);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(0, 10, 0)]
    [InlineData(1, 0, 0)]
    [InlineData(1, 1, 0)]
    [InlineData(1, 10, 0)]
    [InlineData(2, 0, 0)]
    [InlineData(2, 5, 1)]
    [InlineData(2, 1, 0)]
    [InlineData(2, 10, 0)]
    [InlineData(5, 0, 0)]
    [InlineData(5, 1, 0)]
    [InlineData(5, 2, 2)]
    [InlineData(5, 7, 2)]
    [InlineData(5, 4, 4)]
    [InlineData(5, 25, 0)]
    [InlineData(8, 4, 4)]
    [InlineData(8, 7, 7)]
    [InlineData(8, 32, 0)]
    [InlineData(8, 45, 5)]
    public void SetIndexWithDifferentCycleSizes_ShouldCorrectlyInitializeCipherIndexAndCycleCount(int cycleSize, int expectedIndexValue, int expectedCycleCount)
    {
        // Arrange
        MyCipherWheel myCipherWheel = new(cycleSize)
        {
            _isInitialized = true
        };

        // Act
        myCipherWheel.SetIndex(expectedIndexValue);

        // Assert
        myCipherWheel._cipherIndex
            .Should()
            .Be(expectedIndexValue);
        myCipherWheel._cycleCount
            .Should()
            .Be(expectedCycleCount);
    }
}

internal sealed class MyCipherWheel(int cycleSize) : CipherWheel(cycleSize)
{
    public override void Initialize(string seed) => throw new NotImplementedException();

    public override int Transform(int c) => throw new NotImplementedException();
}