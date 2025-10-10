namespace DRSSoftware.EnigmaV2;

internal interface IEnigmaReflector : ITransformerBase
{
    public void ConnectOutgoingWheel(IEnigmaWheel enigmaWheel);
}