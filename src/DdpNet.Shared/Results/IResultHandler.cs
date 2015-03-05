namespace DdpNet.Results
{
    using System.Threading.Tasks;

    internal interface IResultHandler
    {
        /// <summary>
        /// Registers a WaitHandle. This is used to Wait on the result of a call, by passing it to WaitForResult.
        /// This should be called prior to making the function call, to guarentee the result is handled.
        /// </summary>
        /// <param name="filter">The filter used to determine the correct result</param>
        /// <returns>The registered WaitHandle</returns>
        WaitHandle RegisterWaitHandler(ResultFilter filter);

        /// <summary>
        /// Registers a callback. This will be called when the result of a function is found that matches the filter
        /// </summary>
        /// <param name="filter">The filter to determine the matching result</param>
        /// <param name="callback">The function to call when the result is received</param>
        void RegisterResultCallback(ResultFilter filter, ResultCallback callback);

        /// <summary>
        /// Returns a Task that completes when the result is received from the server.
        /// In the case of an error, an exception will be thrown from the task
        /// </summary>
        /// <param name="waitHandle">The handle to wait on</param>
        /// <returns>Task that completes when the result of a function call is returned</returns>
        Task<ReturnedObject> WaitForResult(WaitHandle waitHandle);

        /// <summary>
        /// Adds a result. This is called by the receive thread. It will iterate over all registered
        /// Wait and Callbacks to find a matchin one. When a match is found, it will either trigger the Wait, 
        /// or call the callback.
        /// </summary>
        /// <param name="newReturnedObject"></param>
        void AddResult(ReturnedObject newReturnedObject);
    }
}