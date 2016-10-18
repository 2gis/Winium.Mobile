namespace Winium.Mobile.Driver.CommandExecutors
{
    internal class MouseClickExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            this.Automator.EmulatorController.LeftButtonClick();

            return this.JsonResponse();
        }

        #endregion
    }
}
