namespace DdpNet.Results
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    internal delegate bool ResultFilter(Result result);

    internal delegate void ResultCallback(Result result);

    internal class ResultHandler
    {
        private List<RegisteredResultCallback> callbacks;

        private Dictionary<WaitHandle, RegisteredResultWait> waits;

        private Dictionary<RegisteredResultWait, Result> waitResults;

        internal ResultHandler()
        {
            this.callbacks = new List<RegisteredResultCallback>();
            this.waits = new Dictionary<WaitHandle, RegisteredResultWait>();
            this.waitResults = new Dictionary<RegisteredResultWait, Result>();
        }
 
        internal WaitHandle RegisterWaitHandler(ResultFilter filter)
        {
            var waitEvent = new ManualResetEvent(false);
            var registeredWait = new RegisteredResultWait(filter, waitEvent);
            var handle = new WaitHandle();
            this.waits.Add(handle, registeredWait);

            return handle;
        }

        internal void RegisterResultCallback(ResultFilter filter, ResultCallback callback)
        {
            this.callbacks.Add(new RegisteredResultCallback(filter, callback));
        }

        internal Task<Result> WaitForResult(WaitHandle waitHandle)
        {
            return Task.Factory.StartNew(() =>
            {
                if (waitHandle.Triggered)
                {
                    throw new InvalidOperationException("WaitHandle has already been triggered");
                }

                RegisteredResultWait wait;
                if (!this.waits.TryGetValue(waitHandle, out wait))
                {
                    throw new InvalidOperationException("Specified WaitHandle does not exist");
                }

                if (!wait.WaitEvent.WaitOne(TimeSpan.FromSeconds(5)))
                {
                    throw new TimeoutException("Response was never received");
                }

                Result result;
                if (!this.waitResults.TryGetValue(wait, out result))
                {
                    throw new InvalidOperationException("Wait was triggered, but no result was available");
                }

                return result;
            });
        }

        internal void AddResult(Result newResult)
        {
            var waitsToRemove = new List<WaitHandle>();
            var callbacksToRemove = new List<RegisteredResultCallback>();

            foreach (var wait in this.waits)
            {
                if (wait.Value.Filter(newResult))
                {
                    this.waitResults.Add(wait.Value, newResult);
                    wait.Value.WaitEvent.Set();
                    waitsToRemove.Add(wait.Key);
                }
            }

            foreach (var callback in this.callbacks)
            {
                if (callback.Filter(newResult))
                {
                    callback.Callback(newResult);
                    callbacksToRemove.Add(callback);
                }
            }

            foreach (var removedCallback in callbacksToRemove)
            {
                this.callbacks.Remove(removedCallback);
            }

            foreach (var removedWait in waitsToRemove)
            {
                this.waits.Remove(removedWait);
            }
        }
    }
}
