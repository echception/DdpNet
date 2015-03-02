namespace DdpNet.Results
{
    using Messages;

    internal class UnsubscribeResultFilter : ResultFilter
    {
        private bool nosubCalled;

        private readonly string subscriptionId;

        private ReturnedObject returnedObject;

        internal UnsubscribeResultFilter(string subscriptionId)
        {
            this.subscriptionId = subscriptionId;
            this.nosubCalled = false;
        }

        internal override void HandleReturnObject(ReturnedObject returnedObject)
        {
            if (returnedObject.MessageType == "nosub")
            {
                var noSubObject = returnedObject.ParsedObject.ToObject<NoSubscribe>();

                if (noSubObject.ID == this.subscriptionId)
                {
                    this.nosubCalled = true;
                    this.returnedObject = returnedObject;
                }
            }
        }

        internal override bool IsCompleted()
        {
            return this.nosubCalled;
        }

        internal override ReturnedObject GetReturnedObject()
        {
            return this.returnedObject;
        }
    }
}
