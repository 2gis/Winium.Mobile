namespace Winium.Mobile.Driver.CommandExecutors
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Winium.Mobile.Common;
    using Winium.Mobile.Common.Exceptions;


    #endregion

    internal class ExecuteScriptExecutor : CommandExecutorBase
    {
        #region Methods

        internal object ExecuteMobileScript(string command)
        {
            switch (command)
            {
                case "start":
                    this.Automator.EmulatorController.TypeKey(Keys.F2);
                    break;
                case "search":
                    this.Automator.EmulatorController.TypeKey(Keys.F3);
                    break;
                case "OnScreenKeyboard.Enable":
                    this.Automator.EmulatorController.TypeKey(Keys.PageUp);
                    break;
                case "OnScreenKeyboard.Disable":
                    this.Automator.EmulatorController.TypeKey(Keys.PageDown);
                    break;
                case "App.Open":
                    LaunchAppExecutor.LaunchApp(this.Automator);
                    break;
                case "App.Close":
                    CloseAppExecutor.CloseApp(this.Automator);
                    break;
                case "invokeMethod":
                    var arguments = (this.ExecutedCommand.Parameters["args"] as JArray).Select(jv => (string)jv).ToArray();

                    if (arguments == null)
                    {
                        throw new AutomationException("Bad parameters", ResponseStatus.JavaScriptError);
                    }

                    var type = (string)arguments.GetValue(0);
                    var method = (string)arguments.GetValue(1);

                    var parameters = new Dictionary<string, JToken>();
                    parameters["type"] = type;
                    parameters["method"] = method;
                    var args = arguments.OfType<object>().Skip(2).ToArray();
                    if (args.Any())
                    {
                        parameters["args"] = new JArray(args);
                    }

                    var invokeCommand = new Command(DriverCommand.ExecuteScript, parameters);
                    return this.Automator.CommandForwarder.ForwardCommand(invokeCommand);
                default:
                    const string Url =
                        "https://github.com/2gis/windows-universal-app-driver/wiki/Command-Execute-Script#press-hardware-button";
                    var msg = string.Format(
                        "Unknown 'mobile:' script command '{0}'. See {1} for supported commands.",
                        command ?? string.Empty,
                        Url);
                    throw new AutomationException(msg, ResponseStatus.JavaScriptError);
            }

            return null;
        }

        internal object ForwardCommand()
        {
            var responseBody = this.Automator.CommandForwarder.ForwardCommand(this.ExecutedCommand);
            var deserializeObject = JsonConvert.DeserializeObject<JsonResponse>(responseBody);
            if (deserializeObject.Status != ResponseStatus.Success)
            {
                throw new AutomationException(deserializeObject.Value.ToString(), ResponseStatus.JavaScriptError);
            }

            return deserializeObject.Value;
        }

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

            object response;

            switch (prefix)
            {
                case "mobile:":
                    response = this.ExecuteMobileScript(command);
                    break;
                default:
                    response = this.ForwardCommand();
                    break;
            }

            return this.JsonResponse(ResponseStatus.Success, response);
        }

        #endregion
    }
}
