namespace DdpNet.Messages
{
    using Newtonsoft.Json;

    internal class Connected : BaseMessage
    {
        [JsonProperty(PropertyName = "session")]
        public string Session { get; set; }

        internal Connected() : base("connected")
        {
            
        }
    }
}
