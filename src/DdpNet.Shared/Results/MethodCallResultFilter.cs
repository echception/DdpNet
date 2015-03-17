// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodCallResultFilter.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the MethodCallResultFilter class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet.Results
{
    using System.Linq;

    using DdpNet.Messages;

    /// <summary>
    /// ResultFilter that looks for the results of a remote method call
    /// </summary>
    internal class MethodCallResultFilter : ResultFilter
    {
        #region Fields

        /// <summary>
        /// The method id.
        /// </summary>
        private readonly string methodId;

        /// <summary>
        /// Indicates if the 'result' message was received
        /// </summary>
        private bool resultCalled;

        /// <summary>
        /// The returned object.
        /// </summary>
        private ReturnedObject returnedObject;

        /// <summary>
        /// Indicates if the 'updated' message was received
        /// </summary>
        private bool updatedCalled;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MethodCallResultFilter"/> class.
        /// </summary>
        /// <param name="methodId">
        /// The method id.
        /// </param>
        internal MethodCallResultFilter(string methodId)
        {
            this.updatedCalled = false;
            this.resultCalled = false;

            this.methodId = methodId;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the returned object
        /// </summary>
        /// <returns>
        /// The <see cref="ReturnedObject"/>.
        /// </returns>
        internal override ReturnedObject GetReturnedObject()
        {
            return this.returnedObject;
        }

        /// <summary>
        /// Handles a message received from the server
        /// </summary>
        /// <param name="returnedObject">
        /// The returned object.
        /// </param>
        internal override void HandleReturnObject(ReturnedObject returnedObject)
        {
            if (returnedObject.MessageType == "result" && ((string)returnedObject.ParsedObject["id"]) == this.methodId)
            {
                this.resultCalled = true;
                this.returnedObject = returnedObject;
            }
            else if (returnedObject.MessageType == "updated")
            {
                var updatedObject = returnedObject.ParsedObject.ToObject<Updated>();

                if (updatedObject.Methods.Contains(this.methodId))
                {
                    this.updatedCalled = true;
                }
            }
        }

        /// <summary>
        /// Determines if the ResultFilter has completed
        /// </summary>
        /// <returns>
        /// True if its completed, false otherwise
        /// </returns>
        internal override bool IsCompleted()
        {
            return this.updatedCalled && this.resultCalled;
        }

        #endregion
    }
}