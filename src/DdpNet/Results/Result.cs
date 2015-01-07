namespace DdpNet.Results
{
    using System;

    internal class Result
    {
        internal String MessageType { get; private set; }
        internal dynamic ParsedObject { get; private set; }
        internal string Message { get; private set; }

        internal Result(string messageType, dynamic parsedObject, string message)
        {
            this.Message = message;
            this.MessageType = messageType;
            this.ParsedObject = parsedObject;
        }
    }
}
