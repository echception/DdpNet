namespace DdpNet.Results
{
    using System;
    using Newtonsoft.Json.Linq;

    internal class ReturnedObject
    {
        internal String MessageType { get; private set; }
        internal JObject ParsedObject { get; private set; }
        internal string Message { get; private set; }

        internal ReturnedObject(string messageType, JObject parsedObject, string message)
        {
            this.Message = message;
            this.MessageType = messageType;
            this.ParsedObject = parsedObject;
        }
    }
}
