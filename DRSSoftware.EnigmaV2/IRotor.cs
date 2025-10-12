namespace DRSSoftware.EnigmaV2;

internal interface IRotor : ITransformer
{
    public void ConnectIncoming(IRotor rotor);

    public void ConnectOutgoing(ITransformer transformer);

    public int TransformOut(int c);
}