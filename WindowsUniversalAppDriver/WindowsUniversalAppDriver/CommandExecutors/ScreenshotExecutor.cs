namespace WindowsUniversalAppDriver.CommandExecutors
{
    internal class ScreenshotExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            return this.Automator.EmulatorController.TakeScreenshot();
        }

        #endregion
    }
}
