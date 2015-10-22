namespace Winium.StoreApps.Driver.CommandExecutors
{
    #region

    using Winium.StoreApps.Driver.Automator;
    using System;

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
            try {
                return this.Automator.CommandForwarder.ForwardCommand(this.ExecutedCommand);
            } catch (Exception e) {
                ClickElement(this.Automator, this.ExecutedCommand.Parameters["ID"].ToString());
                return this.JsonResponse();
            }
        }

        #endregion
    }
}
