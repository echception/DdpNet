// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDdpRemoteMethodCall.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the DdpRemoteMethodCall interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    using System.Threading.Tasks;

    /// <summary>
    /// The DdpRemoteMethodCall interface. Exposes methods for calling remote methods on a DdpServer
    /// </summary>
    internal interface IDdpRemoteMethodCall
    {
        #region Public Methods and Operators

        /// <summary>
        /// Call a method on the server
        /// </summary>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <param name="parameters">
        /// The parameters to invoke the method with.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> which completes when the server returns the result of the call.
        /// </returns>
        Task Call(string methodName, params object[] parameters);

        /// <summary>
        /// Call a method on the server and get the return value
        /// </summary>
        /// <param name="methodName">
        /// The method name.
        /// </param>
        /// <param name="parameters">
        /// The parameters to invoke the method with.
        /// </param>
        /// <typeparam name="T">
        /// The type of the return value
        /// </typeparam>
        /// <returns>
        /// The <see cref="Task"/> which completes when the server returns the result of the call.
        /// </returns>
        Task<T> Call<T>(string methodName, params object[] parameters);

        #endregion
    }
}