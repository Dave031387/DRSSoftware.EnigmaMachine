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
        enigmaMachine.MyReflector
            .Should()
            .NotBeNull()
            .And
            .BeSameAs(_mockReflector.Object);
        enigmaMachine.Rotor1
            .Should()
            .NotBeNull()
            .And
            .BeSameAs(_mockRotor1.Object);
        enigmaMachine.Rotor2
            .Should()
            .NotBeNull()
            .And
            .BeSameAs(_mockRotor2.Object);
        enigmaMachine.Rotor3
            .Should()
            .NotBeNull()
            .And
            .BeSameAs(_mockRotor3.Object);
        enigmaMachine.CipherWheelIndexes
            .Should()
            .NotBeNull()
            .And
            .HaveCount(4)
            .And
            .OnlyContain(x => x == 0);
        enigmaMachine.IsInitialized
            .Should()
            .BeFalse();
    }

    [Fact]
    public void CreateNewEnigmaMachineWithNoRotors_ShouldThrowException()
    {
        // Arrange
        string expected = "The Rotors collection passed into the EnigmaMachine constructor must contain at least 1 element. (Parameter 'rotors')";

        // Act
        Action action = () => _ = new EnigmaMachine(_mockReflector.Object, []);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateNewEnigmaMachineWithNullReflector_ShouldThrowException()
    {
        // Arrange/Act
        Action action = () => _ = new EnigmaMachine(null!, [_mockRotor1.Object, _mockRotor2.Object, _mockRotor3.Object]);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void CreateNewEnigmaMachineWithNullRotorInList_ShouldThrowException()
    {
        // Arrange
        string expected = "All Rotors passed into the EnigmaMachine constructor must be non-null, but the Rotor at index 1 is null. (Parameter 'rotors')";

        // Act
        Action action = () => _ = new EnigmaMachine(_mockReflector.Object, [_mockRotor1.Object, null!, _mockRotor3.Object]);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentException>()
            .WithMessage(expected);
    }

    [Fact]
    public void CreateNewEnigmaMachineWithNullRotorList_ShouldThrowException()
    {
        // Arrange/Act
        Action action = () => _ = new EnigmaMachine(_mockReflector.Object, null!);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>();
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
        enigmaMachine.IsInitialized
            .Should()
            .BeFalse();
    }

    [Fact]
    public void InitializeUsingValidSeedValue_ShouldInitialize()
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();
        int[] initialIndexes = [1, 2, 3, 4];
        enigmaMachine.SetState(null, initialIndexes);
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
        enigmaMachine.IsInitialized
            .Should()
            .BeTrue();
        enigmaMachine.CipherWheelIndexes
            .Should()
            .OnlyContain(x => x == 0);
    }

    [Fact]
    public void ResetCipherIndexesWhenInitialized_ShouldResetIndexes()
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();
        const int index1 = 1;
        const int index2 = 2;
        const int index3 = 3;
        const int index4 = 4;
        int[] indexValues = [index1, index2, index3, index4];
        enigmaMachine.SetState(true, indexValues);
        _mockRotor1
            .Setup(static m => m.SetCipherIndex(index1))
            .Verifiable(Times.Once);
        _mockRotor2
            .Setup(static m => m.SetCipherIndex(index2))
            .Verifiable(Times.Once);
        _mockRotor3
            .Setup(static m => m.SetCipherIndex(index3))
            .Verifiable(Times.Once);
        _mockReflector
            .Setup(static m => m.SetCipherIndex(index4))
            .Verifiable(Times.Once);

        // Act
        enigmaMachine.ResetCipherIndexes();

        // Assert
        VerifyAll();
    }

    [Fact]
    public void ResetCipherIndexesWhenNotInitialized_ShouldDoNothing()
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();

        // Act
        enigmaMachine.ResetCipherIndexes();

        // Assert
        VerifyAll();
    }

    [Fact]
    public void SetCipherIndexesWhenInitialized_ShouldSetIndexesToCorrectValues()
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();
        enigmaMachine.SetState(true, null);
        const int index1 = 11;
        const int index2 = 22;
        const int index3 = 33;
        const int index4 = 44;
        _mockRotor1
            .Setup(static m => m.SetCipherIndex(index1))
            .Verifiable(Times.Once);
        _mockRotor2
            .Setup(static m => m.SetCipherIndex(index2))
            .Verifiable(Times.Once);
        _mockRotor3
            .Setup(static m => m.SetCipherIndex(index3))
            .Verifiable(Times.Once);
        _mockReflector
            .Setup(static m => m.SetCipherIndex(index4))
            .Verifiable(Times.Once);

        // Act
        enigmaMachine.SetCipherIndexes(index1, index2, index3, index4);

        // Assert
        VerifyAll();
        enigmaMachine.CipherWheelIndexes
            .Should()
            .ContainInOrder(index1, index2, index3, index4);
    }

    [Fact]
    public void SetCipherIndexesWhenNotInitialized_ShouldThrowException()
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();
        int[] indexes = [1, 2, 3, 4];
        string expected = "The EnigmaMachine must be initialized before calling the SetCipherIndexes method.";

        // Act
        Action action = () => enigmaMachine.SetCipherIndexes(indexes);

        // Assert
        action
            .Should()
            .ThrowExactly<InvalidOperationException>()
            .WithMessage(expected);
        enigmaMachine.CipherWheelIndexes
            .Should()
            .OnlyContain(x => x == 0);
    }

    [Fact]
    public void SetCipherIndexesWhenParameterIsNull_ShouldThrowException()
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();
        enigmaMachine.SetState(true, null);

        // Act
        Action action = () => enigmaMachine.SetCipherIndexes(null!);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentNullException>();
    }

    [Theory]
    [InlineData(new int[] { 1, 2, 3 }, "were")]
    [InlineData(new int[] { 1, 2, 3, 4, 5 }, "were")]
    [InlineData(new int[] { 1 }, "was")]
    public void SetCipherIndexesWhenWrongNumberOfIndexValuesGiven_ShouldThrowException(int[] indexes, string word)
    {
        // Arrange
        EnigmaMachine enigmaMachine = CreateEnigmaMachine();
        enigmaMachine.SetState(true, null);
        string expected = $"Exactly 4 index values must be passed into the SetCipherIndexes method of the EnigmaMachine class, but there {word} {indexes.Length}. (Parameter 'indexes')";

        // Act
        Action action = () => enigmaMachine.SetCipherIndexes(indexes);

        // Assert
        action
            .Should()
            .ThrowExactly<ArgumentException>()
            .WithMessage(expected);
        enigmaMachine.CipherWheelIndexes
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
        enigmaMachine.SetState(true, null);
        _mockRotor1
            .Setup(m => m.Transform(adjusted))
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
        string expected = "The EnigmaMachine must be initialized before calling the Transform method.";

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
        enigmaMachine.SetState(true, null);

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
            .Setup(m => m.ConnectRightComponent(_mockRotor3.Object))
            .Verifiable(Times.Once);
        _mockRotor1
            .Setup(m => m.ConnectLeftComponent(_mockRotor2.Object))
            .Verifiable(Times.Once);
        _mockRotor2
            .Setup(m => m.ConnectRightComponent(_mockRotor1.Object))
            .Verifiable(Times.Once);
        _mockRotor2
            .Setup(m => m.ConnectLeftComponent(_mockRotor3.Object))
            .Verifiable(Times.Once);
        _mockRotor3
            .Setup(m => m.ConnectRightComponent(_mockRotor2.Object))
            .Verifiable(Times.Once);
        _mockRotor3
            .Setup(m => m.ConnectLeftComponent(_mockReflector.Object))
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