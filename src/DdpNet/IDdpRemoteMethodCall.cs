namespace DdpNet
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal interface IDdpRemoteMethodCall
    {
        Task Call(string methodName, params object[] parameters);
        Task<T> Call<T>(string methodName, params object[] parameters);
    }
}
