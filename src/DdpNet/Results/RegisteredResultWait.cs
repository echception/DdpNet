namespace DdpNet.Results
{
    using System;
    using System.Threading;

    internal class RegisteredResultWait
    {
        internal ResultFilter Filter { get; private set; }
        internal AutoResetEvent WaitEvent { get; private set; }

        internal RegisteredResultWait(ResultFilter filter, AutoResetEvent waitEvent)
        {
            this.Filter = filter;
            this.WaitEvent = waitEvent;
        }
    }
}
