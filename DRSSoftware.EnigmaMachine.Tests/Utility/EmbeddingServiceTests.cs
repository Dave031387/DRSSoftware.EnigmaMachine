namespace DRSSoftware.EnigmaMachine.Utility;

public class EmbeddingServiceTests
{
    [Fact]
    public void Embed_ShouldCorrectlyEmbedWhenIndexValuesIsLongestComponent()
    {
        // Arrange
        char firstChar = MinChar;
        EmbeddingService embeddingService = GetEmbeddingServiceForEmbed(firstChar);
        string indicatorString = GetIndicatorString(firstChar);
        string inputText = "ABCDEFGH";
        string seedValue = "abcdef";
        string expected = indicatorString + "Aa0Bb1Cc2Dd3Ee4Ff5G" + DelimiterChar + "6H7" + DelimiterChar + "8";

        // Act
        string actual = embeddingService.Embed(inputText, GetEnigmaConfiguration(8, seedValue));

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }

    [Fact]
    public void Embed_ShouldCorrectlyEmbedWhenInputTextContainsCRLF()
    {
        // Arrange
        char firstChar = (char)(MinChar + 12);
        EmbeddingService embeddingService = GetEmbeddingServiceForEmbed(firstChar);
        string indicatorString = GetIndicatorString(firstChar);
        string inputText = "ABCD\r\nFGHIJKLMNOP";
        string seedValue = "abcdefg";
        string expected = indicatorString + "Aa0Bb1Cc2Dd3\r\ne" + DelimiterChar + "FfGgH" + DelimiterChar + "IJKLMNOP";

        // Act
        string actual = embeddingService.Embed(inputText, GetEnigmaConfiguration(3, seedValue));

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }

    [Fact]
    public void Embed_ShouldCorrectlyEmbedWhenInputTextIsLongestComponent()
    {
        // Arrange
        char firstChar = (char)(MinChar + 10);
        EmbeddingService embeddingService = GetEmbeddingServiceForEmbed(firstChar);
        string indicatorString = GetIndicatorString(firstChar);
        string inputText = "ABCDEFGHIJKLMNOP";
        string seedValue = "abcdefg";
        string expected = indicatorString + "Aa0Bb1Cc2Dd3Ee" + DelimiterChar + "FfGgH" + DelimiterChar + "IJKLMNOP";

        // Act
        string actual = embeddingService.Embed(inputText, GetEnigmaConfiguration(3, seedValue));

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }

    [Fact]
    public void Embed_ShouldCorrectlyEmbedWhenSeedValueContainsCRLF()
    {
        // Arrange
        char firstChar = (char)(MinChar + 7);
        EmbeddingService embeddingService = GetEmbeddingServiceForEmbed(firstChar);
        string indicatorString = GetIndicatorString(firstChar);
        string inputText = "ABCDEFGHIJKLM";
        string seedValue = "abcd\r\nfg";
        string expected = indicatorString + "Aa0Bb1Cc2Dd3E\r\n4Ff" + DelimiterChar + "GgH" + DelimiterChar + "IJKLM";

        // Act
        string actual = embeddingService.Embed(inputText, GetEnigmaConfiguration(4, seedValue));

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }

