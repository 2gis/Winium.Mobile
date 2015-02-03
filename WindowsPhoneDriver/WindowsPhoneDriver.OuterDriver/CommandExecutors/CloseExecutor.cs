namespace WindowsPhoneDriver.OuterDriver.CommandExecutors
{
    internal class CloseExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            this.Automator.Deployer.Disconnect();

            return null;
        }

        #endregion
    }
}
