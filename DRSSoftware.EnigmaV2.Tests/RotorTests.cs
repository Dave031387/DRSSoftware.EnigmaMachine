namespace DRSSoftware.EnigmaV2;

public class RotorTests
{
    private readonly string _seed = "ThisIsASimpleSeedString";

    [Fact]
    public void ConnectIncoming_ShouldCorrectlyConnectIncomingRotor()
    {
        // Arrange
        Rotor rotor = new(1);
        Mock<IRotor> mock = new(MockBehavior.Strict);

        // Act
        rotor.ConnectIncoming(mock.Object);

        // Assert
        rotor._cipherWheelIn
            .Should()
            .BeSameAs(mock.Object);
    }

    [Fact]
    public void ConnectIncomingWhenAlreadyConnected_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new(1);
        Mock<IRotor> mock = new(MockBehavior.Strict);
        rotor._cipherWheelIn = mock.Object;
        string expected = "Invalid attempt to add an incoming rotor when one is already defined for this rotor.";

        // Act
        Action action = () => rotor.ConnectIncoming(mock.Object);

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
    }

    [Fact]
    public void ConnectIncomingWhenParameterIsNull_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new(1);

        // Act
        Action action = () => rotor.ConnectIncoming(null!);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void ConnectOutgoing_ShouldCorrectlyConnectOutgoingTransformer()
    {
        // Arrange
        Rotor rotor = new(1);
        Mock<ICipherWheel> mock = new(MockBehavior.Strict);

        // Act
        rotor.ConnectOutgoing(mock.Object);

        // Assert
        rotor._cipherWheelOut
            .Should()
            .BeSameAs(mock.Object);
    }

    [Fact]
    public void ConnectOutgoingWhenAlreadyConnected_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new(1);
        Mock<ICipherWheel> mock = new(MockBehavior.Strict);
        rotor._cipherWheelOut = mock.Object;
        string expected = "Invalid attempt to add an outgoing cipher wheel when one is already defined for this rotor.";

        // Act
        Action action = () => rotor.ConnectOutgoing(mock.Object);

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
    }

    [Fact]
    public void ConnectOutgoingWhenParameterIsNull_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new(1);

        // Act
        Action action = () => rotor.ConnectOutgoing(null!);

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
        rotor._incomingTable
            .Should()
            .HaveCount(TableSize)
            .And
            .OnlyContain(static x => x == 0);
        rotor._outgoingTable
            .Should()
            .HaveCount(TableSize)
            .And
            .OnlyContain(static x => x == 0);
        rotor._isInitialized
            .Should()
            .BeFalse();
        rotor._cipherWheelIn
            .Should()
            .BeNull();
        rotor._cipherIndex
            .Should()
            .Be(0);
        rotor._cipherWheelOut
            .Should()
            .BeNull();
        rotor._transformIsInProgress
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
        rotor._cycleSize
            .Should()
            .Be(expected);
    }

    [Fact]
    public void Initialize_ShouldProperlyInitializeTheRotor()
    {
        // Arrange
        Rotor rotor = new(1)
        {
            _isInitialized = false,
            _cipherIndex = 7,
            _transformIsInProgress = true
        };
        int maxMatches = 4;

        // Act
        rotor.Initialize(_seed);

        // Assert
        int matches = 0;

        for (int i = 0; i < TableSize; i++)
        {
            int j = rotor._incomingTable[i];

            if (i == j)
            {
                matches++;
            }

            rotor._outgoingTable[j]
                .Should()
                .Be(i, $"_outgoingTable[{j}] should be {i}, but it was {rotor._outgoingTable[j]}");
        }

        matches
            .Should()
            .BeLessThanOrEqualTo(maxMatches, $"there shouldn't be more than {maxMatches} connections that aren't shuffled, but found {matches}");
        rotor._isInitialized
            .Should()
            .BeTrue();
        rotor._cipherIndex
            .Should()
            .Be(0);
        rotor._transformIsInProgress
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
        rotor.ConnectOutgoing(mock.Object);
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
        rotor._transformIsInProgress
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
        rotor.ConnectOutgoing(mock.Object);

        // Act
        int actual = rotor.Transform(value);

        // Assert
        mock.VerifyAll();
        actual
            .Should()
            .Be(transformedValue);
        rotor._cipherIndex
            .Should()
            .Be(expectedIndex);
        rotor._transformIsInProgress
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
        rotor.ConnectOutgoing(mock.Object);

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
    public void TransformInWhenThereIsNoOutgoingTransformerConnected_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new(1);
        rotor.Initialize(_seed);
        string expected = "An outgoing cipher wheel hasn't been connected to this rotor.";

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
        Rotor rotor = new(1);
        rotor.Initialize(_seed);
        rotor._cipherIndex = 77;
        rotor._cycleSize = cycleSize;
        rotor._cycleCount = cycleCount;
        Mock<ICipherWheel> mock = new(MockBehavior.Strict);
        mock.Setup(r => r.Transform(It.IsAny<int>()))
            .Returns(66)
            .Verifiable(Times.Once);
        rotor.ConnectOutgoing(mock.Object);

        // Act
        int actual = rotor.Transform(55);

        // Assert
        mock.VerifyAll();
        rotor._cipherIndex
            .Should()
            .Be(expected);
        rotor._cycleCount
            .Should()
            .Be(updatedCount);
    }

    [Fact]
    public void TransformOutWhenIncomingRotorIsConnected_ShouldReturnTransformedValue()
    {
        // Arrange
        Rotor rotor = new(1)
        {
            _cipherWheelOut = new Rotor(13)
        };
        rotor.Initialize(_seed);
        rotor._transformIsInProgress = true;
        int expected = 35;
        Mock<IRotor> mock = new(MockBehavior.Strict);
        mock.Setup(r => r.Transform(It.IsAny<int>()))
            .Returns(expected)
            .Verifiable(Times.Once);
        rotor.ConnectIncoming(mock.Object);

        // Act
        int actual = rotor.Transform(22);

        // Assert
        mock.VerifyAll();
        actual
            .Should()
            .Be(expected);
        rotor._transformIsInProgress
            .Should()
            .BeFalse();
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(0, 10, TableSize - 10)]
    [InlineData(50, 3, 47)]
    [InlineData(91, 12, 79)]
    [InlineData(12, 91, TableSize - 79)]
    public void TransformOutWhenNoIncomingRotorIsConnected_ShouldReturnTransformedValue(int value, int rotorIndex, int lookupIndex)
    {
        // Arrange
        Rotor rotor = new(1);
        rotor.Initialize(_seed);
        rotor.SetIndex(rotorIndex);
        rotor._cipherWheelOut = new Rotor(13);
        rotor._transformIsInProgress = true;
        int lookup = rotor._outgoingTable[lookupIndex];
        int expected = lookup + rotorIndex > MaxIndex ? lookup + rotorIndex - TableSize : lookup + rotorIndex;

        // Act
        int actual = rotor.Transform(value);

        // Assert
        actual
            .Should()
            .Be(expected);
        rotor._transformIsInProgress
            .Should()
            .BeFalse();
    }
}