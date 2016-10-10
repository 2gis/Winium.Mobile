namespace Winium.Mobile.Driver.CommandExecutors
{
    #region

    using System;
    using System.Drawing;
    using System.Globalization;

    using Winium.Mobile.Connectivity.Gestures;
    using Winium.Mobile.Driver.Automator;

    #endregion

    internal class TouchFlickExecutor : CommandExecutorBase
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

            if (this.ExecutedCommand.Parameters.ContainsKey("speed"))
            {
                var xOffset = Convert.ToInt32(this.ExecutedCommand.Parameters["xoffset"], CultureInfo.InvariantCulture);
                var yOffset = Convert.ToInt32(this.ExecutedCommand.Parameters["yoffset"], CultureInfo.InvariantCulture);
                var speed = Convert.ToDouble(this.ExecutedCommand.Parameters["speed"], CultureInfo.InvariantCulture);

                this.Automator.EmulatorController.PerformGesture(new FlickGesture(startPoint, xOffset, yOffset, speed));
            }
            else
            {
                var xSpeed = Convert.ToDouble(this.ExecutedCommand.Parameters["xspeed"], CultureInfo.InvariantCulture);
                var ySpeed = Convert.ToDouble(this.ExecutedCommand.Parameters["yspeed"], CultureInfo.InvariantCulture);
                this.Automator.EmulatorController.PerformGesture(new FlickGesture(startPoint, xSpeed, ySpeed));
            }

            return this.JsonResponse();
        }

        #endregion
    }
}
