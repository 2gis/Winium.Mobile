namespace Winium.Mobile.Driver.CommandExecutors
{
    #region

    using Newtonsoft.Json.Linq;

    using Winium.Mobile.Common.CommandSettings;

    #endregion

    internal class GetElementAttributeExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var settings = JToken.FromObject(this.Automator.ActualCapabilities.CommandSettings.ElementAttributeSettings);
            this.ExecutedCommand.Parameters.Add(CommandSettings.ElementAttributeSettingsParameter, settings);
            var response = this.Automator.CommandForwarder.ForwardCommand(this.ExecutedCommand);

            return response;
        }

        #endregion
    }
}
