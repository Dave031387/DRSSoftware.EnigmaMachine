namespace DRSSoftware.EnigmaV2;

public class RotorTests
{
    private readonly string _seed = "ThisIsASimpleSeedString";

    [Fact]
    public void ConnectIncoming_ShouldCorrectlyConnectIncomingRotor()
    {
        // Arrange
        Rotor rotor = new();
        Mock<IRotor> mock = new(MockBehavior.Strict);

        // Act
        rotor.ConnectIncoming(mock.Object);

        // Assert
        rotor._rotorIn
            .Should()
            .BeSameAs(mock.Object);
    }

    [Fact]
    public void ConnectIncomingWhenAlreadyConnected_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new();
        Mock<IRotor> mock = new(MockBehavior.Strict);
        rotor._rotorIn = mock.Object;
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
        Rotor rotor = new();

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
        Rotor rotor = new();
        Mock<ITransformer> mock = new(MockBehavior.Strict);

        // Act
        rotor.ConnectOutgoing(mock.Object);

        // Assert
        rotor._transformerOut
            .Should()
            .BeSameAs(mock.Object);
    }

    [Fact]
    public void ConnectOutgoingWhenAlreadyConnected_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new();
        Mock<ITransformer> mock = new(MockBehavior.Strict);
        rotor._transformerOut = mock.Object;
        string expected = "Invalid attempt to add an outgoing transformer when one is already defined for this rotor.";

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
        Rotor rotor = new();

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
        Rotor rotor = new();

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
        rotor._rotorIn
            .Should()
            .BeNull();
        rotor._rotorIndex
            .Should()
            .Be(0);
        rotor._transformerOut
            .Should()
            .BeNull();
        rotor._transformIsInProgress
            .Should()
            .BeFalse();
    }

    [Fact]
    public void Initialize_ShouldProperlyInitializeTheRotor()
    {
        // Arrange
        Rotor rotor = new()
        {
            _isInitialized = false,
            _rotorIndex = 7,
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
        rotor._rotorIndex
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
        Rotor rotor = new();

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
        Rotor rotor = new();
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
    [InlineData(0)]
    [InlineData(33)]
    [InlineData(MaxIndex - 15)]
    [InlineData(MaxIndex)]
    public void SetIndex_ShouldSetIndexToSpecifiedValue(int expected)
    {
        // Arrange
        Rotor rotor = new()
        {
            _isInitialized = true
        };

        // Act
        rotor.SetIndex(expected);

        // Assert
        rotor._rotorIndex
            .Should()
            .Be(expected);
    }

    [Fact]
    public void SetIndexWhenRotorNotInitialized_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new();
        string expected = "The rotor must be initialized before the index can be set.";

        // Act
        Action action = () => rotor.SetIndex(5);

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
        Rotor rotor = new()
        {
            _isInitialized = true
        };
        string expected = $"The value passed into the SetIndex method must be greater than or equal to zero and less than {TableSize}, but it was {value}. (Parameter 'indexValue')";

        // Act
        Action action = () => rotor.SetIndex(value);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>()
            .WithMessage(expected);
    }

    [Theory]
    [InlineData(0, 0, false, false)]
    [InlineData(44, 3, true, false)]
    [InlineData(75, MaxIndex - 1, true, false)]
    [InlineData(91, MaxIndex, false, false)]
    [InlineData(57, MaxIndex, true, true)]
    public void Transform_ShouldBeReversible(int expected, int index, bool shouldRotate, bool shouldRotateNext)
    {
        // Arrange
        Rotor rotor = new();
        rotor.Initialize(_seed);
        rotor.SetIndex(index);
        Mock<ITransformer> mock = new(MockBehavior.Strict);
        mock.Setup(r => r.TransformIn(It.IsAny<int>(), shouldRotateNext))
            .Returns(static (int transformed, bool _) => transformed)
            .Verifiable(Times.Once);
        rotor.ConnectOutgoing(mock.Object);
        int transformedValue = rotor.TransformIn(expected, shouldRotate);

        // Act
        int actual = rotor.TransformOut(transformedValue);

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
    [InlineData(0, 91, 0, 0, false, false)]
    [InlineData(0, 32, 0, 1, true, false)]
    [InlineData(43, 57, 10, 10, false, false)]
    [InlineData(27, 4, 10, 11, true, false)]
    [InlineData(82, 60, MaxIndex, MaxIndex, false, false)]
    [InlineData(14, 26, MaxIndex, 0, true, true)]
    [InlineData(5, 17, MaxIndex - 1, MaxIndex - 1, false, false)]
    [InlineData(90, 8, MaxIndex - 1, MaxIndex, true, false)]
    public void TransformIn_ShouldReturnTransformedValue(int value, int transformedValue, int index, int expectedIndex, bool shouldRotate, bool shouldRotateNext)
    {
        // Arrange
        Rotor rotor = new();
        rotor.Initialize(_seed);
        rotor.SetIndex(index);
        Mock<ITransformer> mock = new(MockBehavior.Strict);
        mock.Setup(r => r.TransformIn(It.IsAny<int>(), shouldRotateNext))
            .Returns(transformedValue)
            .Verifiable(Times.Once);
        rotor.ConnectOutgoing(mock.Object);

        // Act
        int actual = rotor.TransformIn(value, shouldRotate);

        // Assert
        mock.VerifyAll();
        actual
            .Should()
            .Be(transformedValue);
        rotor._rotorIndex
            .Should()
            .Be(expectedIndex);
        rotor._transformIsInProgress
            .Should()
            .BeTrue();
    }

    [Fact]
    public void TransformInWhenRotorNotInitialized_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new();
        string expected = "The rotor must be initialized before the TransformIn method is called.";

        // Act
        Action action = () => rotor.TransformIn(87, false);

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
        Rotor rotor = new();
        rotor.Initialize(_seed);
        string expected = "An outgoing transformer hasn't been connected to this rotor.";

        // Act
        Action action = () => rotor.TransformIn(53, false);

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
    }

    [Fact]
    public void TransformInWhenTransformAlreadyInProgress_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new()
        {
            _transformIsInProgress = true,
            _isInitialized = true
        };
        string expected = "The TransformOut method wasn't called after the last call to the TransformIn method.";

        // Act
        Action action = () => rotor.TransformIn(65, false);

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
    }

    [Fact]
    public void TransformOutWhenIncomingRotorIsConnected_ShouldReturnTransformedValue()
    {
        // Arrange
        Rotor rotor = new();
        rotor.Initialize(_seed);
        rotor._transformIsInProgress = true;
        int expected = 35;
        Mock<IRotor> mock = new(MockBehavior.Strict);
        mock.Setup(r => r.TransformOut(It.IsAny<int>()))
            .Returns(expected)
            .Verifiable(Times.Once);
        rotor.ConnectIncoming(mock.Object);

        // Act
        int actual = rotor.TransformOut(22);

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
        Rotor rotor = new();
        rotor.Initialize(_seed);
        rotor.SetIndex(rotorIndex);
        rotor._transformIsInProgress = true;
        int lookup = rotor._outgoingTable[lookupIndex];
        int expected = lookup + rotorIndex > MaxIndex ? lookup + rotorIndex - TableSize : lookup + rotorIndex;

        // Act
        int actual = rotor.TransformOut(value);

        // Assert
        actual
            .Should()
            .Be(expected);
        rotor._transformIsInProgress
            .Should()
            .BeFalse();
    }

    [Fact]
    public void TransformOutWhenTransformNotInProgress_ShouldThrowException()
    {
        // Arrange
        Rotor rotor = new();
        string expected = "The TransformOut method must not be called on the rotor before calling the TransformIn method.";

        // Act
        Action action = () => rotor.TransformOut(42);

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
    }
}