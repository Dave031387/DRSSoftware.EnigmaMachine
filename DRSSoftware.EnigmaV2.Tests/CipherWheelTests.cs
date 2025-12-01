namespace DRSSoftware.EnigmaV2;

using System.Diagnostics.CodeAnalysis;

public class CipherWheelTests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(TableSize)]
    public void DisplaceIndexWhenIndexOutOfRange_ShouldThrowException(int index)
    {
        // Arrange
        MyCipherWheel cipherWheel = new(1);

        // Act
        Action action = () => cipherWheel.TestDisplaceIndex(index, ' ');
        string expected = $"The index value passed into the DisplaceIndex method of the MyCipherWheel class must be greater than or equal to 0 and less than {TableSize}, but it was {index}. (Parameter '{nameof(index)}')";

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
        MyCipherWheel cipherWheel = new(1);
        int index = 10;

        // Act
        int actual = cipherWheel.TestDisplaceIndex(index, seedChar);

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
        MyCipherWheel cipherWheel = new(1);
        int index = 10;
        int expected = index + 1;

        // Act
        int actual = cipherWheel.TestDisplaceIndex(index, seedChar);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void FindAvailableConnectionPointWhenNoneAreAvailable_ShouldThrowException()
    {
        // Arrange
        MyCipherWheel cipherWheel = new(1);
        bool[] slotIsTaken = new bool[TableSize];
        int startIndex = 32;
        string expected = "The FindAvailableConnectionPoint method of the MyCipherWheel class is unable to find an available connection point.";

        for (int i = 0; i < TableSize; i++)
        {
            slotIsTaken[i] = true;
        }

        // Act
        Action action = () => cipherWheel.TestFindAvailableSlot(startIndex, slotIsTaken);

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
    public void FindAvailableConnectionPointWhenSomeAreAvailable_ShouldReturnIndexToNextAvailableSlot(int startIndex, int expected)
    {
        // Arrange
        MyCipherWheel cipherWheel = new(1);
        bool[] slotIsTaken = new bool[TableSize];

        for (int i = 0; i < TableSize; i++)
        {
            // In this test only slots 10, 32, and 44 are available. All other slots are marked as
            // taken.
            slotIsTaken[i] = i is not 10 and not 32 and not 44;
        }

        // Act
        int actual = cipherWheel.TestFindAvailableSlot(startIndex, slotIsTaken);

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
    public void GetIndexValueWithOffset_ShouldReturnExpectedValue(int baseValue, int offset, int expected)
    {
        // Arrange/Act
        int actual = MyCipherWheel.TestGetValueWithOffset(baseValue, 50, offset);

        // Assert
        actual
            .Should()
            .Be(expected);
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
        MyCipherWheel myCipherWheel = new(13);
        myCipherWheel.SetState(cipherIndex, null, null);
        int[] table = [12, 4, 9, 18, 10, 16, 1, 8, 19, 11, 2, 17, 0, 3, 13, 5, 14, 6, 15, 7];

        // Act
        int actual = myCipherWheel.TestGetTransformedValue(table, baseValue);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Theory]
    [InlineData(0, 0, 0, 0, 0)]
    [InlineData(2, 5, 0, 2, 5)]
    [InlineData(3, 0, 1, 4, 0)]
    [InlineData(TableSize - 1, 0, 1, 0, 0)]
    [InlineData(5, 0, 2, 5, 1)]
    [InlineData(5, 1, 2, 6, 0)]
    [InlineData(10, 11, 13, 10, 12)]
    [InlineData(TableSize - 1, 12, 13, 0, 0)]
    public void Rotate_ShouldReturnCorrectIndexAndCycleCount(int indexValue, int cycleCount, int cycleSize, int expectedIndexValue, int expectedCycleCount)
    {
        // Arrange/Act
        MyCipherWheel myCipherWheel = new(cycleSize);
        myCipherWheel.SetState(indexValue, cycleCount, true);

        // Act
        myCipherWheel.TestRotate();

        // Assert
        myCipherWheel.CipherIndex
            .Should()
            .Be(expectedIndexValue);
        myCipherWheel.CycleCount
            .Should()
            .Be(expectedCycleCount);
    }

    [Fact]
    public void SetCipherIndexWhenRotorNotInitialized_ShouldThrowException()
    {
        // Arrange
        MyCipherWheel myCipherWheel = new(1);
        string expected = "The MyCipherWheel must be initialized before calling the SetCipherIndex method.";

        // Act
        Action action = () => myCipherWheel.SetCipherIndex(5);

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
    public void SetCipherIndexWhenValueIsOutOfRange_ShouldThrowException(int value)
    {
        // Arrange
        MyCipherWheel myCipherWheel = new(1);
        myCipherWheel.SetState(null, null, true);
        string expected = $"The index value passed into the SetCipherIndex method of the MyCipherWheel class must be greater than or equal to 0 and less than {TableSize}, but it was {value}. (Parameter 'indexValue')";

        // Act
        Action action = () => myCipherWheel.SetCipherIndex(value);

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
    public void SetCipherIndexWithDifferentCycleSizes_ShouldCorrectlyInitializeCipherIndexAndCycleCount(int cycleSize, int expectedIndexValue, int expectedCycleCount)
    {
        // Arrange
        MyCipherWheel myCipherWheel = new(cycleSize);
        myCipherWheel.SetState(null, null, true);

        // Act
        myCipherWheel.SetCipherIndex(expectedIndexValue);

        // Assert
        myCipherWheel.CipherIndex
            .Should()
            .Be(expectedIndexValue);
        myCipherWheel.CycleCount
            .Should()
            .Be(expectedCycleCount);
    }
}

[ExcludeFromCodeCoverage]
internal sealed class MyCipherWheel(int cycleSize) : CipherWheel(cycleSize)
{
    public static int TestGetValueWithOffset(int baseValue, int arraySize, int offset) => GetIndexValueWithOffset(baseValue, arraySize, offset);

    public override void Initialize(string seed) => throw new NotImplementedException();

    public int TestDisplaceIndex(int index, char seedChar) => DisplaceIndex(index, seedChar);

    public int TestFindAvailableSlot(int startIndex, bool[] slotIsTaken) => FindAvailableConnectionPoint(startIndex, slotIsTaken);

    public int TestGetTransformedValue(int[] table, int valueIn) => GetTransformedValue(table, valueIn);

    public void TestRotate() => Rotate();

    public override int Transform(int c) => throw new NotImplementedException();
}