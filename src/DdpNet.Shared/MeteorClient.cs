// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MeteorClient.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the MeteorClient class
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DdpNet
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using DdpNet.Connection;
    using DdpNet.ParameterObjects;

    /// <summary>
    /// Extends the DdpClient to provide Meteor specific functions.
    /// In most cases when connecting to a Meteor server, this client should be used
    /// </summary>
    public class MeteorClient : DdpClient, INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The user.
        /// </summary>
        private MeteorUser user;

        /// <summary>
        /// The user session.
        /// </summary>
        private UserSession userSession;

        /// <summary>
        /// The users.
        /// </summary>
        private DdpCollection<MeteorUser> users;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteorClient"/> class.
        /// </summary>
        /// <param name="serverUri">
        /// The server uri.
        /// </param>
        public MeteorClient(Uri serverUri)
            : base(serverUri)
        {
            this.Initialize();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeteorClient"/> class.
        /// </summary>
        /// <param name="webSocketConnection">
        /// The web socket connection.
        /// </param>
        internal MeteorClient(IWebSocketConnection webSocketConnection)
            : base(webSocketConnection)
        {
            this.Initialize();
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The property changed event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the user.
        /// </summary>
        public MeteorUser User
        {
            get
            {
                return this.user;
            }

            private set
            {
                if (this.user != value)
                {
                    this.user = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the user session.
        /// </summary>
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

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Logins in with a user name and password
        /// </summary>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> which completes when the server returns the result of the login attempt.
        /// </returns>
        public Task LoginPassword(string userName, string password)
        {
            Exceptions.ThrowIfNullOrWhitespace(userName, "userName");
            Exceptions.ThrowIfNullOrWhitespace(password, "password");

            var passwordParameter = Utilities.GetPassword(password);
            var userParameter = new UserLogin(userName);

            var loginParameters = new LoginPasswordParameters(userParameter, passwordParameter);

            return this.CallLoginMethod("login", loginParameters);
        }

        /// <summary>
        /// Resumes a previous session
        /// </summary>
        /// <param name="token">
        /// The token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> which completes when the server returns the result of the resume attempt
        /// </returns>
        public Task LoginResumeSession(string token)
        {
            Exceptions.ThrowIfNullOrWhitespace(token, "token");

            var loginResume = new LoginResume(token);

            return this.CallLoginMethod("login", loginResume);
        }

        /// <summary>
        /// Logouts the user from this client
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>, which completes when the server returns the result of the logout
        /// </returns>
        public Task Logout()
        {
            return this.CallLogoutMethod();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Internal method for calling login methods. This will call the given server method,
        /// then set the UserSession
        /// </summary>
        /// <param name="methodName">
        /// The login method name.
        /// </param>
        /// <param name="parameters">
        /// The parameters to invoke the method with.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>, which completes when the server returns the result of the method.
        /// </returns>
        internal async Task CallLoginMethod(string methodName, params object[] parameters)
        {
            var loginResult = await this.Call<UserSession>(methodName, parameters);

            this.UserSession = loginResult;
        }

        /// <summary>
        /// Internal method to call Logout
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>, which completes when the server returns the result of the method
        /// </returns>
        internal async Task CallLogoutMethod()
        {
            await this.Call("logout");

            this.UserSession = null;
        }

        /// <summary>
        /// Raises the OnPropertyChanged event
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler1 = this.PropertyChanged;
            if (handler1 != null)
            {
                handler1(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Initializes the internal collections
        /// </summary>
        private void Initialize()
        {
            this.users = this.GetCollection<MeteorUser>("users");
        }

        #endregion
    }
}