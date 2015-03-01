namespace DdpNet
{
    using System;

    internal class ForceSerializeDdpObjectId : IDisposable
    {
        private readonly DdpObject ddpObject;
        private readonly bool originalValue;

        public ForceSerializeDdpObjectId(DdpObject ddpObject, bool shouldSerialize)
        {
            Exceptions.ThrowIfNull(ddpObject, "ddpObject");

            this.ddpObject = ddpObject;

            this.originalValue = this.ddpObject.SerializeId;
            this.ddpObject.SerializeId = shouldSerialize;
        }

        public void Dispose()
        {
            this.ddpObject.SerializeId = this.originalValue;
        }
    }
}
