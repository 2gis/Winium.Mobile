namespace WindowsPhoneDriver.OuterDriver.CommandExecutors
{
    internal class MouseUpExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            this.Automator.EmulatorController.LeftButtonUp();

            return null;
        }

        #endregion
    }
}
