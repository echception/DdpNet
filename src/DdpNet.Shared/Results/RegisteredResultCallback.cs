namespace DdpNet.Results
{
    using System;

    internal class RegisteredResultCallback
    {
        internal ResultFilter Filter { get; private set; }

        internal ResultCallback Callback { get; private set; }

        internal RegisteredResultCallback(ResultFilter filter, ResultCallback callback)
        {
            this.Filter = filter;
            this.Callback = callback;
        }
    }
}
