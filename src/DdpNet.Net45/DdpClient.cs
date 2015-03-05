namespace DdpNet
{
    using System;
    using Connection;

    public class DdpClient : BaseDdpClient
    {
        public DdpClient(Uri serverUri) : base(new ClientWebSocketConnection(serverUri))
        {
        }

        internal DdpClient(IWebSocketConnection connection) : base(connection)
        {
        }
    }
}
