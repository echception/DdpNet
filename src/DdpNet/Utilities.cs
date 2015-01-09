namespace DdpNet
{
    using System;

    internal static class Utilities
    {
        internal static String GenerateID()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
