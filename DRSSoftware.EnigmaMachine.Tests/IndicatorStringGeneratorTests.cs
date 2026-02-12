namespace DRSSoftware.EnigmaMachine;

using DRSSoftware.EnigmaMachine.Utility;
using Moq.Language;

public class IndicatorStringGeneratorTests
{
    [Theory]
    [InlineData(MinChar, MaxChar - 1)]
    [InlineData(MinChar, MaxChar - 2)]
    [InlineData(MaxChar - 1 - IndicatorPairs, MaxChar - 1)]
    [InlineData(MaxChar - 1 - IndicatorPairs, MaxChar - 2)]
    public void GetIndicatorString_ShouldReturnValidIndicatorString(char firstChar, char indicatorChar)
    {
        // Arrange
        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);
        ISetupSequentialResult<int> sequence = mockNumberGenerator.SetupSequence(gen => gen.GetNext(MinChar, indicatorChar));
        char[] indicatorChars = new char[IndicatorSize];

        for (int i = 0; i < IndicatorPairs; i++)
        {
            int charValue = firstChar + i;
            sequence = sequence.Returns(charValue);
            indicatorChars[i] = (char)charValue;
            indicatorChars[i + IndicatorPairs] = (char)(indicatorChar + MinChar - charValue);
        }

        sequence = sequence.Throws(new InvalidOperationException("No more numbers should be generated."));
        string expected = new(indicatorChars);
        IndicatorStringGenerator generator = new(mockNumberGenerator.Object);

        // Act
        string actual = generator.GetIndicatorString(indicatorChar);

        // Assert
        actual
            .Should()
            .Be(expected);
        Mock.VerifyAll();
    }
}