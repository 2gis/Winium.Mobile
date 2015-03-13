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
        /// Переводит координаты точки с логических в реальные.
        /// </summary>
        /// <param name="point">
        /// Точка относительно логической сетки.
        /// </param>
        /// <returns>
        /// Точка относительно реального разрешения устройства.
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
        /// Переводит координаты точки с реальных в логические.
        /// </summary>
        /// <param name="point">
        /// Точка относительно реального разрешения устройства.
        /// </param>
        /// <returns>
        /// Точка относительно логической сетки.
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
