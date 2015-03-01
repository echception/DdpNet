using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DdpNet
{
    using System.Security.Cryptography;
    using Connection;
    using ParameterObjects;

    public class MeteorClient : DdpClient
    {
        public MeteorClient(Uri serverUri) : base(serverUri)
        {
        }

        internal MeteorClient(IWebSocketConnection webSocketConnection) : base(webSocketConnection)
        {
        }

        public Task LoginPassword(string userName, string password)
        {
            var passwordParameter = this.GetPassword(password);
            var userParameter = new User(userName);

            var loginParameters = new LoginPasswordParameters(userParameter, passwordParameter);

            return this.Call("login", loginParameters);
        }

        private Password GetPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashString = new SHA256Managed();
            var hashedPassword = hashString.ComputeHash(bytes);
            var hashedString = string.Empty;

            foreach (var x in hashedPassword)
            {
                hashedString += string.Format("{0:x2}", x);
            }

            return new Password(hashedString, "sha-256");
        }
    }
}
