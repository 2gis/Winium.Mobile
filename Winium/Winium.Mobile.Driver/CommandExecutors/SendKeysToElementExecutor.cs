namespace Winium.Mobile.Driver.CommandExecutors
{
    #region

    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Winium.Mobile.Common;
    using Winium.Mobile.Driver.CommandHelpers;

    #endregion

    internal class SendKeysToElementExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            /* TODO does not complie with standard and does not allow to type magic keys between other characters, only at the end
             * if the text has the magic keys in it, type them after sending the rest of the text to the inner server
             * It is not recommended to use SendKeys for simulation of hardware buttons, use GoBack command or ExecuteScript with 'mobile:' prefixed commands.
             * hardware buttons:
             * F1 - back
             * F2 - start/windows
             * F3 - search
             */
            var value = this.ExecutedCommand.Parameters["value"].ToObject<string[]>();

            var foundMagicKeys =
                (from key in value
                 where SendKeysHelper.SpecialKeys.ContainsKey(key)
                 select SendKeysHelper.SpecialKeys[key]).ToList();
            if (foundMagicKeys.Any())
            {
                value = value.Where(val => SendKeysHelper.SpecialKeys.ContainsKey(val) == false).ToArray();
            }

            this.ExecutedCommand.Parameters["value"] = JToken.FromObject(value);

            // TODO Use TypeKey for text instead of InnerDrive magic
            // TODO check if response status = success, throw if not
            var responseBody = this.Automator.CommandForwarder.ForwardCommand(this.ExecutedCommand);
            var response = JsonConvert.DeserializeObject<JsonResponse>(responseBody);
            if (response.Status != ResponseStatus.Success)
            {
                return responseBody;
            }

            foreach (var magicKey in foundMagicKeys)
            {
                this.Automator.EmulatorController.TypeKey(magicKey);
            }

            return this.JsonResponse();
        }

        #endregion
    }
}
