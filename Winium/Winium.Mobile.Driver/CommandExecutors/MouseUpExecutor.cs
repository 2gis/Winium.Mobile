namespace Winium.Mobile.Driver.CommandExecutors
{
    internal class MouseUpExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            this.Automator.EmulatorController.LeftButtonUp();

            return this.JsonResponse();
        }

        #endregion
    }
}
