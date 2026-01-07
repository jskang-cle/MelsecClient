using System.Net;
using System.Net.Sockets;

namespace MelsecClient;

sealed class TcpChannel : IChannel
{
    private readonly TcpClient _client;
    private readonly NetworkStream _stream;

    public TcpChannel(IPEndPoint endpoint)
    {
        _client = new TcpClient();
        _client.Connect(endpoint);
        _stream = _client.GetStream();
        if (!_stream.CanWrite) 
            throw new InvalidOperationException("Stream is not ready for writing");
    }

    public byte[] Execute(byte[] buffer)
    {
        _stream.Write(buffer, 0, buffer.Length);
        List<byte> lst = [];
        if (_stream.CanRead)
        {
            byte[] buff = new byte[1024];
            int n;
            do
            {
                n = _stream.Read(buff, 0, buff.Length);
                for (int i = 0; i < n; ++i)
                    lst.Add(buff[i]);
            }
            while (_stream.DataAvailable);
        }
        return [.. lst];
    }

    public int SendTimeout
    {
        get => _client.Client.SendTimeout;
        set => _client.Client.SendTimeout = value;
    }

    public int ReceiveTimeout
    {
        get => _client.Client.ReceiveTimeout;
        set => _client.Client.ReceiveTimeout = value;
    }

    public void Dispose()
    {
        _stream?.Dispose();
        _client?.Dispose();
    }
}
