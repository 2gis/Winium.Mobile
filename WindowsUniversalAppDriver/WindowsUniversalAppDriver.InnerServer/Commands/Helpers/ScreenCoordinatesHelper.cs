namespace WindowsUniversalAppDriver.InnerServer.Commands.Helpers
{
    using System;
    using System.Reflection;

    using Windows.Foundation;
    using Windows.Graphics.Display;

    public static class ScreenCoordinatesHelper
    {
        private static readonly Lazy<double> CurrentRawPixelsPerViewPixelField =
            new Lazy<double>(() => GetRawPixelsPerViewPixel(DisplayInformation.GetForCurrentView()));

        public static double CurrentRawPixelsPerViewPixel 
        {
            get
            {
                return CurrentRawPixelsPerViewPixelField.Value;
            }
        }

        /// <summary>
        /// Переводит координаты точки с реальных в логические.
        /// </summary>
        /// <param name="point">Точка относительно реального разрешения устройства.</param>
        /// <returns>Точка относительно логической сетки.</returns>
        public static Point ScreenPointToLogicalPoint(Point point)
        {
            var x = point.X;
            var y = point.Y;
            var scaleFactor = CurrentRawPixelsPerViewPixel;

            point = new Point(
                Math.Floor((x / scaleFactor) + 0.5),
                Math.Floor((y / scaleFactor) + 0.5));

            return point;
        }

        /// <summary>
        /// Переводит координаты точки с логических в реальные.
        /// </summary>
        /// <param name="point">Точка относительно логической сетки.</param>
        /// <returns>Точка относительно реального разрешения устройства.</returns>
        public static Point LogicalPointToScreenPoint(Point point)
        {
            var x = point.X;
            var y = point.Y;
            var scaleFactor = CurrentRawPixelsPerViewPixel;

            point = new Point(
                Math.Floor((x * scaleFactor) + 0.5),
                Math.Floor((y * scaleFactor) + 0.5));

            return point;
        }

        public static double GetRawPixelsPerViewPixel(DisplayInformation displayInformation)
        {         
            var info = displayInformation
                .GetType()
                .GetTypeInfo()
                .GetDeclaredProperty("RawPixelsPerViewPixel");

            return (double)info.GetValue(displayInformation);
        }
    }
}