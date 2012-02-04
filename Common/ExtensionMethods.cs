/*
Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Common.ExtensionMethods
{
    /// <summary>
    /// Collection of Common Extention Methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Gets a DateTime representing the start of the month.
        /// </summary>
        /// <param name="dt">The DateTime with the month to get the start of.</param>
        /// <returns>A DateTime object set to the start of the month.</returns>
        public static DateTime StartOfMonth(this DateTime dt)
        {
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        }

        /// <summary>
        /// Gets a DateTime representing the emd of the month.
        /// </summary>
        /// <param name="dt">The DateTime with the month to get the end of.</param>
        /// <returns>A DateTime object set to the end of the month.</returns>
        public static DateTime EndOfMonth(this DateTime dt)
        {
            dt = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 999);
            return dt.AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// Gets a DateTime representing the start of the current Quarter.
        /// </summary>
        /// <param name="dt">The DateTime to get the start of current Quarter.</param>
        /// <returns>A DateTime object set to the start of the current quarter</returns>
        public static DateTime StartOfQuarter(this DateTime dt)
        {
            switch (dt.Month)
            {
                case 1:
                case 2:
                case 3:
                    dt = new DateTime(dt.Year, 1, 1);
                    break;

                case 4:
                case 5:
                case 6:
                    dt = new DateTime(dt.Year, 4, 1);
                    break;

                case 7:
                case 8:
                case 9:
                    dt = new DateTime(dt.Year, 7, 1);
                    break;

                case 10:
                case 11:
                case 12:
                    dt = new DateTime(dt.Year, 10, 1);
                    break;

                default:
                    break;
            }

            return dt;
        }

        /// <summary>
        /// Gets a DateTime representing the end of the current Quarter.
        /// </summary>
        /// <param name="dt">The DateTime to get the end of current Quarter.</param>
        /// <returns>A DateTime object set to the end of the current quarter</returns>
        public static DateTime EndOfQuarter(this DateTime dt)
        {
            dt = dt.StartOfQuarter().AddMonths(4).AddDays(-1);
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59, 999);
        }
    }
}
