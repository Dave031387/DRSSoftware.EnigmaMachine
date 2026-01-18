namespace DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// Defines the methods needed for embedding/extracting the Enigma configuration into/out of a text
/// string.
/// </summary>
public interface IEmbeddingService
{
    /// <summary>
    /// Embed the Enigma machine configuration details into the given text string.
    /// </summary>
    /// <remarks>
    /// The input text will be returned unchanged if it is determined that it already contains
    /// embedded configuration values.
    /// </remarks>
    /// <param name="inputText">
    /// An encrypted text string into which the configuration will be embedded.
    /// </param>
    /// <param name="configuration">
    /// The configuration details that are to be embedded into the text string.
    /// </param>
    /// <returns>
    /// The input text string with the Enigma machine configuration details embedded into it.
    /// </returns>
    public string Embed(string inputText, EnigmaConfiguration configuration);

    /// <summary>
    /// Extract the Enigma machine configuration from the given input text.
    /// </summary>
    /// <remarks>
    /// The original input text will be returned along with the default Enigma machine configuration
    /// if it is found that the input text doesn't begin with the required embedding indicator
    /// characters.
    /// </remarks>
    /// <param name="inputText">
    /// The input text that contains the embedded configuration.
    /// </param>
    /// <param name="configuration">
    /// The Enigma configuration details that will be returned to the caller.
    /// </param>
    /// <returns>
    /// The remaining input text after the configuration has been extracted. The configuration
    /// itself is returned in the <paramref name="configuration" /> out parameter.
    /// </returns>
    string Extract(string inputText, out EnigmaConfiguration configuration);

    /// <summary>
    /// Determine whether or not the supplied <paramref name="inputText" /> starts with a Embedding
    /// indicator string.
    /// </summary>
    /// <param name="inputText">
    /// The input text to be checked for the indicator string.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the <paramref name="inputText" /> starts with an indicator
    /// string.
    /// </returns>
    public bool HasIndicatorString(string inputText);
}