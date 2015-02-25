namespace DdpNet
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    internal interface IDdpRemoteMethodCall
    {
        Task Call(string methodName, List<object> parameters);
        Task<T> Call<T>(string methodName, List<object> parameters);
    }
}
