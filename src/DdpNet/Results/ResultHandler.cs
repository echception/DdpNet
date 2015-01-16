namespace DdpNet.Results
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal delegate bool ResultFilter(ReturnedObject returnedObject);

    internal delegate void ResultCallback(ReturnedObject returnedObject);

    internal class ResultHandler
    {
        private List<RegisteredResultCallback> callbacks;

        private Dictionary<WaitHandle, RegisteredResultWait> waits;

        private Dictionary<RegisteredResultWait, ReturnedObject> waitResults;

        internal ResultHandler()
        {
            this.callbacks = new List<RegisteredResultCallback>();
            this.waits = new Dictionary<WaitHandle, RegisteredResultWait>();
            this.waitResults = new Dictionary<RegisteredResultWait, ReturnedObject>();
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

        internal Task<ReturnedObject> WaitForResult(WaitHandle waitHandle)
        {
            return Task.Factory.StartNew(() =>
            {
                if (waitHandle.Triggered)
                {
                    throw new InvalidOperationException("WaitHandle has already completed");
                }

                RegisteredResultWait wait;
                if (!this.waits.TryGetValue(waitHandle, out wait))
                {
                    throw new InvalidOperationException("Specified WaitHandle does not exist");
                }

                if (!wait.WaitEvent.WaitOne(TimeSpan.FromSeconds(50)))
                {
                    throw new TimeoutException("Response was never received");
                }

                waitHandle.Triggered = true;

                ReturnedObject returnedObject;
                if (!this.waitResults.TryGetValue(wait, out returnedObject))
                {
                    throw new InvalidOperationException("Wait was triggered, but no result was available");
                }

                this.waitResults.Remove(wait);

                return returnedObject;
            });
        }

        internal void AddResult(ReturnedObject newReturnedObject)
        {
            var callbacksToRemove = new List<RegisteredResultCallback>();

            var waitsToIterate = this.waits.ToList();
            var callbacksToIterate = this.callbacks.ToList();

            foreach (var wait in waitsToIterate)
            {
                if (wait.Value.Filter(newReturnedObject))
                {
                    this.waitResults.Add(wait.Value, newReturnedObject);
                    wait.Value.WaitEvent.Set();
                }
            }

            foreach (var callback in callbacksToIterate)
            {
                if (callback.Filter(newReturnedObject))
                {
                    callback.Callback(newReturnedObject);
                    callbacksToRemove.Add(callback);
                }
            }

            foreach (var removedCallback in callbacksToRemove)
            {
                this.callbacks.Remove(removedCallback);
            }
        }
    }
}
