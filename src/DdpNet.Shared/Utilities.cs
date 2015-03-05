namespace DdpNet
{
    using System;
    using System.Text;
    using ParameterObjects;
    using PCLCrypto;

    internal static class Utilities
    {
        internal static String GenerateID()
        {
            return Guid.NewGuid().ToString("N");
        }

        internal static Password GetPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hasher = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha256);
            var hashedPassword = hasher.HashData(bytes);
            var hashedString = string.Empty;

            foreach (var x in hashedPassword)
            {
                hashedString += string.Format("{0:x2}", x);
            }

            return new Password(hashedString, "sha-256");
        }
    }
}
