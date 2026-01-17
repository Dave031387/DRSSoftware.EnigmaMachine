namespace DRSSoftware.EnigmaMachine.Utility;

/// <summary>
/// Defines the methods used for applying and removing cloaking transformations on text.
/// </summary>
public interface ICloakingService
{
    /// <summary>
    /// Cloak the given <paramref name="inputText" /> using the supplied
    /// <paramref name="cloakText" />.
    /// </summary>
    /// <remarks>
    /// A special cloaking indicator string will be prepended to the cloaked text to indicate that
    /// the text has been cloaked.
    /// </remarks>
    /// <param name="inputText">
    /// The text to be transformed by applying the cloak.
    /// </param>
    /// <param name="cloakText">
    /// The string used for performing the cloaking transformation.
    /// </param>
    /// <returns>
    /// The string obtained by applying the cloaking transformation to the input string.
    /// </returns>
    string ApplyCloak(string inputText, string cloakText);

    /// <summary>
    /// Determine whether or not the supplied <paramref name="inputText" /> starts with a cloaking
    /// indicator string.
    /// </summary>
    /// <param name="inputText">
    /// The input text to be checked for the indicator string.
    /// </param>
    /// <returns>
    /// <see langword="true" /> if the <paramref name="inputText" /> starts with and indicator
    /// string.
    /// </returns>
    bool HasIndicatorString(string inputText);

    /// <summary>
    /// Remove the cloak from the given <paramref name="inputText" /> using the supplied
    /// <paramref name="cloakText" />.
    /// </summary>
    /// <remarks>
    /// The method assumes that the input text has been previously cloaked using the same
    /// <paramref name="cloakText" />. The cloaking indicator string is removed from the
    /// <paramref name="inputText" /> before removing the cloak.
    /// </remarks>
    /// <param name="inputText">
    /// The text to be transformed by removing the cloak.
    /// </param>
    /// <param name="cloakText">
    /// The string used for undoing the cloaking transformation.
    /// </param>
    /// <returns>
    /// The string obtained by removing the cloaking transformation from the input string.
    /// </returns>
    string RemoveCloak(string inputText, string cloakText);
}