namespace WindowsUniversalAppDriver.CommandExecutors
{
    #region using

    using System.Windows.Forms;

    using WindowsUniversalAppDriver.Common;
    using WindowsUniversalAppDriver.Common.Exceptions;

    using Newtonsoft.Json;

    #endregion

    internal class ExecuteScriptExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            string command;
            var prefix = string.Empty;

            var script = this.ExecutedCommand.Parameters["script"].ToString();
            var index = script.IndexOf(':');
            if (index == -1)
            {
                command = script;
            }
            else
            {
                ++index;
                prefix = script.Substring(0, index);
                command = script.Substring(index).Trim();
            }

            switch (prefix)
            {
                case "mobile:":
                    this.ExecuteMobileScript(command);
                    break;
                default:
                    this.ForwardCommand();
                    break;
            }

            return null;
        }
        
        internal void ExecuteMobileScript(string command)
        {
            switch (command)
            {
                case "start":
                    this.Automator.EmulatorController.TypeKey(Keys.F2);
                    break;
                case "search":
                    this.Automator.EmulatorController.TypeKey(Keys.F3);
                    break;
                default:
                    const string url =
                        "https://github.com/2gis/windows-universal-app-driver/wiki/Command-Execute-Script#press-hardware-button";
                    var msg = string.Format("Unknown 'mobile:' script command '{0}'. See {1} for supported commands.",
                                            command ?? string.Empty, url);
                    throw new AutomationException(msg, ResponseStatus.JavaScriptError);
            }
        }

        internal void ForwardCommand()
        {
            var responseBody = this.Automator.CommandForwarder.ForwardCommand(this.ExecutedCommand);
            var deserializeObject = JsonConvert.DeserializeObject<JsonResponse>(responseBody);
            if (deserializeObject.Status != ResponseStatus.Success)
            {
                throw new AutomationException(deserializeObject.Value.ToString(), ResponseStatus.JavaScriptError);
            }
        }

        #endregion
    }
}
