namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using Windows.Graphics.Display;

    using Winium.Mobile.Common;

    #endregion

    internal class OrientationCommand : CommandBase
    {
        #region Public Methods and Operators

        protected override string DoImpl()
        {
            var orientation = DisplayOrientations.Portrait;

            /*var frame = WiniumVirtualRoot.Current.VisualRoot.Element as PhoneApplicationFrame;
            if (frame != null)
            {
                orientation = frame.Orientation;
            }*/
            return this.JsonResponse(ResponseStatus.Success, orientation.ToString());
        }

        #endregion
    }
}
