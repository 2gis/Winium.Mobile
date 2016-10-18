namespace Winium.Mobile.Driver.CommandExecutors
{
    #region

    using System;
    using System.Drawing;
    using System.Globalization;

    using Winium.Mobile.Driver.Automator;

    #endregion

    internal class MouseMoveToExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var elementId = Automator.GetValue<string>(this.ExecutedCommand.Parameters, "element");
            Point coordinates;
            if (elementId != null)
            {
                coordinates = this.Automator.RequestElementLocation(elementId).GetValueOrDefault();
            }
            else
            {
                var xOffset = Convert.ToInt32(this.ExecutedCommand.Parameters["xoffset"], CultureInfo.InvariantCulture);
                var yOffset = Convert.ToInt32(this.ExecutedCommand.Parameters["yoffset"], CultureInfo.InvariantCulture);
                coordinates = new Point(xOffset, yOffset);
            }

            this.Automator.EmulatorController.MoveCursorTo(coordinates);

            return this.JsonResponse();
        }

        #endregion
    }
}
