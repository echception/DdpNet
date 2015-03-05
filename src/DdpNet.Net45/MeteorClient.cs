namespace DdpNet
{
    using System;
    using Connection;

    public class MeteorClient : BaseMeteorClient
    {
        public MeteorClient(Uri serverUri) : base(new ClientWebSocketConnection(serverUri))
        {
        }

        internal MeteorClient(IWebSocketConnection webSocketConnection) : base(webSocketConnection)
        {
            
        }
    }
}
