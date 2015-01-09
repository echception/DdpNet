namespace DdpNet.ParameterObjects
{
    using Newtonsoft.Json;

    internal class User
    {
        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        public User(string userName)
        {
            this.UserName = userName;
        }
    }
}
