namespace DdpNet
{
    using System;
    using System.Security.Cryptography;
    using System.Text;
    using ParameterObjects;

    internal static class Utilities
    {
        internal static String GenerateID()
        {
            return Guid.NewGuid().ToString("N");
        }

        internal static Password GetPassword(string password)
        {
            var bytes = Encoding.UTF8.GetBytes(password);
            var hashString = new SHA256Managed();
            var hashedPassword = hashString.ComputeHash(bytes);
            var hashedString = string.Empty;

            foreach (var x in hashedPassword)
            {
                hashedString += string.Format("{0:x2}", x);
            }

            return new Password(hashedString, "sha-256");
        }
    }
}
