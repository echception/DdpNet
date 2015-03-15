namespace DdpNet.Results
{
    internal class ConnectedResultFilter : ResultFilter
    {
        private bool connectedCalled;
        private bool failedCalled;

        private ReturnedObject returnedObject;

        public ConnectedResultFilter()
        {
            this.connectedCalled = false;
            this.failedCalled = false;
        }
        internal override void HandleReturnObject(ReturnedObject returnObject)
        {
            if (returnObject.MessageType == "connected")
            {
                this.connectedCalled = true;
                this.returnedObject = returnObject;
            }
            else if (returnObject.MessageType == "failed")
            {
                this.failedCalled = true;
                this.returnedObject = returnObject;
            }
        }

        internal override bool IsCompleted()
        {
            return this.connectedCalled || this.failedCalled;
        }

        internal override ReturnedObject GetReturnedObject()
        {
            return this.returnedObject;
        }
    }
}
