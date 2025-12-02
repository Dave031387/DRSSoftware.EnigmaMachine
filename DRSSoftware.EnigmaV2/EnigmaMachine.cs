using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DRSSoftware.EnigmaV2.Tests")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]

namespace DRSSoftware.EnigmaV2;

/// <summary>
/// The <see cref="EnigmaMachine" /> class is an electronic implementation of the German Enigma
/// machine which was used for encrypting messages during World War 2.
/// </summary>
/// <remarks>
/// This implementation is a variation on the original Enigma machine. The plugboard feature isn't
/// implemented, and there are four rotors instead of the usual three. <br /> Also, upper and
/// lowercase letters as well as numbers, special characters and CRLF are allowed, whereas the
/// original Enigma machine only supported uppercase characters.
/// </remarks>
public sealed class EnigmaMachine : IEnigmaMachine
{
    /// <summary>
    /// An array of integer values that will be used as the cycle size values that will be passed
    /// into the constructors of the <see cref="Reflector" /> and <see cref="Rotor" /> classes when
    /// the default <see cref="EnigmaMachine" /> constructor is used.
    /// </summary>
    private readonly int[] _cycleSizes = [1, 11, 7, 17, 13, 23, 29, 37, 41, 47];

    /// <summary>
    /// Represents the collection of <see cref="Rotor" /> objects. Each <see cref="EnigmaMachine" />
    /// contains one or more of these objects.
    /// </summary>
    /// <remarks>
    /// This array contains a fixed number of rotors, defined by
    /// <see cref="DefaultNumberOfRotors" /> if the default constructor is used, or defined by the
    /// number of rotors passed into the alternate constructor if it is used.
    /// </remarks>
    private readonly IRotor[] _rotors;

    /// <summary>
    /// An array which is used to store the initial index settings of the <see cref="Reflector" />
    /// and <see cref="Rotor" /> objects of this <see cref="EnigmaMachine" />.
    /// </summary>
    /// <remarks>
    /// This array is filled by the <c> "SetIndexes(int[])" </c> method and used by the <c>
    /// "ResetIndexes()" </c> method. <br /> The last entry in this array is the
    /// <see cref="Reflector" /> index. The rest are the <see cref="Rotor" /> indexes.
    /// </remarks>
    private int[] _cipherWheelIndexes = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="EnigmaMachine" /> class.
    /// </summary>
    /// <remarks>
    /// The constructor automatically configures the Enigma machine by invoking the necessary setup
    /// logic. <br /> It also instantiates the <see cref="Reflector" /> object and the required
    /// number of <see cref="Rotor" /> objects as determined by the <c> NumberOfRotors </c>
    /// constant.
    /// </remarks>
    public EnigmaMachine()
    {
        int cycleSizeIndex = 0;
        NumberOfRotors = DefaultNumberOfRotors;
        _rotors = new IRotor[NumberOfRotors];

        for (int i = 0; i < NumberOfRotors; i++)
        {
            _rotors[i] = new Rotor(_cycleSizes[cycleSizeIndex++]);

            if (cycleSizeIndex == _cycleSizes.Length)
            {
                cycleSizeIndex = 1;
            }
        }

        MyReflector = new Reflector(_cycleSizes[cycleSizeIndex]);

        BuildEnigmaMachine();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnigmaMachine" /> class.
    /// </summary>
    /// <param name="reflector">
    /// The <c> IReflector </c> object representing the <see cref="Reflector" /> component of this
    /// <see cref="EnigmaMachine" />.
    /// </param>
    /// <param name="rotors">
    /// One or more <c> IRotor </c> objects representing the <see cref="Rotor" /> components of this
    /// <see cref="EnigmaMachine" />.
    /// </param>
    public EnigmaMachine(IReflector reflector, params IRotor[] rotors)
    {
        ArgumentNullException.ThrowIfNull(reflector, nameof(reflector));
        ArgumentNullException.ThrowIfNull(rotors, nameof(rotors));

        NumberOfRotors = rotors.Length;

        if (NumberOfRotors is 0)
        {
            throw new ArgumentException($"The {nameof(Rotor)}s collection passed into the {nameof(EnigmaMachine)} constructor must contain at least one element.", nameof(rotors));
        }

        MyReflector = reflector;
        _rotors = new IRotor[NumberOfRotors];

        for (int i = 0; i < NumberOfRotors; i++)
        {
            if (rotors[i] is null)
            {
                throw new ArgumentException($"All {nameof(Rotor)}s passed into the {nameof(EnigmaMachine)} constructor must be non-null, but the {nameof(Rotor)} at index {i} is null.", nameof(rotors));
            }

            _rotors[i] = rotors[i];
        }

        BuildEnigmaMachine();
    }

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
        private set;
    }

