namespace WindowsUniversalAppDriver.InnerServer.Commands
{
    #region using

    using Newtonsoft.Json.Linq;

    using Windows.UI.Xaml.Automation.Peers;
    using Windows.UI.Xaml.Automation.Provider;

    using WindowsUniversalAppDriver.Common;
    using WindowsUniversalAppDriver.Common.Exceptions;
    using WindowsUniversalAppDriver.InnerServer.Commands.Helpers;

    #endregion

    internal class ExecuteCommand : CommandBase
    {
        #region Constants

        internal const string HelpUnknownScriptMsg =
            "Unknown script command '{0} {1}'. See {2} for supported commands.";

        internal const string HelpUrlAttributeScript =
            "https://github.com/2gis/windows-universal-app-driver/wiki/Command-Execute-Script#set-property-on-element";

        internal const string HelpUrlAutomationScript =
            "https://github.com/2gis/windows-universal-app-driver/wiki/Command-Execute-Script#use-automationpeerspatterninterface-on-element";

        internal const string HelpUrlScript =
            "https://github.com/2gis/windows-universal-app-driver/wiki/Command-Execute-Script#supported-scripts";

        #endregion

        #region Public Methods and Operators

        public override string DoImpl()
        {
            string command;
            var prefix = string.Empty;

            var script = this.Parameters["script"].ToString();
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
                case "automation:":
                    this.ExecuteAutomationScript(command);
                    break;
                case "attribute:":
                    this.ExecuteAttributeScript(command);
                    break;
                default:
                    var msg = string.Format(HelpUnknownScriptMsg, prefix, command, HelpUrlScript);
                    throw new AutomationException(msg, ResponseStatus.JavaScriptError);
            }

            return this.JsonResponse(ResponseStatus.Success, string.Empty);
        }

        #endregion

        #region Methods

        private void ExecuteAttributeScript(string command)
        {
            if (command != "set")
            {
                var msg = string.Format(HelpUnknownScriptMsg, "attribute:", command, HelpUrlAttributeScript);
                throw new AutomationException(msg, ResponseStatus.JavaScriptError);
            }

            /* 'attribute: set' is used to set property value on element
             * script parameters:
             *      element - WebElement on wich attribute will be set
             *      attribute name - property to be set, nested property can be set using dot syntax
             *      value - value to be set
             */
            var args = (JArray)this.Parameters["args"];

            var elementId = args[0]["ELEMENT"].ToString();
            var element = this.Automator.WebElements.GetRegisteredElement(elementId);

            var attributeName = args[1].ToString();
            var value = args[2];

            element.SetAttribute(attributeName, value);
        }

        private void ExecuteAutomationScript(string command)
        {
            var elementId = ((JArray)this.Parameters["args"])[0]["ELEMENT"].ToString();
            var element = this.Automator.WebElements.GetRegisteredElement(elementId);
            var peer = FrameworkElementAutomationPeer.FromElement(element);
            if (peer == null)
            {
                throw new AutomationException("Element does not support AutomationPeer.", ResponseStatus.JavaScriptError);
            }

            switch (command)
            {
                case "invoke":
                    var invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    if (invokeProvider == null)
                    {
                        throw new AutomationException(
                            "Element does not support IInvokeProvider control pattern interface.", 
                            ResponseStatus.JavaScriptError);
                    }

                    invokeProvider.Invoke();
                    break;
                default:
                    var msg = string.Format(
                        HelpUnknownScriptMsg, "automation:", command, HelpUrlAutomationScript);
                    throw new AutomationException(msg, ResponseStatus.JavaScriptError);
            }
        }

        #endregion
    }
}
