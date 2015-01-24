namespace DdpNet.Connection
{
    using System.Threading.Tasks;

    internal interface IDdpConnectionSender
    {
        Task SendObject(object objectToSend);
    }
}
