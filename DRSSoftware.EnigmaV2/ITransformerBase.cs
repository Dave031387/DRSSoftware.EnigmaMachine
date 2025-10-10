namespace DRSSoftware.EnigmaV2;

internal interface ITransformerBase
{
    public void Initialize(string seed);

    public int TransformIn(int c, bool shouldRotateWheel);
}