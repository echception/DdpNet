namespace DdpNet.MessageHandlers
{
    using System.Threading.Tasks;

    internal class ConnectedHandler : BaseMessageHandler
    {
        public ConnectedHandler() : base("connected")
        {
        }

        public override Task HandleMessage(DdpClient client, string message)
        {
            throw new System.NotImplementedException();
        }
    }
}
