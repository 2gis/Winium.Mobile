namespace Winium.Mobile.Driver.CommandExecutors
{
    internal class CloseExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            if (!this.Automator.ActualCapabilities.DebugConnectToRunningApp)
            {
                // TODO close should only close app, not uninstall
                this.Automator.EmulatorController.Disconnect();
                this.Automator.Deployer.Uninstall();
            }

            return this.JsonResponse();
        }

        #endregion
    }
}
