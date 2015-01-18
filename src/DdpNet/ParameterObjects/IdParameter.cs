namespace DdpNet.ParameterObjects
{
    using Newtonsoft.Json;

    internal class IdParameter
    {
        [JsonProperty(PropertyName = "_id")]
        public string ID { get; set; }

        public IdParameter(string id)
        {
            this.ID = id;
        }
    }
}
