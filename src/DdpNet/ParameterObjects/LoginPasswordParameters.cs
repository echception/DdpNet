namespace DdpNet.ParameterObjects
{
    using Newtonsoft.Json;

    internal class LoginPasswordParameters
    {
        [JsonProperty(PropertyName = "user")]
        public User User { get; set; }

        [JsonProperty(PropertyName = "password")]
        public Password Password { get; set; }

        public LoginPasswordParameters(User user, Password password)
        {
            this.User = user;
            this.Password = password;
        }
    }
}
