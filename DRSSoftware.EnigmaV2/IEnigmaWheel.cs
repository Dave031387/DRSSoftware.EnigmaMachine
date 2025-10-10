namespace DRSSoftware.EnigmaV2;

internal interface IEnigmaWheel : ITransformerBase
{
    public void ConnectIncomingWheel(IEnigmaWheel enigmaWheel);

    public void ConnectOutgoingWheel(ITransformerBase enigmaWheel);

    public void SetWheelIndex(int wheelIndex);

    public int TransformOut(int c);
}