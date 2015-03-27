// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LeaderboardViewModel.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   The leaderboard view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Leaderboard
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using DdpNet;

    /// <summary>
    /// The leaderboard view model.
    /// </summary>
    public class LeaderboardViewModel : INotifyPropertyChanged
    {
        #region Fields

        /// <summary>
        /// The players.
        /// </summary>
        private DdpFilteredCollection<Player> players;

        /// <summary>
        /// The selected player.
        /// </summary>
        private Player selectedPlayer;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardViewModel"/> class.
        /// </summary>
        /// <param name="players">
        /// The players.
        /// </param>
        public LeaderboardViewModel(DdpFilteredCollection<Player> players)
        {
            this.Players = players;
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
        /// Gets or sets the players.
        /// </summary>
        public DdpFilteredCollection<Player> Players
        {
            get
            {
                return this.players;
            }

            set
            {
                this.players = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the selected player.
        /// </summary>
        public Player SelectedPlayer
        {
            get
            {
                return this.selectedPlayer;
            }

            set
            {
                this.selectedPlayer = value;
                this.OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invokes the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}