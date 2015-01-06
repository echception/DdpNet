namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;

    internal interface IMessageHandler
    {
        Task HandleMessage(DdpClient client, string message);
        bool CanHandle(string message);
    }
}
