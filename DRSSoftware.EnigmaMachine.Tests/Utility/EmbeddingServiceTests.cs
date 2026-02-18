namespace DRSSoftware.EnigmaMachine.Utility;

public class EmbeddingServiceTests
{
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

        string expected = new string(indicatorChars) + "Some sample text.";

        // Act
        string actual = embeddingService.Embed(expected, new EnigmaConfiguration());

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }

    private static EmbeddingService GetEmbeddingServiceForEmbed(params char[] firstChar)
    {
        char[] indicatorChars = new char[IndicatorSize];
        Mock<IIndicatorStringGenerator> mockGenerator = new(MockBehavior.Strict);
        ISetupSequentialResult<string> sequence = mockGenerator.SetupSequence(static m => m.GetIndicatorString(EmbeddingIndicatorChar));

        if (firstChar.Length is 0)
        {
            for (int i = 0; i < IndicatorPairs; i++)
            {
                indicatorChars[i] = (char)(MinChar + i);
                indicatorChars[i + IndicatorPairs] = (char)(EmbeddedIndicatorValue - indicatorChars[i]);
            }

            sequence = sequence.Returns(new string(indicatorChars));
        }
        else
        {
            for (int j = 0; j < firstChar.Length; j++)
            {
                for (int i = 0; i < IndicatorPairs; i++)
                {
                    indicatorChars[i] = (char)(firstChar[j] + i);
                    indicatorChars[i + IndicatorPairs] = (char)(EmbeddedIndicatorValue - indicatorChars[i]);
                }

                sequence = sequence.Returns(new string(indicatorChars));
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
}