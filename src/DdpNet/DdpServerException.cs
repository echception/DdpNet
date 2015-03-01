namespace DdpNet
{
    using System;
    using Messages;

    public class DdpServerException : Exception
    {
        public string Error { get; private set; }
        public string Details { get; private set; }
        public string Reason { get; private set; }

        internal DdpServerException(Error error)
            : base(string.Format("Server returned an error {0}. Details: {1}. Reason: {2}", error.ErrorMessage, error.Details, error.Reason))
        {
            this.Error = error.ErrorMessage;
            this.Details = error.Details;
            this.Reason = error.Reason;
        }
    }
}
