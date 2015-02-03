namespace WindowsPhoneDriver.OuterDriver.CommandExecutors
{
    using System;
    using System.Globalization;
    using System.Windows.Forms;

    using WindowsPhoneDriver.Common;
    using WindowsPhoneDriver.Common.Exceptions;

    internal class ExecuteScriptExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            const string MobileScriptPrefix = "mobile:";
            var script = this.ExecutedCommand.Parameters["script"].ToString();
            if (!script.StartsWith(MobileScriptPrefix, StringComparison.OrdinalIgnoreCase))
            {
                throw new NotImplementedException(
                    "execute partially implemented, supports only mobile: prefixed commands");
            }

            var command = script.Split(':')[1].ToLower(CultureInfo.InvariantCulture).Trim();

            if (command.Equals("start"))
            {
                this.Automator.EmulatorController.TypeKey(Keys.F2);
            }
            else if (command.Equals("search"))
            {
                this.Automator.EmulatorController.TypeKey(Keys.F3);
            }
            else
            {
                throw new AutomationException(
                    "Unknown 'mobile:' script command. See https://github.com/2gis/winphonedriver/wiki/Command-Execute-Script for supported commands.", 
                    ResponseStatus.JavaScriptError);
            }

            return null;
        }

        #endregion
    }
}
