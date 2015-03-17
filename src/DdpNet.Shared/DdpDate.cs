// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DdpDate.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   Contains the DdpDate class
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DdpNet
{
    using System;
    using System.Globalization;

    using Newtonsoft.Json;

    /// <summary>
    /// Wrapper for the Ddp Date object.
    /// Ddp defines dates as the number of milliseconds since the epoch.
    /// This class wraps that and exposes a DateTime object with the corresponding date
    /// This should be used in any data objects where a date is needed
    /// </summary>
    public class DdpDate
    {
        #region Static Fields

        /// <summary>
        /// The epoch.
        /// </summary>
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the date time.
        /// </summary>
        [JsonIgnore]
        public DateTime DateTime
        {
            get
            {
                return Epoch.AddMilliseconds(this.MillisecondsSinceEpoch);
            }
        }

        /// <summary>
        /// Gets or sets the milliseconds since epoch.
        /// </summary>
        [JsonProperty(PropertyName = "$date")]
        public long MillisecondsSinceEpoch { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Provides a default string representation
        /// </summary>
        /// <returns>
        /// The <see cref="string"/> representation.
        /// </returns>
        public override string ToString()
        {
            return this.DateTime.ToString("g", CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts the DdpDate to a string representation
        /// </summary>
        /// <param name="formatProvider">
        /// The format provider.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> representation.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return this.DateTime.ToString(formatProvider);
        }

        /// <summary>
        /// Converts the DdpDate to a string representation
        /// </summary>
        /// <param name="format">
        /// The format string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> representation.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.DateTime.ToString(System.String)", Justification = "Mirroring the DateTime ToString methods")]
        public string ToString(string format)
        {
            return this.DateTime.ToString(format);
        }

        /// <summary>
        /// Converts the DdpDate to a string representation
        /// </summary>
        /// <param name="format">
        /// The format string.
        /// </param>
        /// <param name="formatProvider">
        /// The format provider.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> representation.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return this.DateTime.ToString(format, formatProvider);
        }

        #endregion
    }
}