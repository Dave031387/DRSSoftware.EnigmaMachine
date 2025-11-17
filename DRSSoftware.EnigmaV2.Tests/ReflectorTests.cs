namespace DRSSoftware.EnigmaV2;

public class ReflectorTests
{
    private readonly int _cycleSize = 3;
    private readonly string _seed = "ThisIsASimpleSeedString";

    [Fact]
    public void ConnectOutgoing_ShouldCorrectlyConnectOutgoingRotor()
    {
        // Arrange
        Mock<IRotor> mock = new(MockBehavior.Strict);
        Reflector reflector = new(_cycleSize);

        // Act
        reflector.ConnectOutgoing(mock.Object);

        // Assert
        reflector._cipherWheelOut
            .Should()
            .Be(mock.Object);
    }

    [Fact]
    public void ConnectOutgoingWhenAlreadyConnected_ShouldThrowException()
    {
        // Arrange
        Mock<IRotor> mock = new(MockBehavior.Strict);
        Reflector reflector = new(_cycleSize)
        {
            _cipherWheelOut = mock.Object
        };
        string expected = "Invalid attempt to add an outgoing rotor when one is already defined for this reflector.";

        // Act
        Action action = () => reflector.ConnectOutgoing(mock.Object);

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
        Reflector reflector = new(_cycleSize);

        // Act
        Action action = () => reflector.ConnectOutgoing(null!);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void CreateNewReflector_ShouldInitializeObjectProperly()
    {
        // Arrange/Act
        Reflector reflector = new(_cycleSize);

        // Assert
        reflector._cycleCount
            .Should()
            .Be(0);
        reflector._cycleSize
            .Should()
            .Be(_cycleSize);
        reflector._isInitialized
            .Should()
            .BeFalse();
        reflector._cipherIndex
            .Should()
            .Be(0);
        reflector._cipherWheelOut
            .Should()
            .BeNull();
        reflector._reflectorTable
            .Should()
            .OnlyContain(static x => x == 0)
            .And
            .HaveCount(TableSize);
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
    public void CreateReflectorWithDifferentCycleSizes_ShouldConstrainCycleSize(int cycleSize, int expected)
    {
        // Arrange/Act
        Reflector reflector = new(cycleSize);

        // Assert
        reflector._cycleSize
            .Should()
            .Be(expected);
    }

    [Fact]
    public void Initialize_ShouldProperlyInitializeTheReflector()
    {
        // Arrange
        Reflector reflector = new(_cycleSize)
        {
            _cipherIndex = 5
        };

        // Act
        reflector.Initialize(_seed);

        // Assert
        for (int i = 0; i < TableSize; i++)
        {
            int j = reflector._reflectorTable[i];
            reflector._reflectorTable[i]
                .Should()
                .NotBe(i);
            reflector._reflectorTable[j]
                .Should()
                .Be(i);
        }

        reflector._cipherIndex
            .Should()
            .Be(0);
        reflector._isInitialized
            .Should()
            .BeTrue();
    }

    [Theory]
    [InlineData("Test")]
    [InlineData("Test 1234")]
    [InlineData("")]
    public void InitializeWhenSeedIsLessThanMinimumLength_ShouldThrowException(string seed)
    {
        // Arrange
        Reflector reflector = new(_cycleSize);
        string expected = $"The seed string passed into the Initialize method must be at least {MinSeedLength} characters long, but it was {seed.Length}. (Parameter 'seed')";

        // Act
        Action action = () => reflector.Initialize(seed);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentException>()
            .WithMessage(expected);
    }

    [Fact]
    public void InitializeWhenSeedIsNull_ShouldThrowException()
    {
        // Arrange
        Reflector reflector = new(_cycleSize);

        // Act
        Action action = () => reflector.Initialize(null!);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    [Theory]
    [InlineData(0, 38)]
    [InlineData(13, 0)]
    [InlineData(42, MaxIndex)]
    [InlineData(65, 19)]
    [InlineData(MaxIndex, 77)]
    public void Transform_ShouldBeReversible(int expected, int reflectorIndex)
    {
        // Arrange
        Reflector reflector = new(_cycleSize);
        reflector.Initialize(_seed);
        Mock<IRotor> mock = new(MockBehavior.Strict);
        mock.Setup(static r => r.Transform(It.IsAny<int>()))
            .Returns(static (int transformed) => { return transformed; })
            .Verifiable(Times.Exactly(2));
        reflector.ConnectOutgoing(mock.Object);
        reflector._cipherIndex = reflectorIndex;
        int transformed = reflector.Transform(expected);
        reflector._cipherIndex = reflectorIndex;

        // Act
        int actual = reflector.Transform(transformed);

        // Assert
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void TransformWhenInitializedProperly_ShouldReturnTransformedValue()
    {
        // Arrange
        Reflector reflector = new(_cycleSize);
        reflector.Initialize(_seed);
        Mock<IRotor> mock = new(MockBehavior.Strict);
        reflector.ConnectOutgoing(mock.Object);
        int expected = 33;
        mock.Setup(static r => r.Transform(It.IsAny<int>()))
            .Returns(expected)
            .Verifiable(Times.Once);

        // Act
        int actual = reflector.Transform(42);

        // Assert
        mock.VerifyAll();
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void TransformWhenNotInitialized_ShouldThrowException()
    {
        // Arrange
        Reflector reflector = new(_cycleSize);
        string expected = "The reflector must be initialized before calling the Transform method.";

        // Act
        Action action = () => reflector.Transform(55);

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
    }

    [Fact]
    public void TransformWhenOutgoingRotorIsNull_ShouldThrowException()
    {
        // Arrange
        Reflector reflector = new(_cycleSize);
        reflector.Initialize(_seed);
        int[] originalTable = new int[TableSize];
        reflector._reflectorTable.CopyTo(originalTable, 0);
        string expected = "The outgoing rotor hasn't been connected to the reflector.";

        // Act
        Action action = () => reflector.Transform(32);

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
        reflector._cipherIndex
            .Should()
            .Be(0);
    }

    [Theory]
    [InlineData(0, 0, 0, 57)]
    [InlineData(1, 0, 0, 58)]
    [InlineData(2, 0, 1, 57)]
    [InlineData(2, 1, 0, 58)]
    [InlineData(5, 0, 1, 57)]
    [InlineData(5, 3, 4, 57)]
    [InlineData(5, 4, 0, 58)]
    public void TransformWithDifferentCycleCounts_ShouldRotateWhenExpected(int cycleSize, int cycleCount, int updatedCount, int expected)
    {
        // Arrange
        Reflector reflector = new(cycleSize);
        reflector.Initialize(_seed);
        reflector._cipherIndex = 57;
        reflector._cycleSize = cycleSize;
        reflector._cycleCount = cycleCount;
        Mock<IRotor> mock = new(MockBehavior.Strict);
        mock.Setup(static r => r.Transform(It.IsAny<int>()))
            .Returns(33)
            .Verifiable(Times.Once);
        reflector.ConnectOutgoing(mock.Object);

        // Act
        int actual = reflector.Transform(42);

        // Assert
        mock.VerifyAll();
        reflector._cipherIndex
            .Should()
            .Be(expected);
        reflector._cycleCount
            .Should()
            .Be(updatedCount);
    }
}