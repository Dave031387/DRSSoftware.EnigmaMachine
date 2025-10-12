using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DRSSoftware.EnigmaV2.Tests")]

namespace DRSSoftware.EnigmaV2;

/// <summary>
/// The <see cref="EnigmaMachine" /> class is an electronic implementation of the German Enigma
/// machine which was used for encrypting messages during World War 2.
/// </summary>
/// <remarks>
/// This implementation is a variation on the original Enigma machine. The plugboard feature isn't
/// implemented, and there are four cipher wheels (rotors) instead of the usual three. <br /> Also,
/// upper and lowercase letters as well as numbers, special characters and CRLF are allowed, whereas
/// the original Enigma machine only supported uppercase characters.
/// </remarks>
public class EnigmaMachine
{
    /// <summary>
    /// Represents an instance of the <see cref="Reflector" /> class. Each
    /// <see cref="EnigmaMachine" /> has exactly one <see cref="Reflector" />.
    /// </summary>
    internal readonly Reflector _reflector = new();

    /// <summary>
    /// Represents the collection of <see cref="Rotor" /> objects. Each <see cref="EnigmaMachine" />
    /// contains one or more of these objects.
    /// </summary>
    /// <remarks>
    /// This array contains a fixed number of rotors, defined by <see cref="NumberOfRotors" />.
    /// </remarks>
    internal readonly Rotor[] _rotors = new Rotor[NumberOfRotors];

    /// <summary>
    /// An array which is used to store the initial index settings of the <see cref="Reflector" />
    /// and <see cref="Rotor" /> objects.
    /// </summary>
    /// <remarks>
    /// This array is filled by the <see cref="SetIndexes(int[])" /> method and used by the
    /// <see cref="ResetIndexes" /> method. <br /> The last entry in this array is the
    /// <see cref="Reflector" /> index. The rest are the <see cref="Rotor" /> indexes.
    /// </remarks>
    internal readonly int[] _transformerIndexes = new int[NumberOfTransformers];

    /// <summary>
    /// A boolean flag that is set to <see langword="true" /> once the <see cref="Reflector" />
    /// object and <see cref="Rotor" /> objects are all initialized properly.
    /// </summary>
    internal bool _isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnigmaMachine" /> class.
    /// </summary>
    /// <remarks>
    /// The constructor automatically configures the Enigma machine by invoking the necessary setup
    /// logic.
    /// </remarks>
    public EnigmaMachine() => BuildEnigmaMachine();

    /// <summary>
    /// Initializes the internal components of the <see cref="EnigmaMachine" /> using the specified
    /// <paramref name="seed" /> value.
    /// </summary>
    /// <remarks>
    /// This method configures the <see cref="Reflector" /> and each <see cref="Rotor" /> based on
    /// the provided seed. The seed is used to initialize the <see cref="Reflector" />, and modified
    /// versions of the seed are used to initialize each <see cref="Rotor" />. <br /> After
    /// initialization, the system is marked as ready for use.
    /// </remarks>
    /// <param name="seed">
    /// A non-null string used to initialize the reflector and rotors. The seed determines the
    /// initial state of the components.
    /// </param>
    public void Initialize(string seed)
    {
        ArgumentNullException.ThrowIfNull(seed, nameof(seed));

        char[] chars = seed.ToCharArray();
        _reflector.Initialize(seed);

        for (int i = 0; i < NumberOfRotors; i++)
        {
            string alteredSeed = GenerateNewSeed(chars, i + 2);
            _rotors[i].Initialize(alteredSeed);
        }

        _isInitialized = true;
    }

    /// <summary>
    /// Resets the index of the <see cref="Reflector" /> and each <see cref="Rotor" /> to their
    /// initial values (or 0, if <see cref="SetIndexes(int[])" /> was never called).
    /// </summary>
    /// <remarks>
    /// This method has no effect if the system was never initialized.
    /// </remarks>
    public void ResetIndexes()
    {
        if (_isInitialized)
        {
            for (int i = 0; i < NumberOfRotors; i++)
            {
                _rotors[i].SetIndex(_transformerIndexes[i]);
            }

            _reflector.SetIndex(_transformerIndexes[NumberOfRotors]);
        }
    }

