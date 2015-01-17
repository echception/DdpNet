using System;
namespace DdpNet.Results
{
    internal abstract class ResultFilter
    {
        internal abstract void HandleReturnObject(ReturnedObject returnedObject);
        internal abstract bool IsCompleted();

        internal abstract ReturnedObject GetReturnedObject();
    }
}
