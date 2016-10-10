namespace Winium.Mobile.Driver.CommandExecutors
{
    #region

    using System;
    using System.Drawing;
    using System.Globalization;

    using Winium.Mobile.Connectivity.Gestures;
    using Winium.Mobile.Driver.Automator;

    #endregion

    internal class TouchScrollExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var screen = this.Automator.EmulatorController.PhoneScreenSize;
            var startPoint = new Point(screen.Width / 2, screen.Height / 2);

            var elementId = Automator.GetValue<string>(this.ExecutedCommand.Parameters, "element");
            if (elementId != null)
            {
                startPoint = this.Automator.RequestElementLocation(elementId).GetValueOrDefault();
            }

            // TODO: Add handling of missing parameters. Server should respond with a 400 Bad Request if parameters are missing
            var xOffset = Convert.ToInt32(this.ExecutedCommand.Parameters["xoffset"], CultureInfo.InvariantCulture);
            var yOffset = Convert.ToInt32(this.ExecutedCommand.Parameters["yoffset"], CultureInfo.InvariantCulture);

            this.Automator.EmulatorController.PerformGesture(new ScrollGesture(startPoint, xOffset, yOffset));

            return this.JsonResponse();
        }

        #endregion
    }
}