    /// <summary>
    /// Sets the index of the <see cref="Reflector" /> and each <see cref="Rotor" /> of the
    /// <see cref="EnigmaMachine" /> to the specified values.
    /// </summary>
    /// <remarks>
    /// The <see cref="EnigmaMachine" /> must be initialized before calling this method. The last
    /// value in <paramref name="indexes" /> is the index for the <see cref="Reflector" /> and the
    /// rest are indexes for each <see cref="Rotor" />.
    /// </remarks>
    /// <param name="indexes">
    /// An array containing the desired index values. The array must contain exactly
    /// <see cref="NumberOfTransformers" /> elements.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if the length of <paramref name="indexes" /> is not equal to
    /// <see cref="NumberOfTransformers" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the <see cref="EnigmaMachine" /> has not been initialized before calling this
    /// method.
    /// </exception>
    public void SetIndexes(params int[] indexes)
    {
        ArgumentNullException.ThrowIfNull(indexes, nameof(indexes));

        if (indexes.Length is not NumberOfTransformers)
        {
            string word = indexes.Length is 1 ? "was" : "were";
            throw new ArgumentException($"Exactly {NumberOfTransformers} index values must be passed into the SetIndexes method, but there {word} {indexes.Length}.", nameof(indexes));
        }

        if (_isInitialized)
        {
            for (int i = 0; i < NumberOfTransformers; i++)
            {
                _transformerIndexes[i] = indexes[i];
            }

            ResetIndexes();
        }
        else
        {
            throw new InvalidOperationException("The Enigma machine must be initialized before setting the indexes.");
        }
    }

    /// <summary>
    /// Transforms the input <paramref name="text" /> using the configured
    /// <see cref="EnigmaMachine" /> settings.
    /// </summary>
    /// <remarks>
    /// This method processes the input text character by character, applying the configured
    /// <see cref="Rotor" /> and <see cref="Reflector" /> transformations. <br /> Characters outside
    /// the valid range are replaced by the space character before being transformed. Carriage
    /// return characters are ignored. <br /> New line characters coming in are converted to DEL
    /// characters (U+007F), and <see cref="MaxChar" /> values coming out are converted back to new
    /// line characters.
    /// </remarks>
    /// <param name="text">
    /// The input text to be transformed. Cannot be <see langword="null" />.
    /// </param>
    /// <returns>
    /// The transformed text as a string. Line breaks in the transformed text are replaced with
    /// platform-specific newlines (CRLF for Windows based systems).
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the Enigma machine is not initialized before calling this method.
    /// </exception>
    public string Transform(string text)
    {
        ArgumentNullException.ThrowIfNull(text, nameof(text));

        if (_isInitialized)
        {
            char[] inChars = text.ToCharArray();
            List<char> outChars = [];

            for (int i = 0; i < inChars.Length; i++)
            {
                char nextChar = inChars[i];

                if (nextChar is CarriageReturn)
                {
                    continue;
                }

                char c = nextChar is LineFeed ? MaxChar : nextChar is < MinChar or >= MaxChar ? MinChar : nextChar;
                int original = CharToInt(c);
                int transformed = _rotors[0].TransformIn(original, true);

                if (transformed is MaxIndex)
                {
                    outChars.AddRange(Environment.NewLine);
                }
                else
                {
                    outChars.Add(IntToChar(transformed));
                }
            }

            return new([.. outChars]);
        }

        throw new InvalidOperationException("The Enigma machine must be initialized before calling the Transform method.");
    }

    /// <summary>
    /// Generates a new seed string by reordering the characters of the <paramref name="chars" />
    /// array based on the specified <paramref name="offset" />.
    /// </summary>
    /// <param name="chars">
    /// An array of characters to be reordered. Cannot be null.
    /// </param>
    /// <param name="offset">
    /// The step size used to determine the order of characters in the resulting string. <br /> Must
    /// be greater than 1 and less than or equal to <see cref="NumberOfTransformers" />.
    /// </param>
    /// <returns>
    /// A new seed string containing the reordered characters from the input array.
    /// </returns>
    private static string GenerateNewSeed(char[] chars, int offset)
    {
        char[] seedChars = new char[chars.Length];
        int start = 0;
        int index = 0;

        while (index < chars.Length)
        {
            for (int i = start; i < chars.Length; i += offset)
            {
                seedChars[index] = chars[i];

                if (++index == chars.Length)
                {
                    break;
                }
            }

            start++;
        }

        return new(seedChars);
    }

    /// <summary>
    /// Builds the <see cref="EnigmaMachine" /> by adding the <see cref="Reflector" /> object and
    /// the appropriate number of <see cref="Rotor" /> objects. Each object is then connected
    /// </summary>
    private void BuildEnigmaMachine()
    {
        for (int i = 0; i < NumberOfRotors; i++)
        {
            _rotors[i] = new Rotor();
        }

        for (int i = 0; i < NumberOfRotors; i++)
        {
            if (i is not 0)
            {
                _rotors[i].ConnectIncoming(_rotors[i - 1]);
            }

            if (i < NumberOfRotors - 1)
            {
                _rotors[i].ConnectOutgoing(_rotors[i + 1]);
            }
            else
            {
                _rotors[i].ConnectOutgoing(_reflector);
            }
        }

        _reflector.ConnectOutgoing(_rotors[NumberOfRotors - 1]);
        _isInitialized = false;
    }
}