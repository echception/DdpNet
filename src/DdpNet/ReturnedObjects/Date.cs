﻿namespace DdpNet.ReturnedObjects
{
    using System;
    using Newtonsoft.Json;

    internal class Date
    {
        [JsonProperty(PropertyName = "$date")]
        public long MillisecondsSinceEposh { get; set; }

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        [JsonIgnore]
        public DateTime DateTime
        {
            get { return Epoch.AddMilliseconds(this.MillisecondsSinceEposh); }
        }
    }
}
