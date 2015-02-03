namespace WindowsPhoneDriver.OuterDriver.CommandExecutors
{
    internal class MouseClickExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            this.Automator.EmulatorController.LeftButtonClick();

            return null;
        }

        #endregion
    }
}
