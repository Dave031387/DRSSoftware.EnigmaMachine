namespace DRSSoftware.EnigmaV2;

public class EnigmaMachineTests
{
    private readonly Mock<IReflector> _mockReflector = new(MockBehavior.Strict);
    private readonly Mock<IRotor> _mockRotor1 = new(MockBehavior.Strict);
    private readonly Mock<IRotor> _mockRotor2 = new(MockBehavior.Strict);
    private readonly Mock<IRotor> _mockRotor3 = new(MockBehavior.Strict);

    [Fact]
    public void CreateNewEnigmaMachine_ShouldCorrectlyConfigureInitialState()
    {
        // Arrange/Act
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();

        // Assert
        VerifyAll();
        enigmaMachine._reflector
            .Should()
            .NotBeNull()
            .And
            .BeSameAs(_mockReflector.Object);
        enigmaMachine._rotors
            .Should()
            .NotBeNull()
            .And
            .HaveCount(3)
            .And
            .ContainInOrder(_mockRotor1.Object, _mockRotor2.Object, _mockRotor3.Object);
        enigmaMachine._transformerIndexes
            .Should()
            .NotBeNull()
            .And
            .HaveCount(4)
            .And
            .OnlyContain(x => x == 0);
        enigmaMachine._isInitialized
            .Should()
            .BeFalse();
    }

    [Fact]
    public void InitializeUsingNullSeedValue_ShouldThrowException()
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();

        // Act
        Action action = () => enigmaMachine.Initialize(null!);

