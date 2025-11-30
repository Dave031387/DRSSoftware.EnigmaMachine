namespace DRSSoftware.EnigmaV2;

public class RotorTests
{
    private readonly string _seed = "ThisIsASimpleSeedString";

    [Fact]
    public void ConnectLeftComponent_ShouldCorrectlyConnectCipherWheelOnLeftSideOfRotor()
    {
        // Arrange
        Rotor rotor = new(1);
        Mock<ICipherWheel> mock = new(MockBehavior.Strict);

        // Act
        rotor.ConnectLeftComponent(mock.Object);

        // Assert
        rotor.LeftCipherWheel
            .Should()
            .BeSameAs(mock.Object);
    }

    [Fact]
    public void ConnectLeftComponentWhenAlreadyConnected_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new(1);
        rotor.ConnectLeftComponent(new Rotor(11));
        string expected = "Invalid attempt to connect a cipher wheel to the left side of this rotor when one is already connected.";

        // Act
        Action action = () => rotor.ConnectLeftComponent(new Rotor(13));

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
    }

    [Fact]
    public void ConnectLeftComponentWhenParameterIsNull_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new(1);

        // Act
        Action action = () => rotor.ConnectLeftComponent(null!);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void ConnectRightComponent_ShouldCorrectlyConnectRightRotor()
    {
        // Arrange
        Rotor rotor = new(1);
        Mock<IRotor> mock = new(MockBehavior.Strict);

        // Act
        rotor.ConnectRightComponent(mock.Object);

        // Assert
        rotor.RightCipherWheel
            .Should()
            .BeSameAs(mock.Object);
    }

    [Fact]
    public void ConnectRightComponentWhenAlreadyConnected_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new(1);
        rotor.ConnectRightComponent(new Rotor(11));
        string expected = "Invalid attempt to connect a rotor to the right side of this rotor when one is already connected.";

        // Act
        Action action = () => rotor.ConnectRightComponent(new Rotor(13));

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
    }

    [Fact]
    public void ConnectRightComponentWhenParameterIsNull_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new(1);

        // Act
        Action action = () => rotor.ConnectRightComponent(null!);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void CreateNewRotor_ShouldInitializeObjectCorrectly()
    {
        // Arrange/Act
        Rotor rotor = new(1);

        // Assert
        rotor.InboundTransformTable
            .Should()
            .HaveCount(TableSize)
            .And
            .OnlyContain(static x => x == 0);
        rotor.OutboundTransformTable
            .Should()
            .HaveCount(TableSize)
            .And
            .OnlyContain(static x => x == 0);
        rotor.IsInitialized
            .Should()
            .BeFalse();
        rotor.RightCipherWheel
            .Should()
            .BeNull();
        rotor.CipherIndex
            .Should()
            .Be(0);
        rotor.LeftCipherWheel
            .Should()
            .BeNull();
        rotor.TransformIsInProgress
            .Should()
            .BeFalse();
    }

    [Theory]
    [InlineData(-5, 0)]
    [InlineData(-1, 0)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(50, 50)]
    [InlineData(MaxIndex - 1, MaxIndex - 1)]
    [InlineData(MaxIndex, MaxIndex)]
    [InlineData(MaxIndex + 1, MaxIndex)]
    [InlineData(MaxIndex + 5, MaxIndex)]
    public void CreateRotorWithDifferentCycleSizes_ShouldConstrainCycleSize(int cycleSize, int expected)
    {
        // Arrange/Act
        Rotor rotor = new(cycleSize);

        // Assert
        rotor.CycleSize
            .Should()
            .Be(expected);
    }

    [Fact]
    public void Initialize_ShouldProperlyInitializeTheRotor()
    {
        // Arrange
        Rotor rotor = new(1);
        rotor.SetState(7, null, null, true);
        int maxMatches = 4;

        // Act
        rotor.Initialize(_seed);

        // Assert
        int matches = 0;

        for (int i = 0; i < TableSize; i++)
        {
            int j = rotor.GetInboundTransformedValue(i);

            if (i == j)
            {
                matches++;
            }

            rotor.GetOutboundTransformedValue(j)
                .Should()
                .Be(i, $"_outboundTable[{j}] should be {i}, but it was {rotor.GetOutboundTransformedValue(j)}");
        }

        matches
            .Should()
            .BeLessThanOrEqualTo(maxMatches, $"there shouldn't be more than {maxMatches} connections that aren't shuffled, but found {matches}");
        rotor.IsInitialized
            .Should()
            .BeTrue();
        rotor.CipherIndex
            .Should()
            .Be(0);
        rotor.TransformIsInProgress
            .Should()
            .BeFalse();
    }

    [Fact]
    public void InitializeWhenSeedIsNull_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new(1);

        // Act
        Action action = () => rotor.Initialize(null!);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("ABCD")]
    [InlineData("123456789")]
    public void InitializeWhenSeedIsTooShort_ShouldThrowException(string seed)
    {
        // Arrange
        Rotor rotor = new(1);
        string expected = $"The seed string passed into the Initialize method must be at least {MinSeedLength} characters long, but it was {seed.Length}. (Parameter 'seed')";

        // Act
        Action action = () => rotor.Initialize(seed);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentException>()
            .WithMessage(expected);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(44, 3, 1)]
    [InlineData(75, MaxIndex - 1, 1)]
    [InlineData(91, MaxIndex, 0)]
    [InlineData(57, MaxIndex, 1)]
    public void Transform_ShouldBeReversible(int expected, int index, int cycleSize)
    {
        // Arrange
        Rotor rotor = new(cycleSize);
        rotor.Initialize(_seed);
        rotor.SetCipherIndex(index);
        int i = 1;

        int GetReturnValue(int baseValue)
        {
            int returnValue = baseValue + i;
            i -= 2;
            return returnValue;
        }

        Mock<ICipherWheel> mock = new(MockBehavior.Strict);
        mock.Setup(r => r.Transform(It.IsAny<int>()))
            .Returns((int transformed) => GetReturnValue(transformed))
            .Verifiable(Times.Exactly(2));
        rotor.ConnectLeftComponent(mock.Object);
        int transform1 = rotor.Transform(expected);
        int transform2 = rotor.Transform(transform1);
        rotor.SetCipherIndex(index);

        // Act
        int transform3 = rotor.Transform(transform2);
        int actual = rotor.Transform(transform3);

        // Assert
        mock.VerifyAll();
        actual
            .Should()
            .Be(expected);
        rotor.TransformIsInProgress
            .Should()
            .BeFalse();
    }

    [Theory]
    [InlineData(0, 91, 0, 0, 0)]
    [InlineData(0, 32, 0, 1, 1)]
    [InlineData(43, 57, 10, 10, 0)]
    [InlineData(27, 4, 10, 11, 1)]
    [InlineData(82, 60, MaxIndex, MaxIndex, 0)]
    [InlineData(14, 26, MaxIndex, 0, 1)]
    [InlineData(5, 17, MaxIndex - 1, MaxIndex - 1, 0)]
    [InlineData(90, 8, MaxIndex - 1, MaxIndex, 1)]
    public void TransformIn_ShouldReturnTransformedValue(int value, int transformedValue, int index, int expectedIndex, int cycleSize)
    {
        // Arrange
        Rotor rotor = new(cycleSize);
        rotor.Initialize(_seed);
        rotor.SetCipherIndex(index);
        Mock<ICipherWheel> mock = new(MockBehavior.Strict);
        mock.Setup(r => r.Transform(It.IsAny<int>()))
            .Returns(transformedValue)
            .Verifiable(Times.Once);
        rotor.ConnectLeftComponent(mock.Object);

        // Act
        int actual = rotor.Transform(value);

        // Assert
        mock.VerifyAll();
        actual
            .Should()
            .Be(transformedValue);
        rotor.CipherIndex
            .Should()
            .Be(expectedIndex);
        rotor.TransformIsInProgress
            .Should()
            .BeTrue();
    }

    [Fact]
    public void TransformInWhenInitializedProperly_ShouldReturnTransformedValue()
    {
        // Arrange
        Rotor rotor = new(1);
        rotor.Initialize(_seed);
        int expected = 12;
        Mock<ICipherWheel> mock = new(MockBehavior.Strict);
        mock.Setup(r => r.Transform(It.IsAny<int>()))
            .Returns(expected)
            .Verifiable(Times.Once);
        rotor.ConnectLeftComponent(mock.Object);

        // Act
        int actual = rotor.Transform(42);

        // Assert
        mock.VerifyAll();
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void TransformInWhenRotorNotInitialized_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new(1);
        string expected = "The rotor must be initialized before the Transform method is called.";

        // Act
        Action action = () => rotor.Transform(87);

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
    }

    [Fact]
    public void TransformInWhenThereIsNoCipherWheelConnectedToTheLeftSideOfTheRotor_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new(1);
        rotor.Initialize(_seed);
        string expected = "A cipher wheel hasn't been connected to the left side of this rotor.";

        // Act
        Action action = () => rotor.Transform(53);

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
    }

    [Theory]
    [InlineData(0, 0, 0, 77)]
    [InlineData(1, 0, 0, 78)]
    [InlineData(2, 0, 1, 77)]
    [InlineData(2, 1, 0, 78)]
    [InlineData(5, 0, 1, 77)]
    [InlineData(5, 3, 4, 77)]
    [InlineData(5, 4, 0, 78)]
    public void TransformInWithDifferentCycleCounts_ShouldRotateWhenExpected(int cycleSize, int cycleCount, int updatedCount, int expected)
    {
        // Arrange
        Rotor rotor = new(cycleSize);
        rotor.Initialize(_seed);
        rotor.SetState(77, cycleCount, null, null);
        Mock<ICipherWheel> mock = new(MockBehavior.Strict);
        mock.Setup(r => r.Transform(It.IsAny<int>()))
            .Returns(66)
            .Verifiable(Times.Once);
        rotor.ConnectLeftComponent(mock.Object);

        // Act
        int actual = rotor.Transform(55);

        // Assert
        mock.VerifyAll();
        rotor.CipherIndex
            .Should()
            .Be(expected);
        rotor.CycleCount
            .Should()
            .Be(updatedCount);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(0, 10, TableSize - 10)]
    [InlineData(50, 3, 47)]
    [InlineData(91, 12, 79)]
    [InlineData(12, 91, TableSize - 79)]
    public void TransformOutWhenNoRotorIsConnectedOnRightSide_ShouldReturnTransformedValue(int value, int indexValue, int lookupIndex)
    {
        // Arrange
        Rotor rotor = new(1);
        rotor.Initialize(_seed);
        rotor.ConnectLeftComponent(new Rotor(13));
        rotor.SetState(indexValue, null, null, true);
        int lookup = rotor.GetOutboundTransformedValue(lookupIndex);
        int expected = lookup + indexValue > MaxIndex ? lookup + indexValue - TableSize : lookup + indexValue;

        // Act
        int actual = rotor.Transform(value);

        // Assert
        actual
            .Should()
            .Be(expected);
        rotor.TransformIsInProgress
            .Should()
            .BeFalse();
    }

    [Fact]
    public void TransformOutWhenRotorIsConnectedOnRightSide_ShouldReturnTransformedValue()
    {
        // Arrange
        Rotor rotor = new(1);
        int expected = 35;
        Mock<IRotor> mock = new(MockBehavior.Strict);
        mock.Setup(r => r.Transform(It.IsAny<int>()))
            .Returns(expected)
            .Verifiable(Times.Once);
        rotor.ConnectLeftComponent(mock.Object);
        rotor.Initialize(_seed);

        // Act
        int actual = rotor.Transform(22);

        // Assert
        mock.VerifyAll();
        actual
            .Should()
            .Be(expected);
        rotor.TransformIsInProgress
            .Should()
            .BeTrue();
    }
}