namespace DRSSoftware.EnigmaV2;

/// <summary>
/// Defines the contract for an Enigma machine capable of initializing, configuring, and
/// transforming text using a configurable cipher mechanism.
/// </summary>
/// <remarks>
/// Implementations of this interface simulate the behavior of an Enigma machine, allowing for
/// initialization with a specific seed, configuration of internal indexes, and transformation of
/// input text. The interface does not specify thread safety. There should only ever be one user of
/// any given instance of the <see cref="EnigmaMachine" /> at any time.
/// </remarks>
public interface IEnigmaMachine
{
    /// <summary>
    /// Gets a value indicating whether the <see cref="EnigmaMachine" /> has been initialized and is
    /// ready for use.
    /// </summary>
    /// <remarks>
    /// This property is set to <see langword="true" /> once the <c> "Initialize(string)" </c>
    /// method has successfully completed.
    /// </remarks>
    public bool IsInitialized
    {
        get;
    }

    /// <summary>
    /// Gets a reference to the <see cref="Reflector" /> object associated with this
    /// <see cref="EnigmaMachine" />.
    /// </summary>
    public IReflector MyReflector
    {
        get;
    }

    /// <summary>
    /// Gets a reference to the first <see cref="Rotor" /> for this <see cref="EnigmaMachine" />, or
    /// <see langword="null" /> if the first <see cref="Rotor" /> hasn't been defined.
    /// </summary>
    public IRotor? Rotor1
    {
        get;
    }

    /// <summary>
    /// Gets a reference to the second <see cref="Rotor" /> for this <see cref="EnigmaMachine" />,
    /// or <see langword="null" /> if the second <see cref="Rotor" /> hasn't been defined.
    /// </summary>
    public IRotor? Rotor2
    {
        get;
    }

    /// <summary>
    /// Gets a reference to the third <see cref="Rotor" /> for this <see cref="EnigmaMachine" />, or
    /// <see langword="null" /> if the third <see cref="Rotor" /> hasn't been defined.
    /// </summary>
    public IRotor? Rotor3
    {
        get;
    }

    /// <summary>
    /// Gets a reference to the fourth <see cref="Rotor" /> for this <see cref="EnigmaMachine" />,
    /// or <see langword="null" /> if the fourth <see cref="Rotor" /> hasn't been defined.
    /// </summary>
    public IRotor? Rotor4
    {
        get;
    }

    /// <summary>
    /// Gets a reference to the fifth <see cref="Rotor" /> for this <see cref="EnigmaMachine" />, or
    /// <see langword="null" /> if the fifth <see cref="Rotor" /> hasn't been defined.
    /// </summary>
    public IRotor? Rotor5
    {
        get;
    }

    /// <summary>
    /// Gets a reference to the sixth <see cref="Rotor" /> for this <see cref="EnigmaMachine" />, or
    /// <see langword="null" /> if the sixth <see cref="Rotor" /> hasn't been defined.
    /// </summary>
    public IRotor? Rotor6
    {
        get;
    }

    /// <summary>
    /// Gets a reference to the seventh <see cref="Rotor" /> for this <see cref="EnigmaMachine" />,
    /// or <see langword="null" /> if the seventh <see cref="Rotor" /> hasn't been defined.
    /// </summary>
    public IRotor? Rotor7
    {
        get;
    }

    /// <summary>
    /// Gets a reference to the eighth <see cref="Rotor" /> for this <see cref="EnigmaMachine" />,
    /// or <see langword="null" /> if the eighth <see cref="Rotor" /> hasn't been defined.
    /// </summary>
    public IRotor? Rotor8
    {
        get;
    }

    /// <summary>
    /// Initializes the internal components of the <see cref="EnigmaMachine" /> using the specified
    /// <paramref name="seed" /> value.
    /// </summary>
    /// <remarks>
    /// This method configures the <see cref="Reflector" /> and each <see cref="Rotor" /> of the
    /// <see cref="EnigmaMachine" /> based on the provided seed. <br /> The <paramref name="seed" />
    /// is used to initialize the <see cref="Reflector" />, and modified versions of the
    /// <paramref name="seed" /> are used to initialize each <see cref="Rotor" />. <br /> After
    /// initialization, the system is marked as ready for use.
    /// </remarks>
    /// <param name="seed">
    /// A non-null string used to initialize the <see cref="Reflector" /> and each
    /// <see cref="Rotor" />. The seed determines the initial state of the components.
    /// </param>
    public void Initialize(string seed);

    /// <summary>
    /// Resets the index of the <see cref="Reflector" /> and each <see cref="Rotor" /> to their
    /// initial values (or 0, if the <c> "SetIndexes(int[])" </c> method was never called).
    /// </summary>
    /// <remarks>
    /// This method has no effect if the system was never initialized.
    /// </remarks>
    public void ResetCipherIndexes();

    /// <summary>
    /// Sets the cipher index of the <see cref="Reflector" /> and each <see cref="Rotor" /> of the
    /// <see cref="EnigmaMachine" /> to the specified values.
    /// </summary>
    /// <remarks>
    /// The <see cref="EnigmaMachine" /> must be initialized before calling this method.
    /// </remarks>
    /// <param name="indexes">
    /// An array containing the desired index values. The array must contain one index for each
    /// <see cref="Rotor" /> and an index for the <see cref="Reflector" />. <br /> The last element
    /// in the array is used for the <see cref="Reflector" /> and the rest are used for each
    /// <see cref="Rotor" />.
    /// </param>
    public void SetCipherIndexes(params int[] indexes);

    /// <summary>
    /// Transforms the input <paramref name="text" /> using the configured
    /// <see cref="EnigmaMachine" /> settings.
    /// </summary>
    /// <remarks>
    /// This method processes the input text character by character, applying the configured
    /// <see cref="Rotor" /> and <see cref="Reflector" /> transformations. <br /> Characters outside
    /// the valid range are replaced by the space character before being transformed. Carriage
    /// return characters are ignored. <br /> New line characters coming in are converted to
    /// <see langword="DEL" /> characters (U+007F), and <see langword="DEL" /> characters going out
    /// are converted back to new line characters.
    /// </remarks>
    /// <param name="text">
    /// The input text to be transformed. Must not be <see langword="null" />.
    /// </param>
    /// <returns>
    /// The transformed text as a string. Line breaks in the transformed text are replaced with
    /// platform-specific newlines ( <see langword="CRLF" />, for Windows based systems).
    /// </returns>
    public string Transform(string text);
}