        // Assert
        VerifyAll();
        action
            .Should()
            .ThrowExactly<ArgumentNullException>();
        enigmaMachine._isInitialized
            .Should()
            .BeFalse();
    }

    [Fact]
    public void InitializeUsingValidSeedValue_ShouldInitialize()
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();
        int[] initialIndexes = [1, 2, 3, 4];
        initialIndexes.CopyTo(enigmaMachine._transformerIndexes, 0);
        string seed = "This is a seed value used for unit testing.";
        string reflectorSeed = "";
        string rotorSeed1 = "";
        string rotorSeed2 = "";
        string rotorSeed3 = "";
        _mockReflector
            .Setup(m => m.Initialize(It.IsAny<string>()))
            .Callback<string>(c => reflectorSeed = c)
            .Verifiable(Times.Once);
        _mockRotor1
            .Setup(m => m.Initialize(It.IsAny<string>()))
            .Callback<string>(c => rotorSeed1 = c)
            .Verifiable(Times.Once);
        _mockRotor2
            .Setup(m => m.Initialize(It.IsAny<string>()))
            .Callback<string>(c => rotorSeed2 = c)
            .Verifiable(Times.Once);
        _mockRotor3
            .Setup(m => m.Initialize(It.IsAny<string>()))
            .Callback<string>(c => rotorSeed3 = c)
            .Verifiable(Times.Once);

        // Act
        enigmaMachine.Initialize(seed);

        // Assert
        VerifyAll();
        reflectorSeed
            .Should()
            .HaveLength(seed.Length)
            .And
            .NotBe(rotorSeed1)
            .And
            .NotBe(rotorSeed2)
            .And
            .NotBe(rotorSeed3);
        rotorSeed1
            .Should()
            .HaveLength(seed.Length)
            .And
            .NotBe(rotorSeed2)
            .And
            .NotBe(rotorSeed3);
        rotorSeed2
            .Should()
            .HaveLength(seed.Length)
            .And
            .NotBe(rotorSeed3);
        rotorSeed3
            .Should()
            .HaveLength(seed.Length);
        enigmaMachine._isInitialized
            .Should()
            .BeTrue();
        enigmaMachine._transformerIndexes
            .Should()
            .OnlyContain(x => x == 0);
    }

    [Fact]
    public void ResetIndexesWhenInitialized_ShouldResetIndexes()
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();
        enigmaMachine._isInitialized = true;
        const int index1 = 1;
        const int index2 = 2;
        const int index3 = 3;
        const int index4 = 4;
        int[] indexValues = [index1, index2, index3, index4];
        indexValues.CopyTo(enigmaMachine._transformerIndexes, 0);
        _mockRotor1
            .Setup(static m => m.SetIndex(index1))
            .Verifiable(Times.Once);
        _mockRotor2
            .Setup(static m => m.SetIndex(index2))
            .Verifiable(Times.Once);
        _mockRotor3
            .Setup(static m => m.SetIndex(index3))
            .Verifiable(Times.Once);
        _mockReflector
            .Setup(static m => m.SetIndex(index4))
            .Verifiable(Times.Once);

        // Act
        enigmaMachine.ResetIndexes();

        // Assert
        VerifyAll();
    }

    [Fact]
    public void ResetIndexesWhenNotInitialized_ShouldDoNothing()
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();

        // Act
        enigmaMachine.ResetIndexes();

        // Assert
        VerifyAll();
    }

    [Fact]
    public void SetIndexesWhenInitialized_ShouldSetIndexesToCorrectValues()
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();
        enigmaMachine._isInitialized = true;
        const int index1 = 11;
        const int index2 = 22;
        const int index3 = 33;
        const int index4 = 44;
        _mockRotor1
            .Setup(static m => m.SetIndex(index1))
            .Verifiable(Times.Once);
        _mockRotor2
            .Setup(static m => m.SetIndex(index2))
            .Verifiable(Times.Once);
        _mockRotor3
            .Setup(static m => m.SetIndex(index3))
            .Verifiable(Times.Once);
        _mockReflector
            .Setup(static m => m.SetIndex(index4))
            .Verifiable(Times.Once);

        // Act
        enigmaMachine.SetIndexes(index1, index2, index3, index4);

        // Assert
        VerifyAll();
        enigmaMachine._transformerIndexes
            .Should()
            .ContainInOrder(index1, index2, index3, index4);
    }

    [Fact]
    public void SetIndexesWhenNotInitialized_ShouldThrowException()
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();
        int[] indexes = [1, 2, 3, 4];
        string expected = "The Enigma machine must be initialized before setting the indexes.";

        // Act
        Action action = () => enigmaMachine.SetIndexes(indexes);

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
        enigmaMachine._transformerIndexes
            .Should()
            .OnlyContain(x => x == 0);
    }

    [Fact]
    public void SetIndexesWhenParameterIsNull_ShouldThrowException()
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();
        enigmaMachine._isInitialized = true;

        // Act
        Action action = () => enigmaMachine.SetIndexes(null!);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    [Theory]
    [InlineData(new int[] { 1, 2, 3 }, "were")]
    [InlineData(new int[] { 1, 2, 3, 4, 5 }, "were")]
    [InlineData(new int[] { 1 }, "was")]
    public void SetIndexesWhenWrongNumberOfIndexValuesGiven_ShouldThrowException(int[] indexes, string word)
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();
        enigmaMachine._isInitialized = true;
        string expected = $"Exactly 4 index values must be passed into the SetIndexes method, but there {word} {indexes.Length}. (Parameter 'indexes')";

        // Act
        Action action = () => enigmaMachine.SetIndexes(indexes);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentException>()
            .WithMessage(expected);
        enigmaMachine._transformerIndexes
            .Should()
            .OnlyContain(x => x == 0);
    }

    [Theory]
    [InlineData(" ", 0, 56, "X")]
    [InlineData("~", 94, 32, "@")]
    [InlineData("\u001f", 0, 94, "~")]
    [InlineData("\u0004", 0, 95, "\r\n")]
    [InlineData("\u007f", 0, 65, "a")]
    [InlineData("\u0100", 0, 48, "P")]
    [InlineData("\n", 95, 18, "2")]
    [InlineData("\r\n", 95, 29, "=")]
    public void TransformWhenInitialized_ShouldReturnTransformedValue(string original, int adjusted, int transformed, string expected)
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();
        enigmaMachine._isInitialized = true;
        _mockRotor1
            .Setup(m => m.TransformIn(adjusted))
            .Returns(transformed)
            .Verifiable(Times.Once);

        // Act
        string actual = enigmaMachine.Transform(original);

        // Assert
        VerifyAll();
        actual
            .Should()
            .Be(expected);
    }

    [Fact]
    public void TransformWhenNotInitialized_ShouldThrowException()
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();
        string expected = "The Enigma machine must be initialized before calling the Transform method.";

        // Act
        Action action = () => enigmaMachine.Transform("test");

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
        VerifyAll();
    }

    [Fact]
    public void TransformWhenParameterIsNull_ShouldThrowException()
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();
        enigmaMachine._isInitialized = true;

        // Act
        Action action = () => enigmaMachine.Transform(null!);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>();
        VerifyAll();
    }

    private EnigmaMachine CreateEnigmaMachine()
    {
        _mockReflector.Reset();
        _mockRotor1.Reset();
        _mockRotor2.Reset();
        _mockRotor3.Reset();
        _mockReflector
            .Setup(m => m.ConnectOutgoing(_mockRotor3.Object))
            .Verifiable(Times.Once);
        _mockRotor1
            .Setup(m => m.ConnectOutgoing(_mockRotor2.Object))
            .Verifiable(Times.Once);
        _mockRotor2
            .Setup(m => m.ConnectIncoming(_mockRotor1.Object))
            .Verifiable(Times.Once);
        _mockRotor2
            .Setup(m => m.ConnectOutgoing(_mockRotor3.Object))
            .Verifiable(Times.Once);
        _mockRotor3
            .Setup(m => m.ConnectIncoming(_mockRotor2.Object))
            .Verifiable(Times.Once);
        _mockRotor3
            .Setup(m => m.ConnectOutgoing(_mockReflector.Object))
            .Verifiable(Times.Once);
        return new(_mockReflector.Object, _mockRotor1.Object, _mockRotor2.Object, _mockRotor3.Object);
    }

    private void VerifyAll()
    {
        _mockReflector.VerifyAll();
        _mockRotor1.VerifyAll();
        _mockRotor2.VerifyAll();
        _mockRotor3.VerifyAll();
    }
}