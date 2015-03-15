namespace DdpNet
{
    using System;
    using Newtonsoft.Json;
    using ReturnedObjects;

    public class UserSession
    {
        [JsonProperty(PropertyName = "id")]
        public string UserId { get; private set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; private set; }

        [JsonProperty(PropertyName = "tokenExpires")]
        private DdpDate TokenExpires { get; set; }

        [JsonIgnore]
        public DateTime TokenExpiration { get { return this.TokenExpires.DateTime; }}
    }
}
