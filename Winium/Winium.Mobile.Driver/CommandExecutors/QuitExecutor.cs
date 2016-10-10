namespace Winium.Mobile.Driver.CommandExecutors
{
    internal class QuitExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            if (!this.Automator.ActualCapabilities.DebugConnectToRunningApp)
            {
                // TODO quit should close all open windows (apps) and possible close the emulator
                this.Automator.EmulatorController.Disconnect();
                this.Automator.Deployer.Uninstall();
            }

            return this.JsonResponse();
        }

        #endregion
    }
}
