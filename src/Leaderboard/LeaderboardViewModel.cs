namespace Leaderboard
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using DdpNet;

    public class LeaderboardViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DdpFilteredCollection<Player> players;

        private Player selectedPlayer;

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

        public LeaderboardViewModel(DdpFilteredCollection<Player> players)
        {
            this.Players = players;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
