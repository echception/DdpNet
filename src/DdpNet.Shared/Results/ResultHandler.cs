// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResultHandler.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the ResultHandler class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Results
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The result callback.
    /// </summary>
    /// <param name="returnedObject">
    /// The returned object.
    /// </param>
    internal delegate void ResultCallback(ReturnedObject returnedObject);

    /// <summary>
    /// Handles the result of remote function calls made by the server
    /// </summary>
    internal class ResultHandler : IResultHandler
    {
        #region Fields

        /// <summary>
        /// List of callbacks registered
        /// </summary>
        private ConcurrentDictionary<RegisteredResultCallback, int> callbacks;

        /// <summary>
        /// List of WaitHandles registered
        /// </summary>
        private ConcurrentDictionary<WaitHandle, RegisteredResultWait> waits;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultHandler"/> class. 
        /// Creates a new ResultHandler
        /// </summary>
        internal ResultHandler()
        {
            this.callbacks = new ConcurrentDictionary<RegisteredResultCallback, int>();
            this.waits = new ConcurrentDictionary<WaitHandle, RegisteredResultWait>();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds a result. This is called by the receive thread. It will iterate over all registered
        /// Wait and Callbacks to find a matching one. When a match is found, it will either trigger the Wait, 
        /// or call the callback.
        /// </summary>
        /// <param name="newReturnedObject">
        /// The object returned from the server
        /// </param>
        public void AddResult(ReturnedObject newReturnedObject)
        {
            var callbacksToRemove = new List<RegisteredResultCallback>();

            var waitsToIterate = this.waits.ToArray();
            var callbacksToIterate = this.callbacks.ToArray();

            foreach (var wait in waitsToIterate)
            {
                wait.Value.Filter.HandleReturnObject(newReturnedObject);

                if (wait.Value.Filter.IsCompleted())
                {
                    wait.Value.WaitEvent.Set();
                    wait.Key.SetTriggered();
                }
            }

            foreach (var callback in callbacksToIterate)
            {
                callback.Key.Filter.HandleReturnObject(newReturnedObject);
                if (callback.Key.Filter.IsCompleted())
                {
                    callback.Key.Callback(newReturnedObject);
                    callbacksToRemove.Add(callback.Key);
                }
            }

            foreach (var removedCallback in callbacksToRemove)
            {
                int outInt;
                this.callbacks.TryRemove(removedCallback, out outInt);
            }
        }

        /// <summary>
        /// Registers a callback. This will be called when the result of a function is found that matches the filter
        /// </summary>
        /// <param name="filter">
        /// The filter to determine the matching result
        /// </param>
        /// <param name="callback">
        /// The function to call when the result is received
        /// </param>
        public void RegisterResultCallback(ResultFilter filter, ResultCallback callback)
        {
            if (!this.callbacks.TryAdd(new RegisteredResultCallback(filter, callback), 0))
            {
                throw new InvalidOperationException("Could not register callback");
            }
        }

        /// <summary>
        /// Registers a WaitHandle. This is used to Wait on the result of a call, by passing it to WaitForResult.
        /// This should be called prior to making the function call, to guarantee the result is handled.
        /// </summary>
        /// <param name="filter">
        /// The filter used to determine the correct result
        /// </param>
        /// <returns>
        /// The registered WaitHandle
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "RegisteredResultWait will manage the lifetime of the ManualResetEvent")]
        public WaitHandle RegisterWaitHandler(ResultFilter filter)
        {
            var waitEvent = new ManualResetEvent(false);
            var registeredWait = new RegisteredResultWait(filter, waitEvent);
            var handle = new WaitHandle();
            if (!this.waits.TryAdd(handle, registeredWait))
            {
                throw new InvalidOperationException("Wait is already registered");
            }

            return handle;
        }

        /// <summary>
        /// Returns a Task that completes when the result is received from the server.
        /// In the case of an error, an exception will be thrown from the task
        /// </summary>
        /// <param name="waitHandle">
        /// The handle to wait on
        /// </param>
        /// <returns>
        /// Task that completes when the result of a function call is returned
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "WaitHandle", Justification = "WaitHandle is a valid class name")]
        public Task<ReturnedObject> WaitForResult(WaitHandle waitHandle)
        {
            RegisteredResultWait wait;
            if (!this.waits.TryGetValue(waitHandle, out wait))
            {
                throw new InvalidOperationException("Specified WaitHandle does not exist");
            }

            return Task.Factory.StartNew(
                () =>
                    {
                        if (!wait.WaitEvent.WaitOne(TimeSpan.FromSeconds(30)))
                        {
                            throw new TimeoutException("Response was never received");
                        }

                        waitHandle.SetTriggered();
                        RegisteredResultWait outResult;
                        this.waits.TryRemove(waitHandle, out outResult);

                        return wait.Filter.GetReturnedObject();
                    });
        }

        #endregion
    }
}