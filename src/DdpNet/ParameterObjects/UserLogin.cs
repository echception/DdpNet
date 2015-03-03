namespace DdpNet.ParameterObjects
{
    using Newtonsoft.Json;

    internal class UserLogin
    {
        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        public UserLogin(string userName)
        {
            this.UserName = userName;
        }
    }
}
