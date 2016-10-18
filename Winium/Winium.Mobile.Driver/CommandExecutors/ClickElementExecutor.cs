namespace Winium.Mobile.Driver.CommandExecutors
{
    #region

    using Winium.Mobile.Driver.Automator;

    #endregion

    internal class ClickElementExecutor : CommandExecutorBase
    {
        #region Methods

        internal static bool ClickElement(Automator automator, string elementId)
        {
            var location = automator.RequestElementLocation(elementId);

            if (!location.HasValue)
            {
                // TODO return bad parameters?
                return false;
            }

            automator.EmulatorController.LeftButtonClick(location.Value);

            return true;
        }

        protected override string DoImpl()
        {
            ClickElement(this.Automator, this.ExecutedCommand.Parameters["ID"].ToString());

            return this.JsonResponse();
        }

        #endregion
    }
}