    [Fact]
    public void Embed_ShouldCorrectlyEmbedWhenSeedValueIsLongestComponent()
    {
        // Arrange
        char firstChar = (char)(MinChar + 5);
        EmbeddingService embeddingService = GetEmbeddingServiceForEmbed(firstChar);
        string indicatorString = GetIndicatorString(firstChar);
        string inputText = "ABCDEFGH";
        string seedValue = "abcdefghijkl";
        string expected = indicatorString + "Aa0Bb1Cc2Dd3Ee4Ff" + DelimiterChar + "GgHh" + DelimiterChar + "ijkl";

        // Act
        string actual = embeddingService.Embed(inputText, GetEnigmaConfiguration(4, seedValue));

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Embed_ShouldReturnEmptyStringIfInputTextIsNullOrEmpty(string? inputText)
    {
        // Arrange
        EmbeddingService embeddingService = GetEmbeddingServiceForEmbed();

        // Act
        string actual = embeddingService.Embed(inputText!, GetEnigmaConfiguration(5, "Seed Value"));

        // Assert
        actual
            .Should()
            .BeEmpty();
        Mock.VerifyAll();
    }

    [Fact]
    public void Embed_ShouldReturnInputTextUnchangedIfItStartsWithAnEmbeddingIndicatorString()
    {
        // Arrange
        EmbeddingService embeddingService = GetEmbeddingServiceForEmbed();
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
        Mock.VerifyAll();
    }

    [Fact]
    public void Extract_ShouldExtractConfigurationWhenIndexValuesIsLongestComponent()
    {
        // Arrange
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract();
        string indicatorString = GetIndicatorString(MinChar);
        string embeddedTextString = indicatorString + "Aa0Bb1Cc2Dd3Ee4Ff5G" + DelimiterChar + "6H7" + DelimiterChar + "8";
        string expectedInputText = "ABCDEFGH";
        string expectedSeedValue = "abcdef";
        int expectedNumberOfRotors = 8;

        // Act
        string actualInputText = embeddingService.Extract(embeddedTextString, out EnigmaConfiguration? configuration);

        // Assert
        actualInputText
            .Should()
            .Be(expectedInputText);
        AssertEnigmaConfiguration(configuration, expectedSeedValue, expectedNumberOfRotors);
        Mock.VerifyAll();
    }

    [Fact]
    public void Extract_ShouldExtractConfigurationWhenInputTextContainsCRLF()
    {
        // Arrange
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract();
        string indicatorString = GetIndicatorString((char)(MinChar + 12));
        string embeddedTextString = indicatorString + "Aa0Bb1Cc2Dd3\r\ne" + DelimiterChar + "FfGgH" + DelimiterChar + "IJKLMNOP";
        string expectedInputText = "ABCD\r\nFGHIJKLMNOP";
        string expectedSeedValue = "abcdefg";
        int expectedNumberOfRotors = 3;

        // Act
        string actualInputText = embeddingService.Extract(embeddedTextString, out EnigmaConfiguration? configuration);

        // Assert
        actualInputText
            .Should()
            .Be(expectedInputText);
        AssertEnigmaConfiguration(configuration, expectedSeedValue, expectedNumberOfRotors);
        Mock.VerifyAll();
    }

    [Fact]
    public void Extract_ShouldExtractConfigurationWhenInputTextIsLongestComponent()
    {
        // Arrange
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract();
        string indicatorString = GetIndicatorString((char)(MinChar + 10));
        string embeddedTextString = indicatorString + "Aa0Bb1Cc2Dd3Ee" + DelimiterChar + "FfGgH" + DelimiterChar + "IJKLMNOP";
        string expectedInputText = "ABCDEFGHIJKLMNOP";
        string expectedSeedValue = "abcdefg";
        int expectedNumberOfRotors = 3;

        // Act
        string actualInputText = embeddingService.Extract(embeddedTextString, out EnigmaConfiguration? configuration);

        // Assert
        actualInputText
            .Should()
            .Be(expectedInputText);
        AssertEnigmaConfiguration(configuration, expectedSeedValue, expectedNumberOfRotors);
        Mock.VerifyAll();
    }

    [Fact]
    public void Extract_ShouldExtractConfigurationWhenSeedValueContainsCRLF()
    {
        // Arrange
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract();
        string indicatorString = GetIndicatorString((char)(MinChar + 7));
        string embeddedTextString = indicatorString + "Aa0Bb1Cc2Dd3E\r\n4Ff" + DelimiterChar + "GgH" + DelimiterChar + "IJKLM";
        string expectedInputText = "ABCDEFGHIJKLM";
        string expectedSeedValue = "abcd\r\nfg";
        int expectedNumberOfRotors = 4;

        // Act
        string actualInputText = embeddingService.Extract(embeddedTextString, out EnigmaConfiguration? configuration);

        // Assert
        actualInputText
            .Should()
            .Be(expectedInputText);
        AssertEnigmaConfiguration(configuration, expectedSeedValue, expectedNumberOfRotors);
        Mock.VerifyAll();
    }

    [Fact]
    public void Extract_ShouldExtractConfigurationWhenSeedValueIsLongestComponent()
    {
        // Arrange
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract();
        string indicatorString = GetIndicatorString((char)(MinChar + 5));
        string embeddedTextString = indicatorString + "Aa0Bb1Cc2Dd3Ee4Ff" + DelimiterChar + "GgHh" + DelimiterChar + "ijkl";
        string expectedInputText = "ABCDEFGH";
        string expectedSeedValue = "abcdefghijkl";
        int expectedNumberOfRotors = 4;

        // Act
        string actualInputText = embeddingService.Extract(embeddedTextString, out EnigmaConfiguration? configuration);

        // Assert
        actualInputText
            .Should()
            .Be(expectedInputText);
        AssertEnigmaConfiguration(configuration, expectedSeedValue, expectedNumberOfRotors);
        Mock.VerifyAll();
    }

    [Fact]
    public void Extract_ShouldReturnEmbeddedTextUnchangedIfLengthIsTooSmall()
    {
        // Arrange
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract();
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
        Mock.VerifyAll();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Extract_ShouldReturnEmptyStringIfEmbeddedTextIsNullOrEmpty(string? inputText)
    {
        // Arrange
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract();

        // Act
        string actual = embeddingService.Extract(inputText!, out EnigmaConfiguration? configuration);

        // Assert
        actual
            .Should()
            .BeEmpty();
        configuration
            .Should()
            .BeNull();
        Mock.VerifyAll();
    }

    [Fact]
    public void Extract_ShouldReturnInputTextUnchangedWhenNoIndicatorStringIsFound()
    {
        // Arrange
        EmbeddingService embeddingService = GetEmbeddingServiceForExtract();
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
        Mock.VerifyAll();
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

    private static EmbeddingService GetEmbeddingServiceForEmbed(params char[] firstChar)
    {
        Mock<IIndicatorStringGenerator> mockGenerator = new(MockBehavior.Strict);
        ISetupSequentialResult<string> sequence = mockGenerator.SetupSequence(static m => m.GetIndicatorString(EmbeddingIndicatorChar));

        if (firstChar.Length is 0)
        {
            sequence = sequence.Returns(GetIndicatorString(MinChar));
        }
        else
        {
            for (int j = 0; j < firstChar.Length; j++)
            {
                sequence = sequence.Returns(GetIndicatorString(firstChar[j]));
            }
        }

        sequence = sequence.Throws(new InvalidOperationException("No more numbers should be generated."));
        return new(mockGenerator.Object);
    }

    private static EmbeddingService GetEmbeddingServiceForExtract()
    {
        Mock<IIndicatorStringGenerator> mockGenerator = new(MockBehavior.Strict);
        mockGenerator.Setup(static m => m.GetIndicatorString(It.IsAny<char>()))
            .Throws(new InvalidOperationException("No numbers should be generated."));
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