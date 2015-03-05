namespace DdpNet.ParameterObjects
{
    using Newtonsoft.Json;

    internal class Set
    {
        [JsonProperty(PropertyName = "$set")]
        public object ObjectToSet { get; set; }

        public Set(object objectToSet)
        {
            this.ObjectToSet = objectToSet;
        }
    }
}
