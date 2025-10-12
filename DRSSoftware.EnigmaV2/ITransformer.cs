namespace DRSSoftware.EnigmaV2;

internal interface ITransformer
{
    public void Initialize(string seed);

    public void SetIndex(int indexValue);

    public int TransformIn(int c, bool shouldRotate);
}