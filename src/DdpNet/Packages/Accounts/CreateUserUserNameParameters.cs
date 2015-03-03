namespace DdpNet.Packages.Accounts
{
    using System;
    using Newtonsoft.Json;
    using ParameterObjects;

    internal class CreateUserUserNameParameters
    {
        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "password")]
        public Password Password { get; set; }

        public CreateUserUserNameParameters(string username, Password password)
        {
            this.UserName = username;
            this.Password = password;
        }
    }
}
