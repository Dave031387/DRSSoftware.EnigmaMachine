namespace DRSSoftware.EnigmaMachine.ViewModels;

using DRSSoftware.EnigmaMachine.Utility;

[ExcludeFromCodeCoverage]
public class ConfigurationDialogViewModelTests
{
    [Theory]
    [InlineData(null)]
    [InlineData(" \r\n\t")]
    [InlineData("ABCDEFGHI")]
    [InlineData("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345")]
    public void AcceptCommandCanExecute_ShouldBeFalseIfSeedValueIsNullOrWhiteSpaceOrInvalidLength(string? seedValue)
    {
        // Arrange
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);

        // Act
        ConfigurationDialogViewModel viewModel = new(mockNumberGenerator.Object)
        {
            SeedValue = seedValue!
        };

        // Assert
        viewModel.AcceptCommand.CanExecute(null)
            .Should()
            .BeFalse();
        mockNumberGenerator.VerifyAll();
    }

    [Theory]
    [InlineData("ABCDEFGHIJ")]
    [InlineData("1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234")]
    public void AcceptCommandCanExecute_ShouldBeTrueIfSeedValueIsValidLength(string seedValue)
    {
        // Arrange
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);

        // Act
        ConfigurationDialogViewModel viewModel = new(mockNumberGenerator.Object)
        {
            SeedValue = seedValue
        };

        // Assert
        viewModel.AcceptCommand.CanExecute(null)
            .Should()
            .BeTrue();
        mockNumberGenerator.VerifyAll();
    }

    [Fact]
    public void AcceptCommandExecute_ShouldSaveConfigurationAndSetCloseTriggerPropertyToTrue()
    {
        // Arrange
        int numberOfRotors = 5;
        int reflectorIndex = 10;
        int rotorIndex1 = 11;
        int rotorIndex2 = 12;
        int rotorIndex3 = 13;
        int rotorIndex4 = 14;
        int rotorIndex5 = 15;
        int rotorIndex6 = 16;
        int rotorIndex7 = 17;
        int rotorIndex8 = 18;
        string seedValue = "sample seed value";
        bool useEmbeddedConfiguration = true;
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);
        ConfigurationDialogViewModel viewModel = new(mockNumberGenerator.Object)
        {
            ReflectorIndex = reflectorIndex,
            RotorIndex1 = rotorIndex1,
            RotorIndex2 = rotorIndex2,
            RotorIndex3 = rotorIndex3,
            RotorIndex4 = rotorIndex4,
            RotorIndex5 = rotorIndex5,
            RotorIndex6 = rotorIndex6,
            RotorIndex7 = rotorIndex7,
            RotorIndex8 = rotorIndex8,
            SeedValue = seedValue,
            SelectedRotorCount = numberOfRotors,
            UseEmbeddedConfiguration = useEmbeddedConfiguration
        };
        EnigmaConfiguration expectedEnigmaConfiguration = new()
        {
            NumberOfRotors = numberOfRotors,
            ReflectorIndex = reflectorIndex,
            RotorIndex1 = rotorIndex1,
            RotorIndex2 = rotorIndex2,
            RotorIndex3 = rotorIndex3,
            RotorIndex4 = rotorIndex4,
            RotorIndex5 = rotorIndex5,
            RotorIndex6 = rotorIndex6,
            RotorIndex7 = rotorIndex7,
            RotorIndex8 = rotorIndex8,
            SeedValue = seedValue,
            UseEmbeddedConfiguration = useEmbeddedConfiguration
        };
        bool expectedCloseTrigger = true;

        // Act
        viewModel.AcceptCommand.Execute(null);

        // Assert
        viewModel.EnigmaConfiguration
            .Should()
            .BeEquivalentTo(expectedEnigmaConfiguration);
        viewModel.CloseTrigger
            .Should()
            .Be(expectedCloseTrigger);
        mockNumberGenerator.VerifyAll();
    }

    [Fact]
    public void CancelCommandCanExecute_ShouldAlwaysReturnTrue()
    {
        // Arrange
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);
        ConfigurationDialogViewModel viewModel = new(mockNumberGenerator.Object);

        // Act
        bool actual = viewModel.CancelCommand.CanExecute(null);

        // Assert
        actual
            .Should()
            .BeTrue();
        mockNumberGenerator.VerifyAll();
    }

    [Fact]
    public void CancelCommandExecute_ShouldSetCloseTriggerPropertyToTrueWithoutSavingTheConfiguration()
    {
        // Arrange
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);
        ConfigurationDialogViewModel viewModel = new(mockNumberGenerator.Object)
        {
            ReflectorIndex = 10,
            RotorIndex1 = 11,
            RotorIndex2 = 12,
            RotorIndex3 = 13,
            RotorIndex4 = 14,
            RotorIndex5 = 15,
            RotorIndex6 = 16,
            RotorIndex7 = 17,
            RotorIndex8 = 18,
            SeedValue = "sample seed value",
            SelectedRotorCount = 6,
            UseEmbeddedConfiguration = true
        };
        EnigmaConfiguration expectedEnigmaConfiguration = new();
        bool expectedCloseTrigger = true;

        // Act
        viewModel.CancelCommand.Execute(null);

        // Assert
        viewModel.EnigmaConfiguration
            .Should()
            .BeEquivalentTo(expectedEnigmaConfiguration);
        viewModel.CloseTrigger
            .Should()
            .Be(expectedCloseTrigger);
        mockNumberGenerator.VerifyAll();
    }

    [Theory]
    [InlineData(null, true)]
    [InlineData(null, false)]
    [InlineData("", true)]
    [InlineData("", false)]
    [InlineData("seed value", true)]
    public void ClearCommandCanExecute_ShouldBeFalseIfSeedIsNullOrEmptyOrIsAutoSeedSelectedIsTrue(string? seedValue, bool isAutoSeedSelected)
    {
        // Arrange
        List<int> sequence = ['s', 'e', 'e', 'd', ' ', 'v', 'a', 'l', 'u', 'e'];
        List<int>.Enumerator enumerator = sequence.GetEnumerator();
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);
        mockNumberGenerator
            .Setup(m => m.GetNext(32, MaxValue - 1))
            .Returns(sequence.Count)
            .Verifiable(Times.AtMost(1));
        mockNumberGenerator
            .Setup(m => m.GetNext(MinChar, MaxChar))
            .Returns(() =>
            {
                enumerator.MoveNext();
                return enumerator.Current;
            })
            .Verifiable(Times.AtMost(sequence.Count));

        // Act
        ConfigurationDialogViewModel viewModel = new(mockNumberGenerator.Object)
        {
            SeedValue = seedValue!,
            IsAutoSeedSelected = isAutoSeedSelected
        };

        // Assert
        viewModel.ClearCommand.CanExecute(null)
            .Should()
            .BeFalse();
        mockNumberGenerator.VerifyAll();
    }

    [Fact]
    public void ClearCommandCanExecute_ShouldBeTrueIfSeedIsNotNullOrEmptyAndIsAutoSeedSelectedIsFalse()
    {
        // Arrange
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);

        // Act
        ConfigurationDialogViewModel viewModel = new(mockNumberGenerator.Object)
        {
            SeedValue = "seed value"
        };

        // Assert
        viewModel.ClearCommand.CanExecute(null)
            .Should()
            .BeTrue();
        mockNumberGenerator.VerifyAll();
    }

    [Fact]
    public void ClearCommandExecute_ShouldSetTheSeedValuePropertyToAnEmptyString()
    {
        // Arrange
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);
        ConfigurationDialogViewModel viewModel = new(mockNumberGenerator.Object)
        {
            SeedValue = "sample seed value"
        };

        // Act
        viewModel.ClearCommand.Execute(null);

        // Assert
        viewModel.SeedValue
            .Should()
            .BeEmpty();
        mockNumberGenerator.VerifyAll();
    }

    [Fact]
    public void ConfigurationDialogViewModel_ShouldInitializePropertiesCorrectlyWhenConstructed()
    {
        // Arrange
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);

        // Act
        ConfigurationDialogViewModel viewModel = new(mockNumberGenerator.Object);

        // Assert
        viewModel.AcceptCommand
            .Should()
            .NotBeNull();
        viewModel.AvailableIndexValues
            .Should()
            .NotBeNull()
            .And
            .HaveCount(MaxValue - MinValue + 1);
        viewModel.AvailableRotorCounts
            .Should()
            .NotBeNull()
            .And
            .HaveCount(MaxRotorCount - MinRotorCount + 1);
        viewModel.CancelCommand
            .Should()
            .NotBeNull();
        viewModel.ClearCommand
            .Should()
            .NotBeNull();
        viewModel.CloseTrigger
            .Should()
            .BeFalse();
        viewModel.EnigmaConfiguration
            .Should()
            .NotBeNull()
            .And
            .BeEquivalentTo(new EnigmaConfiguration());
        viewModel.IsAutoIndexesSelected
            .Should()
            .BeFalse();
        viewModel.IsAutoRotorsSelected
            .Should()
            .BeFalse();
        viewModel.IsAutoSeedSelected
            .Should()
            .BeFalse();
        viewModel.IsManualIndexesSelected
            .Should()
            .BeTrue();
        viewModel.IsManualRotorsSelected
            .Should()
            .BeTrue();
        viewModel.IsManualSeedSelected
            .Should()
            .BeTrue();
        viewModel.IsRotor4Visible
            .Should()
            .BeFalse();
        viewModel.IsRotor5Visible
            .Should()
            .BeFalse();
        viewModel.IsRotor6Visible
            .Should()
            .BeFalse();
        viewModel.IsRotor7Visible
            .Should()
            .BeFalse();
        viewModel.IsRotor8Visible
            .Should()
            .BeFalse();
        viewModel.MaxSeedValueLength
            .Should()
            .Be(MaxSeedLength);
        viewModel.MinSeedValueLength
            .Should()
            .Be(MinStringLength);
        viewModel.ReflectorIndex
            .Should()
            .Be(0);
        viewModel.RotorIndex1
            .Should()
            .Be(0);
        viewModel.RotorIndex2
            .Should()
            .Be(0);
        viewModel.RotorIndex3
            .Should()
            .Be(0);
        viewModel.RotorIndex4
            .Should()
            .Be(0);
        viewModel.RotorIndex5
            .Should()
            .Be(0);
        viewModel.RotorIndex6
            .Should()
            .Be(0);
        viewModel.RotorIndex7
            .Should()
            .Be(0);
        viewModel.RotorIndex8
            .Should()
            .Be(0);
        viewModel.SeedValue
            .Should()
            .BeEmpty();
        viewModel.SelectedRotorCount
            .Should()
            .Be(MinRotorCount);
        viewModel.UseEmbeddedConfiguration
            .Should()
            .BeFalse();
        mockNumberGenerator.VerifyAll();
    }

    [Fact]
    public void Initialize_ShouldCopyConfigurationAndDisplayCorrectNumberOfRotors()
    {
        // Arrange
        int expectedNumberOfRotors = 4;
        int expectedReflectorIndex = 10;
        int expectedRotorIndex1 = 11;
        int expectedRotorIndex2 = 12;
        int expectedRotorIndex3 = 13;
        int expectedRotorIndex4 = 14;
        int expectedRotorIndex5 = 15;
        int expectedRotorIndex6 = 16;
        int expectedRotorIndex7 = 17;
        int expectedRotorIndex8 = 18;
        string expectedSeedValue = "seed value";
        bool expectedUseEmbeddedConfiguration = true;
        EnigmaConfiguration expectedEnigmaConfiguration = new()
        {
            NumberOfRotors = expectedNumberOfRotors,
            ReflectorIndex = expectedReflectorIndex,
            RotorIndex1 = expectedRotorIndex1,
            RotorIndex2 = expectedRotorIndex2,
            RotorIndex3 = expectedRotorIndex3,
            RotorIndex4 = expectedRotorIndex4,
            RotorIndex5 = expectedRotorIndex5,
            RotorIndex6 = expectedRotorIndex6,
            RotorIndex7 = expectedRotorIndex7,
            RotorIndex8 = expectedRotorIndex8,
            SeedValue = expectedSeedValue,
            UseEmbeddedConfiguration = expectedUseEmbeddedConfiguration
        };
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);
        ConfigurationDialogViewModel viewModel = new(mockNumberGenerator.Object);

        // Act
        viewModel.Initialize(expectedEnigmaConfiguration);

        // Assert
        viewModel.EnigmaConfiguration
            .Should()
            .BeEquivalentTo(expectedEnigmaConfiguration);
        viewModel.SelectedRotorCount
            .Should()
            .Be(expectedNumberOfRotors);
        viewModel.ReflectorIndex
            .Should()
            .Be(expectedReflectorIndex);
        viewModel.RotorIndex1
            .Should()
            .Be(expectedRotorIndex1);
        viewModel.RotorIndex2
            .Should()
            .Be(expectedRotorIndex2);
        viewModel.RotorIndex3
            .Should()
            .Be(expectedRotorIndex3);
        viewModel.RotorIndex4
            .Should()
            .Be(expectedRotorIndex4);
        viewModel.RotorIndex5
            .Should()
            .Be(expectedRotorIndex5);
        viewModel.RotorIndex6
            .Should()
            .Be(expectedRotorIndex6);
        viewModel.RotorIndex7
            .Should()
            .Be(expectedRotorIndex7);
        viewModel.RotorIndex8
            .Should()
            .Be(expectedRotorIndex8);
        viewModel.SeedValue
            .Should()
            .Be(expectedSeedValue);
        viewModel.UseEmbeddedConfiguration
            .Should()
            .Be(expectedUseEmbeddedConfiguration);
        viewModel.IsRotor4Visible
            .Should()
            .Be(expectedNumberOfRotors >= 4);
        viewModel.IsRotor5Visible
            .Should()
            .Be(expectedNumberOfRotors >= 5);
        viewModel.IsRotor6Visible
            .Should()
            .Be(expectedNumberOfRotors >= 6);
        viewModel.IsRotor7Visible
            .Should()
            .Be(expectedNumberOfRotors >= 7);
        viewModel.IsRotor8Visible
            .Should()
            .Be(expectedNumberOfRotors >= 8);
        mockNumberGenerator.VerifyAll();
    }

    [Fact]
    public void IsAutoIndexesSelected_ShouldGenerateRandomIndexValuesWhenPropertyIsSetToTrue()
    {
        // Arrange
        List<int> sequence = [1, 2, 3, 4, 5, 6, 7, 8, 9];
        List<int>.Enumerator enumerator = sequence.GetEnumerator();
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);
        mockNumberGenerator
            .Setup(m => m.GetNext(MinValue, MaxValue + 1))
            .Returns(() =>
            {
                enumerator.MoveNext();
                return enumerator.Current;
            })
            .Verifiable(Times.Exactly(sequence.Count));

        // Act
        ConfigurationDialogViewModel viewModel = new(mockNumberGenerator.Object)
        {
            IsAutoIndexesSelected = true
        };

        // Assert
        viewModel.ReflectorIndex
            .Should()
            .Be(1);
        viewModel.RotorIndex1
            .Should()
            .Be(2);
        viewModel.RotorIndex2
            .Should()
            .Be(3);
        viewModel.RotorIndex3
            .Should()
            .Be(4);
        viewModel.RotorIndex4
            .Should()
            .Be(5);
        viewModel.RotorIndex5
            .Should()
            .Be(6);
        viewModel.RotorIndex6
            .Should()
            .Be(7);
        viewModel.RotorIndex7
            .Should()
            .Be(8);
        viewModel.RotorIndex8
            .Should()
            .Be(9);
        mockNumberGenerator.VerifyAll();
    }

    [Fact]
    public void IsAutoIndexesSelected_ShouldLeaveIndexValuesUnchangedWhenPropertyIsSetToFalse()
    {
        // Arrange
        List<int> sequence = [1, 2, 3, 4, 5, 6, 7, 8, 9, 11, 12, 13, 14, 15, 16, 17, 18, 19];
        List<int>.Enumerator enumerator = sequence.GetEnumerator();
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);
        mockNumberGenerator
            .Setup(m => m.GetNext(MinValue, MaxValue + 1))
            .Returns(() =>
            {
                enumerator.MoveNext();
                return enumerator.Current;
            })
            .Verifiable(Times.Exactly(sequence.Count / 2));
        ConfigurationDialogViewModel viewModel = new(mockNumberGenerator.Object)
        {
            IsAutoIndexesSelected = true
        };

        // Act
        viewModel.IsAutoIndexesSelected = false;

        // Assert
        viewModel.ReflectorIndex
            .Should()
            .Be(1);
        viewModel.RotorIndex1
            .Should()
            .Be(2);
        viewModel.RotorIndex2
            .Should()
            .Be(3);
        viewModel.RotorIndex3
            .Should()
            .Be(4);
        viewModel.RotorIndex4
            .Should()
            .Be(5);
        viewModel.RotorIndex5
            .Should()
            .Be(6);
        viewModel.RotorIndex6
            .Should()
            .Be(7);
        viewModel.RotorIndex7
            .Should()
            .Be(8);
        viewModel.RotorIndex8
            .Should()
            .Be(9);
        mockNumberGenerator.VerifyAll();
    }

    [Fact]
    public void IsAutoRotorsSelected_ShouldGenerateRandomNumberOfRotorsWhenPropertyIsSetToTrue()
    {
        // Arrange
        int expectedRotorCount = MaxRotorCount - 1;
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);
        mockNumberGenerator
            .Setup(m => m.GetNext(MinRotorCount, MaxRotorCount + 1))
            .Returns(expectedRotorCount)
            .Verifiable(Times.Once);

        // Act
        ConfigurationDialogViewModel viewModel = new(mockNumberGenerator.Object)
        {
            IsAutoRotorsSelected = true
        };

        // Assert
        viewModel.SelectedRotorCount
            .Should()
            .Be(expectedRotorCount);
        mockNumberGenerator.VerifyAll();
    }

    [Fact]
    public void IsAutoRotorsSelected_ShouldLeaveNumberOfRotorsUnchangedWhenPropertyIsSetToFalse()
    {
        // Arrange
        int expectedRotorCount = MinRotorCount;
        List<int> sequence = [expectedRotorCount, MaxRotorCount - 1];
        List<int>.Enumerator enumerator = sequence.GetEnumerator();
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);
        mockNumberGenerator
            .Setup(m => m.GetNext(MinRotorCount, MaxRotorCount + 1))
            .Returns(() =>
            {
                enumerator.MoveNext();
                return enumerator.Current;
            })
            .Verifiable(Times.Once);
        ConfigurationDialogViewModel viewModel = new(mockNumberGenerator.Object)
        {
            IsAutoRotorsSelected = true
        };

        // Act
        viewModel.IsAutoRotorsSelected = false;

        // Assert
        viewModel.SelectedRotorCount
            .Should()
            .Be(expectedRotorCount);
        mockNumberGenerator.VerifyAll();
    }

    [Fact]
    public void IsAutoSeedSelected_ShouldGenerateRandomSeedValueWhenPropertyIsSetToTrue()
    {
        // Arrange
        List<int> sequence = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J'];
        List<int>.Enumerator enumerator = sequence.GetEnumerator();
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);
        mockNumberGenerator
            .Setup(m => m.GetNext(32, MaxValue - 1))
            .Returns(sequence.Count)
            .Verifiable(Times.Once);
        mockNumberGenerator
            .Setup(m => m.GetNext(MinChar, MaxChar))
            .Returns(() =>
            {
                enumerator.MoveNext();
                return enumerator.Current;
            })
            .Verifiable(Times.Exactly(sequence.Count));
        ConfigurationDialogViewModel viewModel = new(mockNumberGenerator.Object);
        string expectedSeedValue = "ABCDEFGHIJ";

        // Act
        viewModel.IsAutoSeedSelected = true;

        // Assert
        viewModel.SeedValue
            .Should()
            .Be(expectedSeedValue);
        mockNumberGenerator.VerifyAll();
    }

    [Fact]
    public void IsAutoSeedSelected_ShouldLeaveSeedValueUnchangedWhenPropertyIsSetToFalse()
    {
        // Arrange
        List<int> sequence = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T'];
        List<int>.Enumerator enumerator = sequence.GetEnumerator();
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);
        mockNumberGenerator
            .Setup(m => m.GetNext(32, MaxValue - 1))
            .Returns(sequence.Count / 2)
            .Verifiable(Times.Once);
        mockNumberGenerator
            .Setup(m => m.GetNext(MinChar, MaxChar))
            .Returns(() =>
            {
                enumerator.MoveNext();
                return enumerator.Current;
            })
            .Verifiable(Times.Exactly(sequence.Count / 2));
        ConfigurationDialogViewModel viewModel = new(mockNumberGenerator.Object)
        {
            IsAutoSeedSelected = true
        };
        string expectedSeedValue = "ABCDEFGHIJ";

        // Act
        viewModel.IsAutoSeedSelected = false;

        // Assert
        viewModel.SeedValue
            .Should()
            .Be(expectedSeedValue);
        mockNumberGenerator.VerifyAll();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    [InlineData(8)]
    public void SelectedRotorCount_ShouldShowCorrectNumberOfRotorsWhenSet(int selectedRotorCount)
    {
        // Arrange
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);

        // Act
        ConfigurationDialogViewModel viewModel = new(mockNumberGenerator.Object)
        {
            SelectedRotorCount = selectedRotorCount
        };

        // Assert
        viewModel.IsRotor4Visible
            .Should()
            .Be(selectedRotorCount > 3);
        viewModel.IsRotor5Visible
            .Should()
            .Be(selectedRotorCount > 4);
        viewModel.IsRotor6Visible
            .Should()
            .Be(selectedRotorCount > 5);
        viewModel.IsRotor7Visible
            .Should()
            .Be(selectedRotorCount > 6);
        viewModel.IsRotor8Visible
            .Should()
            .Be(selectedRotorCount > 7);
        mockNumberGenerator.VerifyAll();
    }
}