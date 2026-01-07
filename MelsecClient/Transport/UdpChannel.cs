using System.Net;
using System.Net.Sockets;

namespace MelsecClient;

sealed class UdpChannel : IChannel
{
    private readonly UdpClient _client;
    private IPEndPoint _endPoint;

    public UdpChannel(IPEndPoint endpoint)
    {
        _endPoint = endpoint;
        _client = new UdpClient();
        _client.Connect(endpoint);
    }

    public byte[] Execute(byte[] buffer)
    {
        _client.Send(buffer, buffer.Length);
        return _client.Receive(ref _endPoint);
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
        _client?.Dispose();
    }
}
