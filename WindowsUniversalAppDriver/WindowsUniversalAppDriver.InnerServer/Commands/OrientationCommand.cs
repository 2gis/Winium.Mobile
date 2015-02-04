namespace WindowsUniversalAppDriver.InnerServer.Commands
{
    using Windows.Graphics.Display;

    using WindowsUniversalAppDriver.Common;

    internal class OrientationCommand : CommandBase
    {
        #region Public Methods and Operators

        public override string DoImpl()
        {
            var orientation = DisplayOrientations.Portrait;
            /*var frame = this.Automator.VisualRoot as PhoneApplicationFrame;
            if (frame != null)
            {
                orientation = frame.Orientation;
            }*/

            return this.JsonResponse(ResponseStatus.Success, orientation.ToString());
        }

        #endregion
    }
}
