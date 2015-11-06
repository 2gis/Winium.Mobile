namespace Winium.StoreApps.Driver.CommandExecutors
{
    #region

    using System;
    using System.Drawing;
    using System.Globalization;

    using Winium.StoreApps.Driver.Automator;

    #endregion

    internal class MouseMoveToExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var elementId = Automator.GetValue<string>(this.ExecutedCommand.Parameters, "element");
            var xOffsetParam = this.ExecutedCommand.Parameters["xoffset"];
            var yOffsetParam = this.ExecutedCommand.Parameters["yoffset"];

            Point coordinates = new Point();
            if (xOffsetParam != null && yOffsetParam != null)
            {
                var xOffset = Convert.ToInt32(xOffsetParam, CultureInfo.InvariantCulture);
                var yOffset = Convert.ToInt32(yOffsetParam, CultureInfo.InvariantCulture);
                coordinates = new Point(xOffset, yOffset);

                if (elementId != null)
                {
                    var elementRect = Automator.RequestElementRect(elementId).GetValueOrDefault();
                    coordinates.X += elementRect.X;
                    coordinates.Y += elementRect.Y;
                }
            }
            else if (elementId != null)
            {
                coordinates = this.Automator.RequestElementLocation(elementId).GetValueOrDefault();
            }

            this.Automator.EmulatorController.MoveCursorTo(coordinates);

            return this.JsonResponse();
        }

        #endregion
    }
}
