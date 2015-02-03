namespace WindowsPhoneDriver.OuterDriver.CommandExecutors
{
    using System;
    using System.Drawing;
    using System.Globalization;

    using WindowsPhoneDriver.OuterDriver.Automator;

    internal class MouseMoveToExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            this.Automator.UpdatedOrientationForEmulatorController();

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

            this.Automator.UpdatedOrientationForEmulatorController();
            this.Automator.EmulatorController.MoveCursorTo(coordinates);

            return null;
        }

        #endregion
    }
}
