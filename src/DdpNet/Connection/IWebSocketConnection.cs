namespace DdpNet.Connection
{
    using System.Threading.Tasks;

    public interface IWebSocketConnection
    {
        Task ConnectAsync();

        Task CloseAsync();

        Task SendAsync(string text);
        Task<string> ReceiveAsync();
    }
}
