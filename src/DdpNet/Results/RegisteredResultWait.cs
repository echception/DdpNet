namespace DdpNet.Results
{
    using System;
    using System.Threading;

    internal class RegisteredResultWait
    {
        internal ResultFilter Filter { get; private set; }
        internal ManualResetEvent WaitEvent { get; private set; }

        internal RegisteredResultWait(ResultFilter filter, ManualResetEvent waitEvent)
        {
            this.Filter = filter;
            this.WaitEvent = waitEvent;
        }
    }
}
