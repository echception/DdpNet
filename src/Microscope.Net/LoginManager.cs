namespace Microscope.Net
{
    using Windows.Storage;

    public class LoginManager
    {
        private const string TokenName = "SessionToken";

        public LoginManager()
        {
            
        }

        public string GetSavedSessionToken()
        {
            object sessionToken;
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(TokenName, out sessionToken))
            {
                return (string)sessionToken;
            }

            return string.Empty;
        }

        public void SaveSessionToken(string sessionToken)
        {
            ApplicationData.Current.LocalSettings.Values[TokenName] = sessionToken;
        }

        public void ClearToken()
        {
            ApplicationData.Current.LocalSettings.Values.Remove(TokenName);
        }
    }
}
