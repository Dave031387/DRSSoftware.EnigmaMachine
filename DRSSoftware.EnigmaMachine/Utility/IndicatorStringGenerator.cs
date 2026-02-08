namespace DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// The sole purpose of this class is to generate random cloaking and embedding indicator strings.
/// </summary>
/// <param name="secureNumberGenerator">
/// A reference to an object used for generating cryptographically secure random integer values.
/// </param>
internal class IndicatorStringGenerator(ISecureNumberGenerator secureNumberGenerator) : IIndicatorStringGenerator
{
    /// <summary>
    /// Holds a reference to an object used for generating cryptographically secure random integer
    /// values.
    /// </summary>
    private readonly ISecureNumberGenerator _numberGenerator = secureNumberGenerator;

    /// <summary>
    /// Generate a random indicator string based on the <paramref name="indicatorChar" /> passed
    /// into this method.
    /// </summary>
    /// <remarks>
    /// Each character in the first half of the indicator string is randomly generated between
    /// MinChar (inclusive) and <paramref name="indicatorChar" /> (exclusive). Each character in the
    /// second half is obtained by subtracting the corresponding character in the first half from
    /// <paramref name="indicatorChar" /> and then adding MinChar.
    /// </remarks>
    /// <param name="indicatorChar">
    /// A character value representing the maximum indicator value.
    /// </param>
    /// <returns>
    /// A randomly-generated indicator string.
    /// </returns>
    public string GetIndicatorString(char indicatorChar)
    {
        char[] indicatorChars = new char[IndicatorSize];

        for (int i = 0; i < IndicatorPairs; i++)
        {
            int indicatorValue = _numberGenerator.GetNext(MinChar, indicatorChar);
            indicatorChars[i] = (char)indicatorValue;
            indicatorChars[i + IndicatorPairs] = (char)(indicatorChar + MinChar - indicatorValue);
        }

        return new string(indicatorChars);
    }
}