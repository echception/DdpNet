namespace DdpNet
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class MeteorUser : DdpObject
    {
        [JsonProperty(PropertyName = "username")]
        public string UserName { get; private set; }

        [JsonProperty(PropertyName = "emails")]
        public UserEmailAddress[] Emails { get; private set; }

        [JsonProperty(PropertyName = "profile")]
        public JObject Profile { get; private set; }
    }
}
