namespace DdpNet.Results
{
    using Microsoft.SqlServer.Server;

    internal static class ResultFilterFactory
    {
        internal static ResultFilter CreateSubscribeResultFilter(string subscriptionId)
        {
            return new SubscribeResultFilter(subscriptionId);
        }

        internal static ResultFilter CreateConnectResultFilter()
        {
            return new ConnectedResultFilter();
        }

        internal static ResultFilter CreateCallResultFilter(string methodId)
        {
            return new MethodCallResultFilter(methodId);
        }
    }
}
