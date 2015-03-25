using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Leaderboard
{
    using DdpNet;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private LeaderboardViewModel viewModel;

        private DdpCollection<Player> players;

        private MeteorClient client;

        public MainWindow()
        {
            InitializeComponent();

            this.Initialize();
        }

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

        private async void AddPointsButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.viewModel.SelectedPlayer != null)
            {
                var update = new Dictionary<string, object>() { { "score", this.viewModel.SelectedPlayer.Score + 5 } };

                await players.UpdateAsync(this.viewModel.SelectedPlayer.Id, update);
            }
        }
    }
}
