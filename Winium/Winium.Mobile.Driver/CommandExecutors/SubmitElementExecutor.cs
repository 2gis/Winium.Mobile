namespace Winium.Mobile.Driver.CommandExecutors
{
    #region

    using System.Windows.Forms;

    #endregion

    internal class SubmitElementExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            ClickElementExecutor.ClickElement(this.Automator, this.ExecutedCommand.Parameters["ID"].ToString());
            this.Automator.EmulatorController.TypeKey(Keys.Enter);

            return this.JsonResponse();
        }

        #endregion
    }
}
