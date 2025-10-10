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
    internal readonly EnigmaReflector _reflector = new();
    internal readonly List<EnigmaWheel> _wheels = [];
    internal bool _isInitialized;

    public EnigmaMachine() => BuildEnigmaMachine();

    public void Initialize(string seed)
    {
        // TODO: validate seed parameter
        char[] chars = seed.ToCharArray();
        _reflector.Initialize(seed);

        for (int i = 0; i < NumberOfWheels; i++)
        {
            string alteredSeed = GenerateNewSeed(chars, i + 2);
            _wheels[i].Initialize(alteredSeed);
        }

        _isInitialized = true;
    }

    public void SetWheelIndexes(params int[] indexes)
    {
        // TODO: validate indexes parameter
        if (_isInitialized)
        {
            for (int i = 0; i < NumberOfWheels; ++i)
            {
                _wheels[i].SetWheelIndex(indexes[i]);
            }
        }
        else
        {
            throw new InvalidOperationException("The Enigma machine must be initialized before setting the wheel indexes.");
        }
    }

    public string Transform(string text)
    {
        // TODO: validate text parameter
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
                int transformed = _wheels[0].TransformIn(original, true);

                if (transformed == MaxIndex)
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

    private void BuildEnigmaMachine()
    {
        _wheels.Clear();

        for (int i = 0; i < NumberOfWheels; i++)
        {
            _wheels[i] = new EnigmaWheel();
        }

        for (int i = 0; i < NumberOfWheels; i++)
        {
            if (i is not 0)
            {
                _wheels[i].ConnectIncomingWheel(_wheels[i - 1]);
            }

            if (i < NumberOfWheels - 1)
            {
                _wheels[i].ConnectOutgoingWheel(_wheels[i + 1]);
            }
            else
            {
                _wheels[i].ConnectOutgoingWheel(_reflector);
            }
        }

        _reflector.ConnectOutgoingWheel(_wheels[NumberOfWheels - 1]);
        _isInitialized = false;
    }
}