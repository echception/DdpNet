namespace DdpNet.UnitTest
{
    using System;
    using Messages;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using ParameterObjects;

    [TestClass]
    public class MeteorClientTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MeteorClient_LoginPassword_NullUsername()
        {
            var websocket = new InMemoryConnection();

            var client = new MeteorClient(websocket);

            client.LoginPassword(string.Empty, "Password");
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void MeteorClient_LoginPassword_NullPassword()
        {
            var websocket = new InMemoryConnection();

            var client = new MeteorClient(websocket);

            client.LoginPassword("Username", String.Empty);
        }

        [TestMethod]
        public void MeteorClient_LoginPassword_CallsServerMethod()
        {
            var websocket = new InMemoryConnection();

            var client = new MeteorClient(websocket);

            websocket.Reply(JsonConvert.SerializeObject(new Connected() { Session = "TestSession"}));
            client.ConnectAsync().Wait();

            var loginTask = client.LoginPassword("Username", "Password");

            Method method = null;

            while (method == null)
            {
                var message = websocket.GetSentMessage();
                var jobject =  JObject.Parse(message);
                if (jobject["msg"] != null && jobject["msg"].Value<string>() == "method")
                {
                    method = JsonConvert.DeserializeObject<Method>(message);
                }
            }

            websocket.Reply(JsonConvert.SerializeObject(new Result() { ID = method.ID }));
            websocket.Reply(JsonConvert.SerializeObject(new Updated() { Methods = new []{method.ID}}));

            loginTask.Wait();

            Assert.AreEqual("login", method.MethodName);
            Assert.AreEqual(1, method.Parameters.Length);
            var loginParameters = JsonConvert.DeserializeObject<LoginPasswordParameters>(method.Parameters[0].ToString());

            Assert.AreEqual("Username", loginParameters.User.UserName);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(loginParameters.Password.Digest));
            Assert.AreEqual("sha-256", loginParameters.Password.Algorithm);
        }
    }
}
