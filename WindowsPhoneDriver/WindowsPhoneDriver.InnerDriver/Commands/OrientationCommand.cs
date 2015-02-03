namespace WindowsPhoneDriver.InnerDriver.Commands
{
    using Microsoft.Phone.Controls;

    using WindowsPhoneDriver.Common;

    internal class OrientationCommand : CommandBase
    {
        #region Public Methods and Operators

        public override string DoImpl()
        {
            var orientation = PageOrientation.Portrait;
            var frame = this.Automator.VisualRoot as PhoneApplicationFrame;
            if (frame != null)
            {
                orientation = frame.Orientation;
            }

            return this.JsonResponse(ResponseStatus.Success, orientation.ToString());
        }

        #endregion
    }
}
