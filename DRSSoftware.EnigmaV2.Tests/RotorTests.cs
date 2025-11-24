namespace DRSSoftware.EnigmaV2;

public class RotorTests
{
    private readonly string _seed = "ThisIsASimpleSeedString";

    [Fact]
    public void ConnectInboundComponent_ShouldCorrectlyConnectInboundRotor()
    {
        // Arrange
        Rotor rotor = new(1);
        Mock<IRotor> mock = new(MockBehavior.Strict);

        // Act
        rotor.ConnectInboundComponent(mock.Object);

        // Assert
        rotor.InboundCipherWheel
            .Should()
            .BeSameAs(mock.Object);
    }

    [Fact]
    public void ConnectInboundComponentWhenAlreadyConnected_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new(1);
        rotor.ConnectInboundComponent(new Rotor(11));
        string expected = "Invalid attempt to add an inbound rotor when one is already defined for this rotor.";

        // Act
        Action action = () => rotor.ConnectInboundComponent(new Rotor(13));

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
    }

    [Fact]
    public void ConnectInboundComponentWhenParameterIsNull_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new(1);

        // Act
        Action action = () => rotor.ConnectInboundComponent(null!);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void ConnectOutboundComponent_ShouldCorrectlyConnectOutboundCipherWheel()
    {
        // Arrange
        Rotor rotor = new(1);
        Mock<ICipherWheel> mock = new(MockBehavior.Strict);

        // Act
        rotor.ConnectOutboundComponent(mock.Object);

        // Assert
        rotor.OutboundCipherWheel
            .Should()
            .BeSameAs(mock.Object);
    }

    [Fact]
    public void ConnectOutboundComponentWhenAlreadyConnected_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new(1);
        rotor.ConnectOutboundComponent(new Rotor(11));
        string expected = "Invalid attempt to add an outbound cipher wheel when one is already defined for this rotor.";

        // Act
        Action action = () => rotor.ConnectOutboundComponent(new Rotor(13));

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
    }

    [Fact]
    public void ConnectOutboundComponentWhenParameterIsNull_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new(1);

        // Act
        Action action = () => rotor.ConnectOutboundComponent(null!);

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
        rotor.InboundCipherWheel
            .Should()
            .BeNull();
        rotor.CipherIndex
            .Should()
            .Be(0);
        rotor.OutboundCipherWheel
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
            int j = rotor.GetInboundValue(i);

            if (i == j)
            {
                matches++;
            }

            rotor.GetOutboundValue(j)
                .Should()
                .Be(i, $"_outboundTable[{j}] should be {i}, but it was {rotor.GetOutboundValue(j)}");
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
        rotor.SetIndex(index);
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
        rotor.ConnectOutboundComponent(mock.Object);
        int transform1 = rotor.Transform(expected);
        int transform2 = rotor.Transform(transform1);
        rotor.SetIndex(index);

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
        rotor.SetIndex(index);
        Mock<ICipherWheel> mock = new(MockBehavior.Strict);
        mock.Setup(r => r.Transform(It.IsAny<int>()))
            .Returns(transformedValue)
            .Verifiable(Times.Once);
        rotor.ConnectOutboundComponent(mock.Object);

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
        rotor.ConnectOutboundComponent(mock.Object);

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
    public void TransformInWhenThereIsNoOutboundTransformerConnected_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new(1);
        rotor.Initialize(_seed);
        string expected = "An outbound cipher wheel hasn't been connected to this rotor.";

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
        rotor.ConnectOutboundComponent(mock.Object);

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

    [Fact]
    public void TransformOutWhenInboundRotorIsConnected_ShouldReturnTransformedValue()
    {
        // Arrange
        Rotor rotor = new(1);
        int expected = 35;
        Mock<IRotor> mock = new(MockBehavior.Strict);
        mock.Setup(r => r.Transform(It.IsAny<int>()))
            .Returns(expected)
            .Verifiable(Times.Once);
        rotor.ConnectOutboundComponent(mock.Object);
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

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(0, 10, TableSize - 10)]
    [InlineData(50, 3, 47)]
    [InlineData(91, 12, 79)]
    [InlineData(12, 91, TableSize - 79)]
    public void TransformOutWhenNoInboundRotorIsConnected_ShouldReturnTransformedValue(int value, int indexValue, int lookupIndex)
    {
        // Arrange
        Rotor rotor = new(1);
        rotor.Initialize(_seed);
        rotor.ConnectOutboundComponent(new Rotor(13));
        rotor.SetState(indexValue, null, null, true);
        int lookup = rotor.GetOutboundValue(lookupIndex);
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
}