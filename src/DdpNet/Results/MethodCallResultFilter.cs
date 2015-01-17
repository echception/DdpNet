namespace DdpNet.Results
{
    using System.Linq;
    using Messages;

    internal class MethodCallResultFilter : ResultFilter
    {
        private bool updatedCalled;
        private bool resultCalled;

        private ReturnedObject returnedObject;

        private readonly string methodId;

        internal MethodCallResultFilter(string methodId)
        {
            this.updatedCalled = false;
            this.resultCalled = false;

            this.methodId = methodId;
        }

        internal override void HandleReturnObject(ReturnedObject returnedObject)
        {
            if (returnedObject.MessageType == "result" && ((string)(returnedObject.ParsedObject["id"])) == this.methodId)
            {
                this.resultCalled = true;
                this.returnedObject = returnedObject;
            }
            else if (returnedObject.MessageType == "updated")
            {
                var updatedObject = returnedObject.ParsedObject.ToObject<Updated>();

                if (updatedObject.Methods.Contains(this.methodId))
                {
                    this.updatedCalled = true;
                }
            }
        }

        internal override bool IsCompleted()
        {
            return this.updatedCalled && this.resultCalled;
        }

        internal override ReturnedObject GetReturnedObject()
        {
            return this.returnedObject;
        }
    }
}
