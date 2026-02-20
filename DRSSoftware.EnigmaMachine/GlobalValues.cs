using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DRSSoftware.EnigmaMachine.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace DRSSoftware.EnigmaMachine;

/// <summary>
/// This static class defines all of the common constant values used in the Enigma machine.
/// </summary>
public static class GlobalValues
{
    /// <summary>
    /// The carriage return character ('\r', or U+000D).
    /// </summary>
    public const char CarriageReturn = '\r';

    /// <summary>
    /// The value obtained when a pair of corresponding cloaking indicator characters are added
    /// together.
    /// </summary>
    public const int CloakedIndicatorValue = CloakingIndicatorChar + MinChar;

    /// <summary>
    /// A character value that is one higher than the maximum cloaking indicator value.
    /// </summary>
    public const char CloakingIndicatorChar = (char)(EmbeddingIndicatorChar - 1);

    /// <summary>
    /// Text displayed when the Enigma machine is properly configured.
    /// </summary>
    public const string ConfiguredStatusText = "Configured";

    /// <summary>
    /// A string containing a Carriage Return/Line Feed pair of characters.
    /// </summary>
    public const string CRLF = "\r\n";

    /// <summary>
    /// A character value used by the embedding algorithm to mark the end of one of the sequences
    /// (input text, seed value text, or reflector and rotor index values).
    /// </summary>
    public const char DelimiterChar = '\u0096';

    /// <summary>
    /// The value obtained when a pair of corresponding embedded indicator characters are added
    /// together.
    /// </summary>
    public const int EmbeddedIndicatorValue = EmbeddingIndicatorChar + MinChar;

    /// <summary>
    /// A character value that is one higher than the maximum embedding indicator value.
    /// </summary>
    public const char EmbeddingIndicatorChar = (char)(MaxChar - 1);

    /// <summary>
    /// The number of character pairs that make up the cloaking indicator string or the embedded
    /// configuration indicator string.
    /// </summary>
    public const int IndicatorPairs = 3;

    /// <summary>
    /// The total size of the cloaking indicator string or the embedded configuration indicator
    /// string.
    /// </summary>
    public const int IndicatorSize = IndicatorPairs * 2;

    /// <summary>
    /// The line feed character ('\n', or U+000A).
    /// </summary>
    public const char LineFeed = '\n';

    /// <summary>
    /// The maximum valid character value (U+007F) supported by the Enigma machine.
    /// </summary>
    public const char MaxChar = '\u007f';

    /// <summary>
    /// The maximum number of rotors supported by the Enigma machine.
    /// </summary>
    public const int MaxRotorCount = 8;

    /// <summary>
    /// The maximum seed value length supported by the Enigma machine.
    /// </summary>
    public const int MaxSeedLength = MaxValue - 1;

    /// <summary>
    /// The maximum integer value corresponding to the maximum character value supported by the
    /// Enigma machine.
    /// </summary>
    public const int MaxValue = MaxChar - MinChar;

    /// <summary>
    /// The minimum valid character value (the space character, or U+0020) supported by the Enigma
    /// machine.
    /// </summary>
    public const char MinChar = '\u0020';

    /// <summary>
    /// The minimum valid size of an embedded text string.
    /// </summary>
    /// <remarks>
    /// The size is based on the size of the indicator string, the minimum number of rotors, and the
    /// minimum length of the seed value string plus 4 (one for the reflector index value, one for
    /// the encrypted text string, and two for the two delimiter characters).
    /// </remarks>
    public const int MinEmbeddedStringSize = IndicatorSize + MinRotorCount + MinStringLength + 4;

    /// <summary>
    /// The minimum number of rotors supported by the Enigma machine.
    /// </summary>
    public const int MinRotorCount = 3;

    /// <summary>
    /// The minimum valid length for seed strings and cloaking strings.
    /// </summary>
    public const int MinStringLength = 10;

    /// <summary>
    /// The Minimum integer value corresponding to the minimum character value supported by the
    /// Enigma machine.
    /// </summary>
    public const int MinValue = 0;

    /// <summary>
    /// Text displayed when the Enigma machine is not fully configured.
    /// </summary>
    public const string NotConfiguredStatusText = "Not Configured";
}