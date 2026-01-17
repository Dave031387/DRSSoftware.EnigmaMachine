namespace DRSSoftware.EnigmaMachine.Utility;

using System.Security.Cryptography;

/// <summary>
/// Utility class used for generating cryptographically secure random integer values.
/// </summary>
internal sealed class SecureNumberGenerator : ISecureNumberGenerator
{
    /// <summary>
    /// Generates a cryptographically secure random integer between the specified minimum and
    /// maximum values.
    /// </summary>
    /// <remarks>
    /// The <paramref name="minValue" /> is inclusive, while the <paramref name="maxValue" /> is
    /// exclusive.
    /// </remarks>
    /// <param name="minValue">
    /// The minimum integer value (inclusive).
    /// </param>
    /// <param name="maxValue">
    /// The maximum integer value (exclusive).
    /// </param>
    /// <returns>
    /// A cryptographically secure random integer between <paramref name="minValue" /> and
    /// <paramref name="maxValue" />.
    /// </returns>
    public int GetNext(int minValue, int maxValue)
        => RandomNumberGenerator.GetInt32(minValue, maxValue);
}