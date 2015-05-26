namespace Winium.StoreApps.InnerServer.Commands.Helpers
{
    #region

    using System;
    using System.Reflection;

    using Windows.Foundation;
    using Windows.Graphics.Display;

    #endregion

    public static class ScreenCoordinatesHelper
    {
        #region Static Fields

        private static readonly Lazy<double> CurrentRawPixelsPerViewPixelField =
            new Lazy<double>(() => GetRawPixelsPerViewPixel(DisplayInformation.GetForCurrentView()));

        #endregion

        #region Public Properties

        public static double CurrentRawPixelsPerViewPixel
        {
            get
            {
                return CurrentRawPixelsPerViewPixelField.Value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public static double GetRawPixelsPerViewPixel(DisplayInformation displayInformation)
        {
            var info = displayInformation.GetType().GetTypeInfo().GetDeclaredProperty("RawPixelsPerViewPixel");

            return (double)info.GetValue(displayInformation);
        }

        /// <summary>
        /// Translate logical coordinates to real coordinates.
        /// </summary>
        /// <param name="point">
        /// Point in logical coordinates.
        /// </param>
        /// <returns>
        /// Point in real coordinates.
        /// </returns>
        public static Point LogicalPointToScreenPoint(Point point)
        {
            var x = point.X;
            var y = point.Y;
            var scaleFactor = CurrentRawPixelsPerViewPixel;

            point = new Point(Math.Floor((x * scaleFactor) + 0.5), Math.Floor((y * scaleFactor) + 0.5));

            return point;
        }

        /// <summary>
        /// Translate real coordinates to logical coordinates.
        /// </summary>
        /// <param name="point">
        /// Point in real coordinates.
        /// </param>
        /// <returns>
        /// Point in logical coordinates.
        /// </returns>
        public static Point ScreenPointToLogicalPoint(Point point)
        {
            var x = point.X;
            var y = point.Y;
            var scaleFactor = CurrentRawPixelsPerViewPixel;

            point = new Point(Math.Floor((x / scaleFactor) + 0.5), Math.Floor((y / scaleFactor) + 0.5));

            return point;
        }

        #endregion
    }
}
