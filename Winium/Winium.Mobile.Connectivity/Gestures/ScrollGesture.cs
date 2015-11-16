namespace Winium.Mobile.Connectivity.Gestures
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Drawing;

    #endregion

    public class ScrollGesture : IGesture
    {
        #region Constructors and Destructors

        public ScrollGesture(Point startPoint, int xOffset, int yOffset, TimeSpan tapDuration = default(TimeSpan))
        {
            this.StartPosition = startPoint;
            this.EndPosition = new Point(startPoint.X + xOffset, startPoint.Y + yOffset);
            this.PeriodBetweenPoints = tapDuration;
            this.NumberOfIntermediatePoints = 5;
            if (this.PeriodBetweenPoints == default(TimeSpan))
            {
                this.PeriodBetweenPoints = TimeSpan.FromMilliseconds(50);
            }
        }

        #endregion

        #region Public Properties

        public Point EndPosition { get; set; }

        public int NumberOfIntermediatePoints { get; set; }

        public TimeSpan PeriodBetweenPoints { get; set; }

        public Point StartPosition { get; set; }

        #endregion

        #region Public Methods and Operators

        public IEnumerable<Point> GetScreenPoints()
        {
            var list = new List<Point> { this.StartPosition };

            for (var i = 0; i < this.NumberOfIntermediatePoints; i++)
            {
                list.Add(this.GenerateIntermediatePoint(i));
            }

            list.Add(this.EndPosition);
            list.Add(this.EndPosition);
            return list;
        }

        #endregion

        #region Methods

        private Point GenerateIntermediatePoint(int zeroBasedIndex)
        {
            var ratio = (zeroBasedIndex + 1.0) / (this.NumberOfIntermediatePoints + 1.0);

            return new Point
                       {
                           X = this.StartPosition.X + (int)(ratio * (this.EndPosition.X - this.StartPosition.X)), 
                           Y = this.StartPosition.Y + (int)(ratio * (this.EndPosition.Y - this.StartPosition.Y))
                       };
        }

        #endregion
    }
}
