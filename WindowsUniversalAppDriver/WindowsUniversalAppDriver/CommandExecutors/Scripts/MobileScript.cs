namespace WindowsUniversalAppDriver.CommandExecutors.Scripts
{
    #region using

    using System.Windows.Forms;

    using WindowsUniversalAppDriver.Common;
    using WindowsUniversalAppDriver.Common.Exceptions;
    using WindowsUniversalAppDriver.EmulatorHelpers;

    #endregion

    internal class MobileScript : IScript
    {
        private readonly EmulatorController emulatorController;

        internal MobileScript(EmulatorController controller)
        {
            emulatorController = controller;
        }

        public void Execute(string command)
        {
            switch (command)
            {
                case "start":
                    this.emulatorController.TypeKey(Keys.F2);
                    break;
                case "search":
                    this.emulatorController.TypeKey(Keys.F3);
                    break;
                default:
                    const string url = "https://github.com/2gis/winphonedriver/wiki/Command-Execute-Script";
                    var msg = string.Format("Unknown 'mobile:' script command '{0}'. See {1} for supported commands.",
                                            command ?? string.Empty, url);
                    throw new AutomationException(msg, ResponseStatus.JavaScriptError);
            }
        }
    }
}
