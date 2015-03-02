namespace WindowsUniversalAppDriver.CommandExecutors
{
    using WindowsUniversalAppDriver.Common;

    internal class ScreenshotExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var base64Screenshot = this.Automator.EmulatorController.TakeScreenshot();
            return this.JsonResponse(ResponseStatus.Success, base64Screenshot);
        }

        #endregion
    }
}
