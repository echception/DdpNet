namespace DdpNet
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Connection;
    using ParameterObjects;

    public class MeteorClient : DdpClient, INotifyPropertyChanged
    {
        private UserSession userSession;
        private MeteorUser user;

        private DdpCollection<MeteorUser> users; 

        public UserSession UserSession
        {
            get
            {
                return this.userSession;
            }
            private set
            {
                if (value != this.userSession)
                {
                    this.userSession = value;
                    this.OnPropertyChanged();

                    if (this.userSession != null && !string.IsNullOrWhiteSpace(this.userSession.UserId))
                    {
                        var userObject = this.users.Single(x => x.Id == this.userSession.UserId);

                        this.User = userObject;
                    }
                    else
                    {
                        this.User = null;
                    }
                }
            }
        }

        public MeteorUser User
        {
            get { return this.user; }
            private set
            {
                if (this.user != value)
                {
                    this.user = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public MeteorClient(Uri serverUri) : base(serverUri)
        {
            this.Initialize();
        }

        internal MeteorClient(IWebSocketConnection webSocketConnection)
            : base(webSocketConnection)
        {
            this.Initialize();
        }

        public Task LoginPassword(string userName, string password)
        {
            Exceptions.ThrowIfNullOrWhitespace(userName, "userName");
            Exceptions.ThrowIfNullOrWhitespace(password, "password");

            var passwordParameter = Utilities.GetPassword(password);
            var userParameter = new UserLogin(userName);

            var loginParameters = new LoginPasswordParameters(userParameter, passwordParameter);

            return this.CallLoginMethod("login", loginParameters);
        }

        public Task LoginResumeSession(string token)
        {
            Exceptions.ThrowIfNullOrWhitespace(token, "token");

            var loginResume = new LoginResume(token);

            return this.CallLoginMethod("login", loginResume);
        }

        public Task Logout()
        {
            return this.CallLogoutMethod();
        }

        internal async Task CallLoginMethod(string methodName, params object[] parameters)
        {
            var loginResult = await this.Call<UserSession>(methodName, parameters);

            this.UserSession = loginResult;
        }

        internal async Task CallLogoutMethod()
        {
            await this.Call("logout");

            this.UserSession = null;
        }

        private void Initialize()
        {
            this.users = this.GetCollection<MeteorUser>("users");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler1 = PropertyChanged;
            if (handler1 != null) handler1(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
