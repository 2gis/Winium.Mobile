namespace Winium.Mobile.Driver.CommandExecutors
{
    #region

    using System.Windows.Forms;

    #endregion

    internal class GoBackExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            // F1 is shortcut for "Back" hardware button
            this.Automator.EmulatorController.TypeKey(Keys.F1);

            return this.JsonResponse();
        }

        #endregion
    }
}
