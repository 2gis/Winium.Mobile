namespace WindowsPhoneDriver.OuterDriver.CommandExecutors
{
    using System.Windows.Forms;

    internal class GoBackExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            // F1 is shortcut for "Back" hardware button
            this.Automator.EmulatorController.TypeKey(Keys.F1);

            return null;
        }

        #endregion
    }
}
