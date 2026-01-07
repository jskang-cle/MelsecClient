namespace MelsecClient;

public sealed class Melsec3EProtocol : MelsecEthProtocol
{
    internal Melsec3EProtocol(string ip, ushort port)
        : base(ip, port,
              errorCodePosition: 9,
              minResponseLength: 10,
              returnValuePosition: 11,
              returnPacketHeader: 0xD0,
              dataLengthPosition: 7)
    {
    }
    
    protected override byte[] GetPacketHead()
    {
        return [0x50, 0x00];
    }
}