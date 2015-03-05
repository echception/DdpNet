namespace DdpNet
{
    using Newtonsoft.Json;

    public class UserEmailAddress
    {
        [JsonProperty(PropertyName="address")]
        public string Address { get; private set; }

        [JsonProperty(PropertyName="verified")]
        public bool Verified { get; private set; }
    }
}
