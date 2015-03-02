namespace WindowsUniversalAppDriver.CommandExecutors
{
    internal class MouseDownExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            this.Automator.EmulatorController.LeftButtonDown();

            return this.JsonResponse();
        }

        #endregion
    }
}
