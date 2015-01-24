namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;
    using Collections;
    using Connection;
    using Results;

    internal interface IMessageHandler
    {
        Task HandleMessage(IDdpConnectionSender client, ICollectionManager collectionManager, IResultHandler resultHandler, string message);
        bool CanHandle(string message);
    }
}
