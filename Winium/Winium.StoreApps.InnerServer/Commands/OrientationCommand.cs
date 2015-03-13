namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using Windows.Graphics.Display;

    using Winium.StoreApps.Common;

    #endregion

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
