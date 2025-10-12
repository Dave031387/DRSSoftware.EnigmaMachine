namespace DRSSoftware.EnigmaV2;

internal interface IReflector : ITransformer
{
    public void ConnectOutgoing(IRotor rotor);
}