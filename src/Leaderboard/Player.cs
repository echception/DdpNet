namespace Leaderboard
{
    using DdpNet;

    using Newtonsoft.Json;

    public class Player : DdpObject
    {
        private string name;

        private int score;

        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
                this.OnPropertyChanged();
            }
        }

        [JsonProperty(PropertyName = "score")]
        public int Score
        {
            get
            {
                return this.score;
            }
            set
            {
                this.score = value;
                this.OnPropertyChanged();
            }
        }
    }
}
