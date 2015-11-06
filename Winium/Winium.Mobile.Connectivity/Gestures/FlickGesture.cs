namespace Winium.Mobile.Connectivity.Gestures
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Drawing;

    #endregion

    public class FlickGesture : IGesture
    {
        #region Constants

        private const int NumberOfIntermediatePoints = 4;

        #endregion

        #region Constructors and Destructors

        public FlickGesture(Point startPoint, int xOffset, int yOffset, double speed)
        {
            this.StartPosition = startPoint;
            this.PeriodBetweenPoints =
                TimeSpan.FromMilliseconds(this.Distance() / (NumberOfIntermediatePoints + 1) / speed);

            this.EndPosition = new Point(startPoint.X + xOffset, startPoint.Y + yOffset);
        }

        public FlickGesture(Point startPoint, double xSpeed, double ySpeed)
        {
            this.StartPosition = startPoint;
            this.PeriodBetweenPoints = TimeSpan.FromMilliseconds(25);

            var time = this.PeriodBetweenPoints.TotalSeconds * (NumberOfIntermediatePoints + 1);

            var xOffset = xSpeed * time;
            var yOffset = ySpeed * time;

            this.EndPosition = new Point(startPoint.X + (int)xOffset, startPoint.Y + (int)yOffset);
        }

        #endregion

        #region Public Properties

        public Point EndPosition { get; set; }

        public TimeSpan PeriodBetweenPoints { get; set; }

        public Point StartPosition { get; set; }

        #endregion

        #region Public Methods and Operators

        public IEnumerable<Point> GetScreenPoints()
        {
            var list = new List<Point> { this.StartPosition };

            for (var i = 0; i < NumberOfIntermediatePoints; i++)
            {
                list.Add(this.GenerateIntermediatePoint(i));
            }

            list.Add(this.EndPosition);

            return list;
        }

        #endregion

        #region Methods

        private double Distance()
        {
            return
                Math.Sqrt(
                    Math.Pow(this.StartPosition.X - this.EndPosition.X, 2)
                    + Math.Pow(this.StartPosition.Y - this.EndPosition.Y, 2));
        }

        private Point GenerateIntermediatePoint(int zeroBasedIndex)
        {
            var ratio = (zeroBasedIndex + 1.0) / (NumberOfIntermediatePoints + 1.0);

            return new Point
                       {
                           X = this.StartPosition.X + (int)(ratio * (this.EndPosition.X - this.StartPosition.X)), 
                           Y = this.StartPosition.Y + (int)(ratio * (this.EndPosition.Y - this.StartPosition.Y))
                       };
        }

        #endregion
    }
}