    /// <summary>
    /// Gets a reference to the reflector object associated with this <see cref="EnigmaMachine" />.
    /// </summary>
    public IReflector MyReflector
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets the number of rotors that have been configured for this <see cref="EnigmaMachine" />.
    /// </summary>
    public int NumberOfRotors
    {
        get;
        private set;
    }

    /// <summary>
    /// Gets a reference to the first <see cref="Rotor" /> for this <see cref="EnigmaMachine" />, or
    /// <see langword="null" /> if the first rotor hasn't been defined.
    /// </summary>
    public IRotor? Rotor1 => NumberOfRotors > 0 ? _rotors[0] : null;

    /// <summary>
    /// Gets a reference to the second <see cref="Rotor" /> for this <see cref="EnigmaMachine" />,
    /// or <see langword="null" /> if the second rotor hasn't been defined.
    /// </summary>
    public IRotor? Rotor2 => NumberOfRotors > 1 ? _rotors[1] : null;

    /// <summary>
    /// Gets a reference to the third <see cref="Rotor" /> for this <see cref="EnigmaMachine" />, or
    /// <see langword="null" /> if the third rotor hasn't been defined.
    /// </summary>
    public IRotor? Rotor3 => NumberOfRotors > 2 ? _rotors[2] : null;

    /// <summary>
    /// Gets a reference to the fourth <see cref="Rotor" /> for this <see cref="EnigmaMachine" />,
    /// or <see langword="null" /> if the fourth rotor hasn't been defined.
    /// </summary>
    public IRotor? Rotor4 => NumberOfRotors > 3 ? _rotors[3] : null;

    /// <summary>
    /// Gets a reference to the fifth <see cref="Rotor" /> for this <see cref="EnigmaMachine" />, or
    /// <see langword="null" /> if the fifth rotor hasn't been defined.
    /// </summary>
    public IRotor? Rotor5 => NumberOfRotors > 4 ? _rotors[4] : null;

    /// <summary>
    /// Gets a reference to the sixth <see cref="Rotor" /> for this <see cref="EnigmaMachine" />, or
    /// <see langword="null" /> if the sixth rotor hasn't been defined.
    /// </summary>
    public IRotor? Rotor6 => NumberOfRotors > 5 ? _rotors[5] : null;

    /// <summary>
    /// Gets a reference to the seventh <see cref="Rotor" /> for this <see cref="EnigmaMachine" />,
    /// or <see langword="null" /> if the seventh rotor hasn't been defined.
    /// </summary>
    public IRotor? Rotor7 => NumberOfRotors > 6 ? _rotors[6] : null;

    /// <summary>
    /// Gets a reference to the eighth <see cref="Rotor" /> for this <see cref="EnigmaMachine" />,
    /// or <see langword="null" /> if the eighth rotor hasn't been defined.
    /// </summary>
    public IRotor? Rotor8 => NumberOfRotors > 7 ? _rotors[7] : null;

    /// <summary>
    /// Gets a copy of the <see cref="_cipherWheelIndexes" /> array containing the initial indexes
    /// of the <see cref="Reflector" /> and <see cref="Rotor" /> objects.
    /// </summary>
    /// <remarks>
    /// This property is intended for unit testing purposes only.
    /// </remarks>
    internal int[] CipherWheelIndexes
    {
        get
        {
            int[] copy = new int[_cipherWheelIndexes.Length];
            _cipherWheelIndexes.CopyTo(copy, 0);
            return copy;
        }
    }

