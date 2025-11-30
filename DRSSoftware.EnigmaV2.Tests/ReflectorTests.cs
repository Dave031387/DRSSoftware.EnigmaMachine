namespace DRSSoftware.EnigmaV2;

public class ReflectorTests
{
    private readonly int _cycleSize = 3;
    private readonly string _seed = "ThisIsASimpleSeedString";

    [Fact]
    public void ConnectRightComponent_ShouldCorrectlyRotorToRightSide()
    {
        // Arrange
        Mock<IRotor> mock = new(MockBehavior.Strict);
        Reflector reflector = new(_cycleSize);

        // Act
        reflector.ConnectRightComponent(mock.Object);

        // Assert
        reflector.RightCipherWheel
            .Should()
            .Be(mock.Object);
    }

    [Fact]
    public void ConnectRightComponentWhenAlreadyConnected_ShouldThrowException()
    {
        // Arrange
        Mock<IRotor> mock = new(MockBehavior.Strict);
        Reflector reflector = new(_cycleSize);
        reflector.ConnectRightComponent(new Rotor(11));
        string expected = "Invalid attempt to connect a rotor to the right side of the reflector one when one is already connected.";

        // Act
        Action action = () => reflector.ConnectRightComponent(new Rotor(13));

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
        Reflector reflector = new(_cycleSize);

        // Act
        Action action = () => reflector.ConnectRightComponent(null!);

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
        reflector.CycleCount
            .Should()
            .Be(0);
        reflector.CycleSize
            .Should()
            .Be(_cycleSize);
        reflector.IsInitialized
            .Should()
            .BeFalse();
        reflector.CipherIndex
            .Should()
            .Be(0);
        reflector.LeftCipherWheel
            .Should()
            .BeNull();
        reflector.OutboundTransformTable
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
        reflector.CycleSize
            .Should()
            .Be(expected);
    }

    [Fact]
    public void Initialize_ShouldProperlyInitializeTheReflector()
    {
        // Arrange
        Reflector reflector = new(_cycleSize);
        reflector.SetState(5, 6, false);

        // Act
        reflector.Initialize(_seed);

        // Assert
        int[] reflectorTable = reflector.OutboundTransformTable;

        for (int i = 0; i < TableSize; i++)
        {
            int j = reflectorTable[i];
            reflectorTable[i]
                .Should()
                .NotBe(i);
            reflectorTable[j]
                .Should()
                .Be(i);
        }

        reflector.CipherIndex
            .Should()
            .Be(0);
        reflector.CycleCount
            .Should()
            .Be(0);
        reflector.IsInitialized
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
        reflector.ConnectRightComponent(mock.Object);
        reflector.SetState(reflectorIndex, null, null);
        int transformed = reflector.Transform(expected);
        reflector.SetState(reflectorIndex, null, null);

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
        reflector.ConnectRightComponent(mock.Object);
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
    public void TransformWhenNoRotorIsConnectedOnRightSide_ShouldThrowException()
    {
        // Arrange
        Reflector reflector = new(_cycleSize);
        reflector.Initialize(_seed);
        string expected = "A rotor hasn't been connected to the right side of the reflector.";

        // Act
        Action action = () => reflector.Transform(32);

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
        reflector.CipherIndex
            .Should()
            .Be(0);
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
        reflector.SetState(57, cycleCount, null);
        Mock<IRotor> mock = new(MockBehavior.Strict);
        mock.Setup(static r => r.Transform(It.IsAny<int>()))
            .Returns(33)
            .Verifiable(Times.Once);
        reflector.ConnectRightComponent(mock.Object);

        // Act
        int actual = reflector.Transform(42);

        // Assert
        mock.VerifyAll();
        reflector.CipherIndex
            .Should()
            .Be(expected);
        reflector.CycleCount
            .Should()
            .Be(updatedCount);
    }
}