namespace DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// Defines the method used for generating cloaking and embedding indicator strings.
/// </summary>
public interface IIndicatorStringGenerator
{
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
    public string GetIndicatorString(char indicatorChar);
}