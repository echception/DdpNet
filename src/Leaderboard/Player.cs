// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Player.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   The player.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Leaderboard
{
    using DdpNet;

    using Newtonsoft.Json;

    /// <summary>
    /// The player. Inherits from DdpObject so it can be used in DdpCollections
    /// </summary>
    public class Player : DdpObject
    {
        #region Fields

        /// <summary>
        /// The name of the player
        /// </summary>
        private string name;

        /// <summary>
        /// The player's score.
        /// </summary>
        private int score;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the score.
        /// </summary>
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

        #endregion
    }
}