namespace DdpNet.Messages
{
    using Newtonsoft.Json.Linq;

    internal class Result : BaseMessage
    {
        public string ID { get; set; }

        public Error Error { get; set; }

        public JObject ResultObject { get; set; }

        protected Result() : base("result")
        {
        }
    }
}
