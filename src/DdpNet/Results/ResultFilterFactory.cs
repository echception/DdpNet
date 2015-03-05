namespace DdpNet.Results
{
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

        internal static ResultFilter CreateUnsubscribeResultFilter(string id)
        {
            return new UnsubscribeResultFilter(id);
        }
    }
}
