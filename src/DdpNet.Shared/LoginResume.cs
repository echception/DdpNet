namespace DdpNet
{
    using Newtonsoft.Json;

    public class LoginResume
    {
        [JsonProperty(PropertyName = "resume")]
        public string Token { get; set; }

        public LoginResume(string token)
        {
            this.Token = token;
        }
    }
}
