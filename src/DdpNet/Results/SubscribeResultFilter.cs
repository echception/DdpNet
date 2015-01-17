namespace DdpNet.Results
{
    using System.Linq;
    using Messages;

    internal class SubscribeResultFilter : ResultFilter
    {
        private bool readyCalled;
        private bool unsubCalled;

        private ReturnedObject returnedObject;

        private readonly string subscriptionId;
        internal SubscribeResultFilter(string subscriptionId)
        {
            this.readyCalled = false;
            this.unsubCalled = false;

            this.subscriptionId = subscriptionId;
        }

        internal override void HandleReturnObject(ReturnedObject returnedObject)
        {
            if (returnedObject.MessageType == "ready")
            {
                var readyObject = returnedObject.ParsedObject.ToObject<Ready>();

                if (readyObject.SubscriptionsReady.Contains(this.subscriptionId))
                {
                    this.readyCalled = true;
                    this.returnedObject = returnedObject;
                }
            }
        }

        internal override bool IsCompleted()
        {
            return this.readyCalled || this.unsubCalled;
        }

        internal override ReturnedObject GetReturnedObject()
        {
            return this.returnedObject;
        }
    }
}