    /// <summary>
    /// Initializes the internal components (reflector and rotors) of the
    /// <see cref="EnigmaMachine" /> using the specified <paramref name="seed" /> value.
    /// </summary>
    /// <remarks>
    /// This method configures the <see cref="Reflector" /> and each <see cref="Rotor" /> based on
    /// the provided <paramref name="seed" /> value. <br /> The seed is used to initialize the
    /// <see cref="Reflector" />, and modified versions of the seed are used to initialize each
    /// <see cref="Rotor" />. <br /> After initialization, the <see cref="EnigmaMachine" /> is
    /// marked as ready for use.
    /// </remarks>
    /// <param name="seed">
    /// A non-null string used to initialize the reflector and rotors. The seed determines the
    /// initial state of the components.
    /// </param>
    public void Initialize(string seed)
    {
        ArgumentNullException.ThrowIfNull(seed, nameof(seed));

        char[] chars = seed.ToCharArray();
        MyReflector.Initialize(seed);

        for (int i = 0; i < NumberOfRotors; i++)
        {
            string rotorSeed = GenerateNewSeed(chars, i + 2);
            _rotors[i].Initialize(rotorSeed);
            _cipherWheelIndexes[i] = 0;
        }

        _cipherWheelIndexes[^1] = 0;
        IsInitialized = true;
    }

    /// <summary>
    /// Resets the cipher index of the <see cref="Reflector" /> and each <see cref="Rotor" /> to
    /// their initial values (or 0, if <c> "SetIndexes(int[])" </c> was never called).
    /// </summary>
    /// <remarks>
    /// This method has no effect if the <see cref="EnigmaMachine" /> was never initialized.
    /// </remarks>
    public void ResetCipherIndexes()
    {
        if (IsInitialized)
        {
            for (int i = 0; i < NumberOfRotors; i++)
            {
                _rotors[i].SetCipherIndex(_cipherWheelIndexes[i]);
            }

            MyReflector.SetCipherIndex(_cipherWheelIndexes[^1]);
        }
    }

