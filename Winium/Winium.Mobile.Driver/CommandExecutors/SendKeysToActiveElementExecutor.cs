namespace Winium.Mobile.Driver.CommandExecutors
{
    #region

    using System.Linq;

    using Winium.Mobile.Driver.CommandHelpers;

    #endregion

    internal class SendKeysToActiveElementExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            // TODO support only limited set of special keys (see SendKeysHelper.SpecialKeys)
            var value = this.ExecutedCommand.Parameters["value"].ToObject<string[]>();

            foreach (var s in value.Where(s => SendKeysHelper.SpecialKeys.ContainsKey(s)))
            {
                this.Automator.EmulatorController.TypeKey(SendKeysHelper.SpecialKeys[s]);
            }

            return this.JsonResponse();
        }

        #endregion
    }
}
