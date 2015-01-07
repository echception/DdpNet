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
        private List<Result> unhandledResults;

        private List<RegisteredResultCallback> callbacks;

        private List<RegisteredResultWait> waits;

        private object lockObject = new object();

        internal ResultHandler()
        {
            this.callbacks = new List<RegisteredResultCallback>();
            this.waits = new List<RegisteredResultWait>();
            this.unhandledResults = new List<Result>();
        }
 
        internal Task<Result> WaitForResult(ResultFilter filter)
        {
            return Task.Factory.StartNew(() =>
            {
                AutoResetEvent waitEvent;

                lock (lockObject)
                {
                    var existingResult = this.FindMatchingResult(filter);

                    if (existingResult != null)
                    {
                        this.unhandledResults.Remove(existingResult);
                        return existingResult;

                    }

                    waitEvent = new AutoResetEvent(false);
                    this.waits.Add(new RegisteredResultWait(filter, waitEvent));
                }

                if (!waitEvent.WaitOne(TimeSpan.FromSeconds(30)))
                {
                    throw new TimeoutException("Result was never received from the server");
                }

                var result = this.FindMatchingResult(filter);

                if (result == null)
                {
                    throw new InvalidOperationException("Wait handle was triggered, but no matching result was found");
                }

                this.unhandledResults.Remove(result);

                return result;
            });
        }

        internal void RegisterResultCallback(ResultFilter filter, ResultCallback callback)
        {
            var existingResult = this.FindMatchingResult(filter);

            if (existingResult != null)
            {
                this.unhandledResults.Remove(existingResult);
                callback(existingResult);
            }

            this.callbacks.Add(new RegisteredResultCallback(filter, callback));
        }

        internal void AddResult(Result newResult)
        {
            lock (lockObject)
            {
                this.unhandledResults.Add(newResult);

                var resultsToRemove = new List<Result>();

                foreach (var result in this.unhandledResults)
                {
                    foreach (var wait in this.waits)
                    {
                        if (wait.Filter(result))
                        {
                            wait.WaitEvent.Set();
                            continue;
                        }
                    }

                    foreach (var callback in this.callbacks)
                    {
                        if (callback.Filter(result))
                        {
                            resultsToRemove.Add(result);
                            callback.Callback(result);
                            continue;
                        }
                    }
                }

                foreach (var result in resultsToRemove)
                {
                    this.unhandledResults.Remove(result);
                }
            }
        }

        private Result FindMatchingResult(ResultFilter filter)
        {
            foreach (var result in this.unhandledResults)
            {
                if (filter(result))
                {
                    return result;
                }
            }

            return null;
        }
    }
}
