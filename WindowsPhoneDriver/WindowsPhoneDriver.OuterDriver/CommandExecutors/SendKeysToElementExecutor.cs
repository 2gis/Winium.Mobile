namespace WindowsPhoneDriver.OuterDriver.CommandExecutors
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    internal class SendKeysToElementExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            /* TODO does not complie with standard and does not allow to type magic keys between other characters, only at the end
             * if the text has the magic keys in it, type them after sending the rest of the text to the inner driver
             * It is not recommended to use SendKeys for simulation of hardware buttons, use GoBack command or ExecuteScript with 'mobile:' prefixed commands.
             * hardware buttons:
             * F1 - back 
             * F2 - start/windows
             * F3 - search
             */
            var magicKeys = new Dictionary<string, Keys>
                                {
                                    { "\ue007", Keys.Enter }, 
                                    { "\ue031", Keys.F1 }, 
                                    { "\ue032", Keys.F2 }, 
                                    { "\ue033", Keys.F3 }
                                };
            var originalContent = this.ExecutedCommand.Parameters;
            var value = ((object[])originalContent["value"]).Select(o => o.ToString()).ToArray();

            var foundMagicKeys = (from key in value where magicKeys.ContainsKey(key) select magicKeys[key]).ToList();

            value = value.Where(val => magicKeys.ContainsKey(val) == false).ToArray();

            this.ExecutedCommand.Parameters["value"] = value;

            // TODO check if response status = success, throw if not
            var responseBody = this.Automator.CommandForwarder.ForwardCommand(this.ExecutedCommand);

            foreach (var magicKey in foundMagicKeys)
            {
                this.Automator.EmulatorController.TypeKey(magicKey);
            }

            return null;
        }

        #endregion
    }
}