    /// <summary>
    /// Sets the cipher index of the <see cref="Reflector" /> and each <see cref="Rotor" /> of the
    /// <see cref="EnigmaMachine" /> to the specified values.
    /// </summary>
    /// <remarks>
    /// The <see cref="EnigmaMachine" /> must be initialized before calling this method. The last
    /// value in <paramref name="indexes" /> is the index for the <see cref="Reflector" /> and the
    /// rest are indexes for each <see cref="Rotor" />.
    /// </remarks>
    /// <param name="indexes">
    /// An array containing the desired index values. The array must contain exactly one value for
    /// each <see cref="Rotor" /> and one value for the <see cref="Reflector" />. <br /> The last
    /// element in the array is used for the <see cref="Reflector" /> and the rest are used for each
    /// <see cref="Rotor" />.
    /// </param>
    /// <exception cref="ArgumentException">
    /// Thrown if the length of <paramref name="indexes" /> is not equal to exactly on more than the
    /// number of <see cref="Rotor" /> objects in this <see cref="EnigmaMachine" />.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the <see cref="EnigmaMachine" /> has not been initialized before calling this
    /// method.
    /// </exception>
    public void SetCipherIndexes(params int[] indexes)
    {
        ArgumentNullException.ThrowIfNull(indexes, nameof(indexes));

        int numberOfCipherWheels = NumberOfRotors + 1;

        if (indexes.Length != numberOfCipherWheels)
        {
            string word = indexes.Length is 1 ? "was" : "were";
            throw new ArgumentException($"Exactly {numberOfCipherWheels} index values must be passed into the {nameof(SetCipherIndexes)} method of the {nameof(EnigmaMachine)} class, but there {word} {indexes.Length}.", nameof(indexes));
        }

        if (IsInitialized)
        {
            for (int i = 0; i < numberOfCipherWheels; i++)
            {
                _cipherWheelIndexes[i] = indexes[i];
            }

            ResetCipherIndexes();
        }
        else
        {
            throw new InvalidOperationException($"The {nameof(EnigmaMachine)} must be initialized before calling the {nameof(SetCipherIndexes)} method.");
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
    /// <exception cref="InvalidOperationException">
    /// Thrown if the <see cref="EnigmaMachine" /> is not initialized before calling this method.
    /// </exception>
    public string Transform(string text)
    {
        ArgumentNullException.ThrowIfNull(text, nameof(text));

        if (IsInitialized)
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

                int original = CharToInt(nextChar);
                int transformed = Rotor1!.Transform(original);

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

        throw new InvalidOperationException($"The {nameof(EnigmaMachine)} must be initialized before calling the {nameof(Transform)} method.");
    }

    /// <summary>
    /// Set the initial state of this <see cref="EnigmaMachine" /> instance.
    /// </summary>
    /// <remarks>
    /// This method is intended for unit testing purposes only.
    /// </remarks>
    /// <param name="isInitialized">
    /// A boolean value indicating whether or not the <see cref="EnigmaMachine" /> is initialized.
    /// </param>
    /// <param name="cipherWheelIndexes">
    /// An array of integer values representing the initial indexes of the cipher wheels (i.e.,
    /// <see cref="Reflector" /> and <see cref="Rotor" /> components).
    /// </param>
    internal void SetState(bool? isInitialized, int[]? cipherWheelIndexes)
    {
        if (isInitialized.HasValue)
        {
            IsInitialized = isInitialized.Value;
        }

        cipherWheelIndexes?.CopyTo(_cipherWheelIndexes, 0);
    }

    /// <summary>
    /// Generates a new seed string by reordering the characters of the <paramref name="chars" />
    /// array based on the specified <paramref name="offset" /> value.
    /// </summary>
    /// <param name="chars">
    /// An array of characters to be reordered. Cannot be <see langword="null" />.
    /// </param>
    /// <param name="offset">
    /// The step size used to determine the order of characters in the resulting string. <br /> Must
    /// be greater than 1 and less than or equal to the number of rotors plus one.
    /// </param>
    /// <returns>
    /// A new seed string containing the reordered characters from the input array.
    /// </returns>
    private static string GenerateNewSeed(char[] chars, int offset)
    {
        char[] seedChars = new char[chars.Length];
        int start = offset;
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

            start--;
        }

        return new(seedChars);
    }

    /// <summary>
    /// Builds the <see cref="EnigmaMachine" /> by establishing the connections between the
    /// <see cref="Reflector" /> and various <see cref="Rotor" /> objects.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>
    /// The right side of the first <see cref="Rotor" /> is <see langword="null" /> and the left
    /// side is connected to the second <see cref="Rotor" />.
    /// </item>
    /// <item>
    /// The right side of the last <see cref="Rotor" /> is connected to the previous
    /// <see cref="Rotor" /> in sequence and the left side is connected to the
    /// <see cref="Reflector" />.
    /// </item>
    /// <item>
    /// For every <see cref="Rotor" /> but the first and last, their right side is connected to the
    /// previous <see cref="Rotor" /> in sequence, and their left side is connected to the next
    /// <see cref="Rotor" /> in sequence.
    /// </item>
    /// <item>
    /// The right side of the <see cref="Reflector" /> is connected to the left side of the last
    /// <see cref="Rotor" />. (The <see cref="Reflector" /> only has one side.)
    /// </item>
    /// </list>
    /// </remarks>
    private void BuildEnigmaMachine()
    {
        for (int i = 0; i < NumberOfRotors; i++)
        {
            if (i > 0)
            {
                _rotors[i].ConnectRightComponent(_rotors[i - 1]);
            }

            if (i < NumberOfRotors - 1)
            {
                _rotors[i].ConnectLeftComponent(_rotors[i + 1]);
            }
            else
            {
                _rotors[i].ConnectLeftComponent(MyReflector);
            }
        }

        MyReflector.ConnectRightComponent(_rotors[^1]);
        _cipherWheelIndexes = new int[NumberOfRotors + 1];
        IsInitialized = false;
    }
}