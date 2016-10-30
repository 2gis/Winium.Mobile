namespace Winium.Mobile.Driver.CommandExecutors
{
    #region

    using Newtonsoft.Json;

    using Common;

    #endregion

    internal class SetContextExecutor : CommandExecutorBase
    {
        protected override string DoImpl()
        {
            var rv = this.Automator.CommandForwarder.ForwardCommand(this.ExecutedCommand);
            var response = JsonConvert.DeserializeObject<JsonResponse>(rv);

            if (response.Status == ResponseStatus.Success)
            {
                this.Automator.CurrentContext = response.Value as string;
            }

            return rv;
        }
    }
}
