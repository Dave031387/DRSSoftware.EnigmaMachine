namespace DRSSoftware.EnigmaMachine.Utility;

public class EmbeddingServiceTests
{
    [Fact]
    public void Embed_ShouldCorrectlyEmbedWhenIndexValuesIsLongestComponent()
    {
        // Arrange
        char firstChar = MinChar;
        int inputTextLength = 8;
        int seedValueLength = 6;
        int numberOfRotors = 8;
        int numberOfIndexValues = numberOfRotors + 1;
        EmbeddedText embeddedText = new(inputTextLength, seedValueLength, numberOfIndexValues);
        EmbeddingService embeddingService = GetEmbeddingServiceForEmbed(out Mock<IIndicatorStringGenerator> mockGenerator, true, firstChar);
        string indicatorString = GetIndicatorString(firstChar);
        string expected = indicatorString + embeddedText.Value;

        // Act
        string actual = embeddingService.Embed(embeddedText.InputText, GetEnigmaConfiguration(numberOfRotors, embeddedText.SeedValue));

        // Assert
        actual
            .Should()
            .Be(expected);
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void Embed_ShouldCorrectlyEmbedWhenInputTextContainsCRLF()
    {
        // Arrange
        char firstChar = (char)(MinChar + 12);
        int inputTextLength = 16;
        int seedValueLength = 7;
        int numberOfRotors = 3;
        int numberOfIndexValues = numberOfRotors + 1;
        EmbeddedText embeddedText = new(inputTextLength, seedValueLength, numberOfIndexValues, 5);
        EmbeddingService embeddingService = GetEmbeddingServiceForEmbed(out Mock<IIndicatorStringGenerator> mockGenerator, true, firstChar);
        string indicatorString = GetIndicatorString(firstChar);
        string expected = indicatorString + embeddedText.Value;

        // Act
        string actual = embeddingService.Embed(embeddedText.InputText, GetEnigmaConfiguration(numberOfRotors, embeddedText.SeedValue));

        // Assert
        actual
            .Should()
            .Be(expected);
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void Embed_ShouldCorrectlyEmbedWhenInputTextIsLongestComponent()
    {
        // Arrange
        char firstChar = (char)(MinChar + 10);
        int inputTextLength = 16;
        int seedValueLength = 7;
        int numberOfRotors = 3;
        int numberOfIndexValues = numberOfRotors + 1;
        EmbeddedText embeddedText = new(inputTextLength, seedValueLength, numberOfIndexValues);
        EmbeddingService embeddingService = GetEmbeddingServiceForEmbed(out Mock<IIndicatorStringGenerator> mockGenerator, true, firstChar);
        string indicatorString = GetIndicatorString(firstChar);
        string expected = indicatorString + embeddedText.Value;

        // Act
        string actual = embeddingService.Embed(embeddedText.InputText, GetEnigmaConfiguration(numberOfRotors, embeddedText.SeedValue));

        // Assert
        actual
            .Should()
            .Be(expected);
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void Embed_ShouldCorrectlyEmbedWhenSeedValueContainsCRLF()
    {
        // Arrange
        char firstChar = (char)(MinChar + 7);
        int inputTextLength = 13;
        int seedValueLength = 7;
        int numberOfRotors = 4;
        int numberOfIndexValues = numberOfRotors + 1;
        EmbeddedText embeddedText = new(inputTextLength, seedValueLength, numberOfIndexValues, 0, 5);
        EmbeddingService embeddingService = GetEmbeddingServiceForEmbed(out Mock<IIndicatorStringGenerator> mockGenerator, true, firstChar);
        string indicatorString = GetIndicatorString(firstChar);
        string expected = indicatorString + embeddedText.Value;

        // Act
        string actual = embeddingService.Embed(embeddedText.InputText, GetEnigmaConfiguration(numberOfRotors, embeddedText.SeedValue));

        // Assert
        actual
            .Should()
            .Be(expected);
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void Embed_ShouldCorrectlyEmbedWhenSeedValueIsLongestComponent()
    {
        // Arrange
        char firstChar = (char)(MinChar + 5);
        int inputTextLength = 8;
        int seedValueLength = 12;
        int numberOfRotors = 4;
        int numberOfIndexValues = numberOfRotors + 1;
        EmbeddedText embeddedText = new(inputTextLength, seedValueLength, numberOfIndexValues);
        EmbeddingService embeddingService = GetEmbeddingServiceForEmbed(out Mock<IIndicatorStringGenerator> mockGenerator, true, firstChar);
        string indicatorString = GetIndicatorString(firstChar);
        string expected = indicatorString + embeddedText.Value;

        // Act
        string actual = embeddingService.Embed(embeddedText.InputText, GetEnigmaConfiguration(numberOfRotors, embeddedText.SeedValue));

        // Assert
        actual
            .Should()
            .Be(expected);
        mockGenerator.VerifyAll();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Embed_ShouldReturnEmptyStringIfInputTextIsNullOrEmpty(string? inputText)
    {
        // Arrange
        EmbeddingService embeddingService = GetEmbeddingServiceForEmbed(out Mock<IIndicatorStringGenerator> mockGenerator, false);

        // Act
        string actual = embeddingService.Embed(inputText!, GetEnigmaConfiguration(5, "Seed Value"));

        // Assert
        actual
            .Should()
            .BeEmpty();
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void Embed_ShouldReturnInputTextUnchangedIfItStartsWithAnEmbeddingIndicatorString()
    {
        // Arrange
        EmbeddingService embeddingService = GetEmbeddingServiceForEmbed(out Mock<IIndicatorStringGenerator> mockGenerator, false);
        char[] indicatorChars = new char[IndicatorSize];

        for (int i = 0; i < IndicatorPairs; i++)
        {
            indicatorChars[i] = (char)(MinChar + i);
            indicatorChars[i + IndicatorPairs] = (char)(EmbeddedIndicatorValue - indicatorChars[i]);
        }

        string expected = GetIndicatorString(MinChar) + "Some sample text.";

        // Act
        string actual = embeddingService.Embed(expected, GetEnigmaConfiguration(3, "Seed Value"));

        // Assert
        actual
            .Should()
            .Be(expected);
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void Extract_ShouldDisplayErrorMessageWhenNumberOfRotorsIsTooLarge()
    {
        // Arrange
        int inputTextLength = 8;
        int seedValueLength = 10;
        int numberOfIndexValues = 10;
        EmbeddedText embeddedText = new(inputTextLength, seedValueLength, numberOfIndexValues);
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract(out Mock<IIndicatorStringGenerator> mockGenerator);
        string indicatorString = GetIndicatorString(MinChar);
        string embeddedTextString = indicatorString + embeddedText.Value;

        // Act
        string actualInputText = embeddingService.Extract(embeddedTextString, out EnigmaConfiguration? configuration);

        // Assert
        actualInputText
            .Should()
            .Be(EmbeddingExtractErrorMessage);
        configuration
            .Should()
            .BeNull();
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void Extract_ShouldDisplayErrorMessageWhenNumberOfRotorsIsTooSmall()
    {
        // Arrange
        int inputTextLength = 8;
        int seedValueLength = 10;
        int numberOfIndexValues = 3;
        EmbeddedText embeddedText = new(inputTextLength, seedValueLength, numberOfIndexValues);
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract(out Mock<IIndicatorStringGenerator> mockGenerator);
        string indicatorString = GetIndicatorString(MinChar);
        string embeddedTextString = indicatorString + embeddedText.Value;

        // Act
        string actualInputText = embeddingService.Extract(embeddedTextString, out EnigmaConfiguration? configuration);

        // Assert
        actualInputText
            .Should()
            .Be(EmbeddingExtractErrorMessage);
        configuration
            .Should()
            .BeNull();
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void Extract_ShouldDisplayErrorMessageWhenSeedValueIsTooShort()
    {
        // Arrange
        int inputTextLength = 8;
        int seedValueLength = 9;
        int numberOfIndexValues = 9;
        EmbeddedText embeddedText = new(inputTextLength, seedValueLength, numberOfIndexValues);
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract(out Mock<IIndicatorStringGenerator> mockGenerator);
        string indicatorString = GetIndicatorString(MinChar);
        string embeddedTextString = indicatorString + embeddedText.Value;

        // Act
        string actualInputText = embeddingService.Extract(embeddedTextString, out EnigmaConfiguration? configuration);

        // Assert
        actualInputText
            .Should()
            .Be(EmbeddingExtractErrorMessage);
        configuration
            .Should()
            .BeNull();
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void Extract_ShouldExtractConfigurationWhenInputTextContainsCRLF()
    {
        // Arrange
        int inputTextLength = 16;
        int seedValueLength = 10;
        int numberOfRotors = 3;
        int numberOfIndexValues = numberOfRotors + 1;
        EmbeddedText embeddedText = new(inputTextLength, seedValueLength, numberOfIndexValues, 6);
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract(out Mock<IIndicatorStringGenerator> mockGenerator);
        string indicatorString = GetIndicatorString((char)(MinChar + 12));
        string embeddedTextString = indicatorString + embeddedText.Value;

        // Act
        string actualInputText = embeddingService.Extract(embeddedTextString, out EnigmaConfiguration? configuration);

        // Assert
        actualInputText
            .Should()
            .Be(embeddedText.InputText);
        AssertEnigmaConfiguration(configuration, embeddedText.SeedValue, numberOfRotors);
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void Extract_ShouldExtractConfigurationWhenInputTextIsLongestComponent()
    {
        // Arrange
        int inputTextLength = 16;
        int seedValueLength = 10;
        int numberOfRotors = 3;
        int numberOfIndexValues = numberOfRotors + 1;
        EmbeddedText embeddedText = new(inputTextLength, seedValueLength, numberOfIndexValues);
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract(out Mock<IIndicatorStringGenerator> mockGenerator);
        string indicatorString = GetIndicatorString((char)(MinChar + 10));
        string embeddedTextString = indicatorString + embeddedText.Value;

        // Act
        string actualInputText = embeddingService.Extract(embeddedTextString, out EnigmaConfiguration? configuration);

        // Assert
        actualInputText
            .Should()
            .Be(embeddedText.InputText);
        AssertEnigmaConfiguration(configuration, embeddedText.SeedValue, numberOfRotors);
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void Extract_ShouldExtractConfigurationWhenSeedValueContainsCRLF()
    {
        // Arrange
        int inputTextLength = 13;
        int seedValueLength = 10;
        int numberOfRotors = 4;
        int numberOfIndexValues = numberOfRotors + 1;
        EmbeddedText embeddedText = new(inputTextLength, seedValueLength, numberOfIndexValues, 0, 8);
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract(out Mock<IIndicatorStringGenerator> mockGenerator);
        string indicatorString = GetIndicatorString((char)(MinChar + 7));
        string embeddedTextString = indicatorString + embeddedText.Value;

        // Act
        string actualInputText = embeddingService.Extract(embeddedTextString, out EnigmaConfiguration? configuration);

        // Assert
        actualInputText
            .Should()
            .Be(embeddedText.InputText);
        AssertEnigmaConfiguration(configuration, embeddedText.SeedValue, numberOfRotors);
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void Extract_ShouldExtractConfigurationWhenSeedValueIsLongestComponent()
    {
        // Arrange
        int inputTextLength = 8;
        int seedValueLength = 12;
        int numberOfRotors = 7;
        int numberOfIndexValues = numberOfRotors + 1;
        EmbeddedText embeddedText = new(inputTextLength, seedValueLength, numberOfIndexValues);
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract(out Mock<IIndicatorStringGenerator> mockGenerator);
        string indicatorString = GetIndicatorString((char)(MinChar + 5));
        string embeddedTextString = indicatorString + embeddedText.Value;

        // Act
        string actualInputText = embeddingService.Extract(embeddedTextString, out EnigmaConfiguration? configuration);

        // Assert
        actualInputText
            .Should()
            .Be(embeddedText.InputText);
        AssertEnigmaConfiguration(configuration, embeddedText.SeedValue, numberOfRotors);
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void Extract_ShouldReturnEmbeddedTextUnchangedIfLengthIsTooSmall()
    {
        // Arrange
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract(out Mock<IIndicatorStringGenerator> mockGenerator);
        string inputText = GetIndicatorString(MinChar) + new string('X', MinEmbeddedStringSize - IndicatorSize - 1);

        // Act
        string actual = embeddingService.Extract(inputText, out EnigmaConfiguration? configuration);

        // Assert
        actual
            .Should()
            .Be(inputText);
        configuration
            .Should()
            .BeNull();
        mockGenerator.VerifyAll();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Extract_ShouldReturnEmptyStringIfEmbeddedTextIsNullOrEmpty(string? inputText)
    {
        // Arrange
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract(out Mock<IIndicatorStringGenerator> mockGenerator);

        // Act
        string actual = embeddingService.Extract(inputText!, out EnigmaConfiguration? configuration);

        // Assert
        actual
            .Should()
            .BeEmpty();
        configuration
            .Should()
            .BeNull();
        mockGenerator.VerifyAll();
    }

    [Fact]
    public void Extract_ShouldReturnInputTextUnchangedWhenNoIndicatorStringIsFound()
    {
        // Arrange
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract(out Mock<IIndicatorStringGenerator> mockGenerator);
        string inputText = new('X', MinEmbeddedStringSize);

        // Act
        string actual = embeddingService.Extract(inputText, out EnigmaConfiguration? configuration);

        // Assert
        actual
            .Should()
            .Be(inputText);
        configuration
            .Should()
            .BeNull();
        mockGenerator.VerifyAll();
    }

    private static void AssertEnigmaConfiguration(EnigmaConfiguration? configuration, string expectedSeedValue, int expectedNumberOfRotors)
    {
        configuration
            .Should()
            .NotBeNull();
        configuration.SeedValue
            .Should()
            .Be(expectedSeedValue);
        configuration.NumberOfRotors
            .Should()
            .Be(expectedNumberOfRotors);
        configuration.ReflectorIndex
            .Should()
            .Be('0' - MinChar);

        if (expectedNumberOfRotors > 0)
        {
            configuration.RotorIndex1
                .Should()
                .Be('1' - MinChar);
        }

        if (expectedNumberOfRotors > 1)
        {
            configuration.RotorIndex2
                .Should()
                .Be('2' - MinChar);
        }

        if (expectedNumberOfRotors > 2)
        {
            configuration.RotorIndex3
                .Should()
                .Be('3' - MinChar);
        }

        if (expectedNumberOfRotors > 3)
        {
            configuration.RotorIndex4
                .Should()
                .Be('4' - MinChar);
        }

        if (expectedNumberOfRotors > 4)
        {
            configuration.RotorIndex5
                .Should()
                .Be('5' - MinChar);
        }

        if (expectedNumberOfRotors > 5)
        {
            configuration.RotorIndex6
                .Should()
                .Be('6' - MinChar);
        }

        if (expectedNumberOfRotors > 6)
        {
            configuration.RotorIndex7
                .Should()
                .Be('7' - MinChar);
        }

        if (expectedNumberOfRotors > 7)
        {
            configuration.RotorIndex8
                .Should()
                .Be('8' - MinChar);
        }

        configuration.UseEmbeddedConfiguration
            .Should()
            .BeFalse();
    }

    private static EmbeddingService GetEmbeddingServiceForEmbed(out Mock<IIndicatorStringGenerator> mockGenerator, bool shouldGenerateIndicatorString, params char[] firstChar)
    {
        mockGenerator = new(MockBehavior.Strict);

        if (shouldGenerateIndicatorString)
        {
            List<string> sequence = [];

            if (firstChar.Length is 0)
            {
                sequence.Add(GetIndicatorString(MinChar));
            }
            else
            {
                for (int j = 0; j < firstChar.Length; j++)
                {
                    sequence.Add(GetIndicatorString(firstChar[j]));
                }
            }

            List<string>.Enumerator enumerator = sequence.GetEnumerator();
            mockGenerator
                .Setup(m => m.GetIndicatorString(EmbeddingIndicatorChar))
                .Returns(() =>
                {
                    enumerator.MoveNext();
                    return enumerator.Current;
                })
                .Verifiable(Times.Exactly(sequence.Count));
        }
        else
        {
            mockGenerator
                .Setup(m => m.GetIndicatorString(It.IsAny<char>()))
                .Returns(string.Empty)
                .Verifiable(Times.Never);
        }

        return new(mockGenerator.Object);
    }

    private static EmbeddingService GetEmbeddingServiceForExtract(out Mock<IIndicatorStringGenerator> mockGenerator)
    {
        mockGenerator = new(MockBehavior.Strict);
        mockGenerator.Setup(static m => m.GetIndicatorString(It.IsAny<char>()))
            .Returns(string.Empty)
            .Verifiable(Times.Never);
        return new(mockGenerator.Object);
    }

    private static EnigmaConfiguration GetEnigmaConfiguration(int numberOfRotors, string seedValue)
    {
        return new()
        {
            NumberOfRotors = numberOfRotors,
            ReflectorIndex = '0' - MinChar,
            RotorIndex1 = '1' - MinChar,
            RotorIndex2 = '2' - MinChar,
            RotorIndex3 = '3' - MinChar,
            RotorIndex4 = '4' - MinChar,
            RotorIndex5 = '5' - MinChar,
            RotorIndex6 = '6' - MinChar,
            RotorIndex7 = '7' - MinChar,
            RotorIndex8 = '8' - MinChar,
            SeedValue = seedValue
        };
    }

    private static string GetIndicatorString(char firstChar)
    {
        char[] indicatorChars = new char[IndicatorSize];
        for (int i = 0; i < IndicatorPairs; i++)
        {
            indicatorChars[i] = (char)(firstChar + i);
            indicatorChars[i + IndicatorPairs] = (char)(EmbeddedIndicatorValue - indicatorChars[i]);
        }
        return new string(indicatorChars);
    }
}

public class EmbeddedText
{
    private readonly char[] _indexValueChars = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];
    private readonly char[] _inputTextChars = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];
    private readonly char[] _seedValueChars = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'];

    public EmbeddedText(int inputTextLength, int seedValueLength, int numberOfIndexValues, int inputCRLF = 0, int seedCRLF = 0)
    {
        int maxIndex = inputTextLength > seedValueLength
            ? inputTextLength > numberOfIndexValues
                ? inputTextLength
                : numberOfIndexValues
            : seedValueLength > numberOfIndexValues
                ? seedValueLength
                : numberOfIndexValues;
        List<char> embeddedChars = [];
        List<char> inputChars = [];
        List<char> seedChars = [];

        for (int i = 0; i < maxIndex; i++)
        {
            if (i < inputTextLength)
            {
                if (inputCRLF > 0 && i == inputCRLF)
                {
                    inputChars.AddRange(CRLF);
                    embeddedChars.AddRange(CRLF);
                }
                else
                {
                    inputChars.Add(_inputTextChars[i]);
                    embeddedChars.Add(_inputTextChars[i]);
                }
            }
            else if (i == inputTextLength && i < maxIndex)
            {
                embeddedChars.Add(DelimiterChar);
            }

            if (i < seedValueLength)
            {
                if (seedCRLF > 0 && i == seedCRLF)
                {
                    seedChars.AddRange(CRLF);
                    embeddedChars.AddRange(CRLF);
                }
                else
                {
                    seedChars.Add(_seedValueChars[i]);
                    embeddedChars.Add(_seedValueChars[i]);
                }
            }
            else if (i == seedValueLength && i < maxIndex)
            {
                embeddedChars.Add(DelimiterChar);
            }

            if (i < numberOfIndexValues)
            {
                embeddedChars.Add(_indexValueChars[i]);
            }
            else if (i == numberOfIndexValues && i < maxIndex)
            {
                embeddedChars.Add(DelimiterChar);
            }
        }

        InputText = new([.. inputChars]);
        SeedValue = new([.. seedChars]);
        Value = new([.. embeddedChars]);
    }

    public string InputText
    {
        get;
    }

    public string SeedValue
    {
        get;
    }

    public string Value
    {
        get;
    }
}