namespace DdpNet.Results
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    internal delegate void ResultCallback(ReturnedObject returnedObject);

    /// <summary>
    /// Handles the result of remote function calls made by the server
    /// </summary>
    internal class ResultHandler : IResultHandler
    {
        /// <summary>
        /// List of callbacks registered
        /// </summary>
        private List<RegisteredResultCallback> callbacks;

        /// <summary>
        /// List of WaitHandles registered
        /// </summary>
        private Dictionary<WaitHandle, RegisteredResultWait> waits;

        /// <summary>
        /// Creates a new ResultHandler
        /// </summary>
        internal ResultHandler()
        {
            this.callbacks = new List<RegisteredResultCallback>();
            this.waits = new Dictionary<WaitHandle, RegisteredResultWait>();
        }
 
        /// <summary>
        /// Registers a WaitHandle. This is used to Wait on the result of a call, by passing it to WaitForResult.
        /// This should be called prior to making the function call, to guarentee the result is handled.
        /// </summary>
        /// <param name="filter">The filter used to determine the correct result</param>
        /// <returns>The registered WaitHandle</returns>
        public WaitHandle RegisterWaitHandler(ResultFilter filter)
        {
            var waitEvent = new ManualResetEvent(false);
            var registeredWait = new RegisteredResultWait(filter, waitEvent);
            var handle = new WaitHandle();
            this.waits.Add(handle, registeredWait);

            return handle;
        }

        /// <summary>
        /// Registers a callback. This will be called when the result of a function is found that matches the filter
        /// </summary>
        /// <param name="filter">The filter to determine the matching result</param>
        /// <param name="callback">The function to call when the result is received</param>
        public void RegisterResultCallback(ResultFilter filter, ResultCallback callback)
        {
            this.callbacks.Add(new RegisteredResultCallback(filter, callback));
        }

        /// <summary>
        /// Returns a Task that completes when the result is received from the server.
        /// In the case of an error, an exception will be thrown from the task
        /// </summary>
        /// <param name="waitHandle">The handle to wait on</param>
        /// <returns>Task that completes when the result of a function call is returned</returns>
        public Task<ReturnedObject> WaitForResult(WaitHandle waitHandle)
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

                waitHandle.SetTriggered();
                this.waits.Remove(waitHandle);

                return wait.Filter.GetReturnedObject();
            });
        }

        /// <summary>
        /// Adds a result. This is called by the receive thread. It will iterate over all registered
        /// Wait and Callbacks to find a matchin one. When a match is found, it will either trigger the Wait, 
        /// or call the callback.
        /// </summary>
        /// <param name="newReturnedObject"></param>
        public void AddResult(ReturnedObject newReturnedObject)
        {
            var callbacksToRemove = new List<RegisteredResultCallback>();

            var waitsToIterate = this.waits.ToList();
            var callbacksToIterate = this.callbacks.ToList();

            foreach (var wait in waitsToIterate)
            {
                wait.Value.Filter.HandleReturnObject(newReturnedObject);

                if (wait.Value.Filter.IsCompleted())
                {
                    wait.Value.WaitEvent.Set();
                }
            }

            foreach (var callback in callbacksToIterate)
            {
                callback.Filter.HandleReturnObject(newReturnedObject);
                if (callback.Filter.IsCompleted())
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
