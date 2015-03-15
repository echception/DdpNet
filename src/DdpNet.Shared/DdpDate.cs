namespace DdpNet
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;

    public class DdpDate
    {
        [JsonProperty(PropertyName = "$date")]
        public long MillisecondsSinceEpoch { get; set; }

        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        [JsonIgnore]
        public DateTime DateTime
        {
            get { return Epoch.AddMilliseconds(this.MillisecondsSinceEpoch); }
        }

        public override string ToString()
        {
            return this.DateTime.ToString("g", CultureInfo.CurrentCulture);
        }
    }
}
