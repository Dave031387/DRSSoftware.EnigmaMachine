namespace DRSSoftware.EnigmaMachine.Utility;

using System.Collections.Generic;

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
        List<int> sequence = [];
        char[] indicatorChars = new char[IndicatorSize];

        for (int i = 0; i < IndicatorPairs; i++)
        {
            int charValue = firstChar + i;
            sequence.Add(charValue);
            indicatorChars[i] = (char)charValue;
            indicatorChars[i + IndicatorPairs] = (char)(indicatorChar + MinChar - charValue);
        }

        List<int>.Enumerator enumerator = sequence.GetEnumerator();

        Mock<ISecureNumberGenerator> mockNumberGenerator = new(MockBehavior.Strict);
        mockNumberGenerator.Setup(m => m.GetNext(MinChar, indicatorChar))
            .Returns(() =>
            {
                enumerator.MoveNext(); 
                return enumerator.Current;
            })
            .Verifiable(Times.Exactly(sequence.Count));
        string expected = new(indicatorChars);
        IndicatorStringGenerator generator = new(mockNumberGenerator.Object);

        // Act
        string actual = generator.GetIndicatorString(indicatorChar);

        // Assert
        actual
            .Should()
            .Be(expected);
        mockNumberGenerator.VerifyAll();
    }
}