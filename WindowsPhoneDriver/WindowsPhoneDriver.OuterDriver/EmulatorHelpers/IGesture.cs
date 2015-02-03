namespace WindowsPhoneDriver.OuterDriver.EmulatorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    internal interface IGesture
    {
        #region Public Properties

        TimeSpan PeriodBetweenPoints { get; set; }

        #endregion

        #region Public Methods and Operators

        IEnumerable<Point> GetScreenPoints();

        #endregion
    }
}
