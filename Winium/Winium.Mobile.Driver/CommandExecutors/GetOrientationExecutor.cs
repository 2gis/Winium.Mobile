namespace Winium.Mobile.Driver.CommandExecutors
{
    #region

    using Winium.Mobile.Connectivity.Emulator;
    using Winium.Mobile.Common;

    #endregion

    internal class GetOrientationExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var orientation = this.Automator.EmulatorController.Orientation;

            var orientationName = (orientation == EmulatorController.PhoneOrientation.Portrait)
                                      ? "PORTRAIT"
                                      : "LANDSCAPE";

            return this.JsonResponse(ResponseStatus.Success, orientationName);
        }

        #endregion
    }
}
