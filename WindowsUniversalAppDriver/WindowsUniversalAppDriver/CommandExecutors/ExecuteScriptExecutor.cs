namespace WindowsUniversalAppDriver.CommandExecutors
{
    #region using

    using WindowsUniversalAppDriver.CommandExecutors.Scripts;
    using WindowsUniversalAppDriver.Common;
    using WindowsUniversalAppDriver.Common.Exceptions;

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
                prefix = script.Substring(0, index);
                command = script.Substring(index).Trim();
            }

            IScript scriptExecutor;
            switch (prefix)
            {
                case "mobile:":
                    scriptExecutor = new MobileScript(this.Automator.EmulatorController);
                    break;
                default:
                    const string url = "https://github.com/2gis/winphonedriver/wiki/Command-Execute-Script";
                    var msg = string.Format("Unknown script prefix '{0}'. See {1} for supported scripts.", prefix, url);
                    throw new AutomationException(msg, ResponseStatus.JavaScriptError);
            }

            scriptExecutor.Execute(command);
            return null;
        }

        #endregion
    }
}
