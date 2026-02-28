namespace DRSSoftware.EnigmaMachine.Utility;

public class EnigmaMachineBuilderTests
{
    [Fact]
    public void Build_ShouldReturnInitializedEnigmaMachineIfSeedValueIsNotNullOrEmpty()
    {
        // Arrange
        int numberOfRotors = 5;
        string seedValue = "seedValue!";
        EnigmaConfiguration configuration = new()
        {
            NumberOfRotors = numberOfRotors,
            ReflectorIndex = 0,
            RotorIndex1 = 0,
            RotorIndex2 = 0,
            RotorIndex3 = 0,
            RotorIndex4 = 0,
            RotorIndex5 = 0,
            RotorIndex6 = 1,
            RotorIndex7 = 2,
            RotorIndex8 = 3,
            SeedValue = seedValue,
            UseEmbeddedConfiguration = false
        };
        object[] parameters = [numberOfRotors];
        Mock<IEnigmaMachine> mockEnigmaMachine = new(MockBehavior.Strict);
        mockEnigmaMachine
            .Setup(m => m.Initialize(seedValue))
            .Verifiable(Times.Once);
        Mock<IContainer> mockContainer = new(MockBehavior.Strict);
        mockContainer
            .Setup(m => m.Resolve<IEnigmaMachine>(parameters))
            .Returns(mockEnigmaMachine.Object)
            .Verifiable(Times.Once);
        EnigmaMachineBuilder enigmaMachineBuilder = new(mockContainer.Object);

        // Act
        IEnigmaMachine actual = enigmaMachineBuilder.Build(configuration);

        // Assert
        actual
            .Should()
            .NotBeNull()
            .And
            .BeSameAs(mockEnigmaMachine.Object);
        mockContainer.VerifyAll();
        mockEnigmaMachine.VerifyAll();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Build_ShouldReturnUninitializedEnigmaMachineIfSeedValueIsNullOrEmpty(string? seedValue)
    {
        // Arrange
        int numberOfRotors = 4;
        EnigmaConfiguration configuration = new()
        {
            NumberOfRotors = numberOfRotors,
            ReflectorIndex = 1,
            RotorIndex1 = 2,
            RotorIndex2 = 3,
            RotorIndex3 = 4,
            RotorIndex4 = 5,
            RotorIndex5 = 6,
            RotorIndex6 = 7,
            RotorIndex7 = 8,
            RotorIndex8 = 9,
            SeedValue = seedValue!,
            UseEmbeddedConfiguration = false
        };
        object[] parameters = [numberOfRotors];
        Mock<IEnigmaMachine> mockEnigmaMachine = new(MockBehavior.Strict);
        Mock<IContainer> mockContainer = new(MockBehavior.Strict);
        mockContainer
            .Setup(m => m.Resolve<IEnigmaMachine>(parameters))
            .Returns(mockEnigmaMachine.Object)
            .Verifiable(Times.Once);
        EnigmaMachineBuilder enigmaMachineBuilder = new(mockContainer.Object);

        // Act
        IEnigmaMachine actual = enigmaMachineBuilder.Build(configuration);

        // Assert
        actual
            .Should()
            .NotBeNull()
            .And
            .BeSameAs(mockEnigmaMachine.Object);
        mockContainer.VerifyAll();
        mockEnigmaMachine.VerifyAll();
    }

    [Fact]
    public void Build_ShouldSetCipherIndexesWhenNeeded()
    {
        // Arrange
        int numberOfRotors = 3;
        int reflectorIndex = 1;
        int rotorIndex1 = 2;
        int rotorIndex2 = 3;
        int rotorIndex3 = 4;
        string seedValue = "seedValue!";
        int[] indexes = [rotorIndex1, rotorIndex2, rotorIndex3, reflectorIndex];
        EnigmaConfiguration configuration = new()
        {
            NumberOfRotors = numberOfRotors,
            ReflectorIndex = reflectorIndex,
            RotorIndex1 = rotorIndex1,
            RotorIndex2 = rotorIndex2,
            RotorIndex3 = rotorIndex3,
            RotorIndex4 = 5,
            RotorIndex5 = 6,
            RotorIndex6 = 7,
            RotorIndex7 = 8,
            RotorIndex8 = 9,
            SeedValue = seedValue,
            UseEmbeddedConfiguration = false
        };
        object[] parameters = [numberOfRotors];
        Mock<IEnigmaMachine> mockEnigmaMachine = new(MockBehavior.Strict);
        mockEnigmaMachine
            .Setup(m => m.Initialize(seedValue))
            .Verifiable(Times.Once);
        mockEnigmaMachine
            .Setup(m => m.SetCipherIndexes(indexes))
            .Verifiable(Times.Once);
        Mock<IContainer> mockContainer = new(MockBehavior.Strict);
        mockContainer
            .Setup(m => m.Resolve<IEnigmaMachine>(parameters))
            .Returns(mockEnigmaMachine.Object)
            .Verifiable(Times.Once);
        EnigmaMachineBuilder enigmaMachineBuilder = new(mockContainer.Object);

        // Act
        IEnigmaMachine actual = enigmaMachineBuilder.Build(configuration);

        // Assert
        actual
            .Should()
            .NotBeNull()
            .And
            .BeSameAs(mockEnigmaMachine.Object);
        mockContainer.VerifyAll();
        mockEnigmaMachine.VerifyAll();
    }
}