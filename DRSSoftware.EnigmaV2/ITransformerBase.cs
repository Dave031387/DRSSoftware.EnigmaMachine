namespace DRSSoftware.EnigmaV2;

internal interface ITransformerBase
{
    public void Initialize(string seed);

    public void SetWheelIndex(int wheelIndex);

    public int TransformIn(int c, bool shouldRotateWheel);
}