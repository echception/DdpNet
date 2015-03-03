namespace DdpNet.Packages.Accounts
{
    using System.Threading.Tasks;
    using ParameterObjects;

    public static class AccountsExtensionMethods
    {
        public static Task CreateUserWithUserName(this MeteorClient client, string userName, string password)
        {
            Exceptions.ThrowIfNullOrWhitespace(userName, "userName");
            Exceptions.ThrowIfNullOrWhitespace(password, "password");

            var passwordParameters = Utilities.GetPassword(password);
            var parameters = new CreateUserUserNameParameters(userName, passwordParameters);

            return client.CallLoginMethod("createUser", parameters);
        }
    }
}
