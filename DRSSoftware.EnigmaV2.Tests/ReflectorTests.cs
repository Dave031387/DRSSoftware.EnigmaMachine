namespace DRSSoftware.EnigmaV2;

using FluentAssertions;
using System;

public class ReflectorTests
{
    private readonly string _seed = "ThisIsASimpleSeedString";

    [Fact]
    public void ConnectOutgoing_ShouldCorrectlyConnectOutgoingRotor()
    {
        // Arrange
        Mock<IRotor> mock = new(MockBehavior.Strict);
        Reflector reflector = new();

        // Act
        reflector.ConnectOutgoing(mock.Object);

        // Assert
        reflector._rotorOut
            .Should()
            .Be(mock.Object);
    }

    [Fact]
    public void ConnectOutgoingWhenAlreadyConnected_ShouldThrowException()
    {
        // Arrange
        Mock<IRotor> mock = new(MockBehavior.Strict);
        Reflector reflector = new()
        {
            _rotorOut = mock.Object
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
        Reflector reflector = new();

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
        Reflector reflector = new();

        // Assert
        reflector._isInitialized
            .Should()
            .BeFalse();
        reflector._reflectorIndex
            .Should()
            .Be(0);
        reflector._rotorOut
            .Should()
            .BeNull();
        reflector._reflectorTable.Length
            .Should()
            .Be(TableSize);
        reflector._reflectorTable
            .Should()
            .OnlyContain(static x => x == 0);
    }

    [Fact]
    public void Initialize_ShouldProperlyInitializeTheReflector()
    {
        // Arrange
        Reflector reflector = new()
        {
            _reflectorIndex = 5
        };

        // Act
        reflector.Initialize(_seed);

        // Assert
        AssertValidTable(reflector);

        reflector._reflectorIndex
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
    public void InitializeWhenSeedIsNull_ShouldThrowException(string seed)
    {
        // Arrange
        Reflector reflector = new();
        string expected = $"The seed string passed into the Initialize method must be at least {MinSeedLength} characters long, but it was {seed!.Length}. (Parameter 'seed')";

        // Act
        Action action = () => reflector.Initialize(seed);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentException>()
            .WithMessage(expected);
    }

    [Fact]
    public void SetIndexWhenNotInitialized_ShouldThrowException()
    {
        // Arrange
        Reflector reflector = new();
        string expected = "The reflector must be initialized before the index can be set.";

        // Act
        Action action = () => reflector.SetIndex(5);

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
        reflector._reflectorIndex
            .Should()
            .Be(0);
    }

    [Theory]
    [InlineData(0, 10)]
    [InlineData(90, 10)]
    [InlineData(45, 46)]
    [InlineData(0, MaxIndex)]
    public void SetIndexWhenValueIsInRange_ShouldSetIndex(int originalValue, int expected)
    {
        // Arrange
        Reflector reflector = new();
        reflector.Initialize(_seed);
        int[] originalTable = new int[TableSize];
        reflector._reflectorTable.CopyTo(originalTable, 0);
        reflector._reflectorIndex = originalValue;

        // Act
        reflector.SetIndex(expected);

        // Assert
        reflector._reflectorIndex
            .Should()
            .Be(expected);
        AssertValidTable(reflector, originalTable);
    }

    [Fact]
    public void SetIndexWhenValueIsUnchanged_ShouldDoNothing()
    {
        // Arrange
        Reflector reflector = new();
        reflector.Initialize(_seed);
        int[] originalTable = new int[TableSize];
        reflector._reflectorTable.CopyTo(originalTable, 0);
        int expected = 39;
        reflector._reflectorIndex = expected;

        // Act
        reflector.SetIndex(expected);

        // Assert
        reflector._reflectorIndex
            .Should()
            .Be(expected);
        AssertValidTable(reflector, originalTable, true);
    }

    [Theory]
    [InlineData(-5)]
    [InlineData(-1)]
    [InlineData(TableSize)]
    [InlineData(TableSize + 5)]
    public void SetIndexWhenValueOutOfRange_ShouldThrowException(int value)
    {
        // Arrange
        Reflector reflector = new()
        {
            _isInitialized = true
        };
        string expected = $"The value passed into the SetIndex method must be greater than or equal to zero and less than {TableSize}, but it was {value}. (Parameter 'indexValue')";

        // Act
        Action action = () => reflector.SetIndex(value);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentOutOfRangeException>()
            .WithMessage(expected);
        reflector._reflectorIndex
            .Should()
            .Be(0);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TransformWhenInitializedProperly_ShouldReturnTransformedValue(bool shouldRotate)
    {
        // Arrange
        Reflector reflector = new();
        reflector.Initialize(_seed);
        int[] originalTable = new int[TableSize];
        reflector._reflectorTable.CopyTo(originalTable, 0);
        Mock<IRotor> mock = new(MockBehavior.Strict);
        reflector.ConnectOutgoing(mock.Object);
        int expected = 33;
        mock.Setup(static r => r.TransformOut(It.IsAny<int>())).Returns(expected).Verifiable(Times.Once);
        int reflectorIndex = shouldRotate ? 1 : 0;

        // Act
        int actual = reflector.TransformIn(42, shouldRotate);

        // Assert
        mock.VerifyAll();
        actual
            .Should()
            .Be(expected);
        reflector._reflectorIndex
            .Should()
            .Be(reflectorIndex);
        AssertValidTable(reflector, originalTable, !shouldRotate);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TransformWhenNotInitialized_ShouldThrowException(bool shouldRotate)
    {
        // Arrange
        Reflector reflector = new();
        string expected = "The reflector must be initialized before calling the TransformIn method.";

        // Act
        Action action = () => reflector.TransformIn(55, shouldRotate);

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TransformWhenOutgoingRotorIsNull_ShouldThrowException(bool shouldRotate)
    {
        // Arrange
        Reflector reflector = new();
        reflector.Initialize(_seed);
        int[] originalTable = new int[TableSize];
        reflector._reflectorTable.CopyTo(originalTable, 0);
        string expected = "The outgoing rotor hasn't been connected to the reflector.";

        // Act
        Action action = () => reflector.TransformIn(32, shouldRotate);

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
        reflector._reflectorIndex
            .Should()
            .Be(0);
        AssertValidTable(reflector, originalTable, true);
    }

    private static void AssertValidTable(Reflector reflector, int[]? originalTable = null, bool shouldBeSame = false)
    {
        int matchCount = 0;

        for (int i = 0; i < TableSize; i++)
        {
            int j = reflector._reflectorTable[i];
            reflector._reflectorTable[i]
                .Should()
                .NotBe(i);
            reflector._reflectorTable[j]
                .Should()
                .Be(i);

            if (originalTable is not null && originalTable[i] == reflector._reflectorTable[i])
            {
                matchCount++;
            }
        }

        if (originalTable is not null)
        {
            if (shouldBeSame)
            {
                matchCount
                    .Should()
                    .Be(TableSize);
            }
            else
            {
                matchCount
                    .Should()
                    .NotBe(TableSize);
            }
        }
    }
}