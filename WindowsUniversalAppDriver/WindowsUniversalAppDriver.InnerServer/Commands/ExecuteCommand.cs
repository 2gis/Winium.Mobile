namespace WindowsUniversalAppDriver.InnerServer.Commands
{
    #region using

    using System.Collections.Generic;

    using Windows.UI.Xaml.Automation.Peers;
    using Windows.UI.Xaml.Automation.Provider;

    using WindowsUniversalAppDriver.Common;
    using WindowsUniversalAppDriver.Common.Exceptions;

    using Newtonsoft.Json;

    #endregion

    internal class ExecuteCommand : CommandBase
    {
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
                default:
                    const string url = "https://github.com/2gis/winphonedriver/wiki/Command-Execute-Script";
                    var msg = string.Format("Unknown script prefix '{0}'. See {1} for supported scripts.", prefix, url);
                    throw new AutomationException(msg, ResponseStatus.JavaScriptError);
            }

            return this.JsonResponse(ResponseStatus.Success, string.Empty);
        }

        #endregion

        #region Methods

        internal void ExecuteAutomationScript(string command)
        {
            var args = JsonConvert.DeserializeObject<Dictionary<string, object>[]>(this.Parameters["args"].ToString());
            var elementId = args[0]["ELEMENT"].ToString();
            var element = this.Automator.WebElements.GetRegisteredElement(elementId);
            var peer = FrameworkElementAutomationPeer.FromElement(element);
            if (peer == null)
            {
                throw new AutomationException("Element not supported AutomationPeer", ResponseStatus.JavaScriptError);
            }

            switch (command)
            {
                case "invoke":
                    var invokeProvider = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                    if (invokeProvider == null)
                    {
                        throw new AutomationException("Element not supported invoke interface", 
                                                      ResponseStatus.JavaScriptError);
                    }

                    invokeProvider.Invoke();
                    break;
                default:
                    // TODO: Need a more specific URL
                    const string url = "https://github.com/2gis/winphonedriver/wiki/Command-Execute-Script";
                    var msg = string.Format("Unknown 'automation:' script command '{0}'. "
                                            + "See {1} for supported commands.", command ?? string.Empty, url);
                    throw new AutomationException(msg, ResponseStatus.JavaScriptError);
            }
        }

        #endregion
    }
}
