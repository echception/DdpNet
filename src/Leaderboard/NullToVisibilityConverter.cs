// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NullToVisibilityConverter.cs" company="Chris Amert">
//   Copyright (c) 2015
// </copyright>
// <summary>
//   The null to visibility converter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Leaderboard
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Converter for Null to Visibility
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether the interaction reversed. When false, Null will convert to Visible.
        /// When true, null will convert to Collapsed.
        /// </summary>
        public bool IsReversed { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Converts the value
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The converted <see cref="object"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool val = value == null;
            if (this.IsReversed)
            {
                val = !val;
            }

            if (val)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        /// <summary>
        /// The convert back. Not implemented.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="targetType">
        /// The target type.
        /// </param>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <param name="culture">
        /// The culture.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="NotImplementedException">
        /// This method is not implemented
        /// </exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}