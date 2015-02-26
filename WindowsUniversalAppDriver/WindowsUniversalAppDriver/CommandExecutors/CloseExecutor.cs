namespace WindowsUniversalAppDriver.CommandExecutors
{
    internal class CloseExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            this.Automator.Deployer.Uninstall();

            return this.JsonResponse();
        }

        #endregion
    }
}
