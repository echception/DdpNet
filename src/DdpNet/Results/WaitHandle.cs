namespace DdpNet.Results
{
    using System.Threading;

    internal class WaitHandle
    {
        internal bool Triggered { get; private set; }

        internal WaitHandle()
        {
            this.Triggered = false;
        }
    }
}
