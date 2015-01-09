namespace DdpNet.Results
{
    using System;

    internal class ReturnedObject
    {
        internal String MessageType { get; private set; }
        internal dynamic ParsedObject { get; private set; }
        internal string Message { get; private set; }

        internal ReturnedObject(string messageType, dynamic parsedObject, string message)
        {
            this.Message = message;
            this.MessageType = messageType;
            this.ParsedObject = parsedObject;
        }
    }
}
