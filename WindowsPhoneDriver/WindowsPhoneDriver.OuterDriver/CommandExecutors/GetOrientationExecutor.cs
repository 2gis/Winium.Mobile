namespace WindowsPhoneDriver.OuterDriver.CommandExecutors
{
    using System;

    using Newtonsoft.Json;

    using WindowsPhoneDriver.Common;

    internal class GetOrientationExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var orientation = string.Empty;
            var responseBody = this.Automator.CommandForwarder.ForwardCommand(this.ExecutedCommand);
            var deserializeObject = JsonConvert.DeserializeObject<JsonResponse>(responseBody);
            if (deserializeObject.Status == ResponseStatus.Success)
            {
                var value = deserializeObject.Value.ToString();
                orientation = value.StartsWith("landscape", StringComparison.OrdinalIgnoreCase) ? "LANDSCAPE" : "PORTRAIT";
            }

            return this.JsonResponse(ResponseStatus.Success, orientation);
        }

        #endregion
    }
}
