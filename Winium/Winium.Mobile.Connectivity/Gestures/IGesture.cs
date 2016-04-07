namespace Winium.Mobile.Connectivity.Gestures
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Drawing;

    #endregion

    public interface IGesture
    {
        #region Public Properties

        TimeSpan PeriodBetweenPoints { get; set; }

        #endregion

        #region Public Methods and Operators

        IEnumerable<Point> GetScreenPoints();

        #endregion
    }
}
