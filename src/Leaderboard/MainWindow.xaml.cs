// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Leaderboard
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Windows;

    using DdpNet;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        /// <summary>
        /// The client.
        /// </summary>
        private MeteorClient client;

        /// <summary>
        /// The players.
        /// </summary>
        private DdpCollection<Player> players;

        /// <summary>
        /// The view model.
        /// </summary>
        private LeaderboardViewModel viewModel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();

            this.Initialize();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add points button_ on click.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private async void AddPointsButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.viewModel.SelectedPlayer != null)
            {
                var update = new Dictionary<string, object>() { { "score", this.viewModel.SelectedPlayer.Score + 5 } };

                await this.players.UpdateAsync(this.viewModel.SelectedPlayer.Id, update);
            }
        }

        /// <summary>
        /// Initializes the ViewModel from  Meteor
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task Initialize()
        {
            this.client = new MeteorClient(new Uri("ws://localhost:3000/websocket"));
            await this.client.ConnectAsync();

            this.players = this.client.GetCollection<Player>("players");
            DdpFilteredCollection<Player> sortedPlayers = this.players.Filter(
                sortFilter: (p1, p2) =>
                    {
                        var scoreComparison = p2.Score.CompareTo(p1.Score);

                        if (scoreComparison != 0)
                        {
                            return scoreComparison;
                        }

                        return string.Compare(p1.Name, p2.Name, System.StringComparison.Ordinal);
                    });

            this.viewModel = new LeaderboardViewModel(sortedPlayers);

            this.DataContext = this.viewModel;
        }

        #endregion
    }
}