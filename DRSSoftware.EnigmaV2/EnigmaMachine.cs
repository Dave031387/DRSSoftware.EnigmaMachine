using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DRSSoftware.EnigmaV2.Tests")]

namespace DRSSoftware.EnigmaV2;

public class EnigmaMachine
{
    internal readonly EnigmaReflector _reflector = new();
    internal readonly EnigmaWheel _wheel1 = new();
    internal readonly EnigmaWheel _wheel2 = new();
    internal readonly EnigmaWheel _wheel3 = new();
    internal readonly EnigmaWheel _wheel4 = new();
    internal bool _isInitialized;

    public EnigmaMachine()
    {
        _wheel1.ConnectOutgoingWheel(_wheel2);

        _wheel2.ConnectIncomingWheel(_wheel1);
        _wheel2.ConnectOutgoingWheel(_wheel3);

        _wheel3.ConnectIncomingWheel(_wheel2);
        _wheel3.ConnectOutgoingWheel(_wheel4);

        _wheel4.ConnectIncomingWheel(_wheel3);
        _wheel4.ConnectOutgoingWheel(_reflector);

        _reflector.ConnectOutgoingWheel(_wheel4);
    }

    public void Initialize(string seed)
    {
        char[] chars = seed.ToCharArray();
        string seed1 = GenerateNewSeed(chars, 2);
        string seed2 = GenerateNewSeed(chars, 3);
        string seed3 = GenerateNewSeed(chars, 4);
        string seed4 = GenerateNewSeed(chars, 5);
        _reflector.Initialize(seed);
        _wheel1.Initialize(seed1);
        _wheel2.Initialize(seed2);
        _wheel3.Initialize(seed3);
        _wheel4.Initialize(seed4);
        _isInitialized = true;
    }

    public void SetWheelIndexes(int index1, int index2, int index3, int index4)
    {
        if (_isInitialized)
        {
            _wheel1.SetWheelIndex(index1);
            _wheel2.SetWheelIndex(index2);
            _wheel3.SetWheelIndex(index3);
            _wheel4.SetWheelIndex(index4);
        }
        else
        {
            throw new InvalidOperationException("The Enigma machine must be initialized before setting the wheel indexes.");
        }
    }

    public string Transform(string text)
    {
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
                int transformed = _wheel1.TransformIn(original, true);

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
}