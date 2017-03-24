namespace Winium.Silverlight.InnerServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Winium.Mobile.Common;
    using Winium.Silverlight.InnerServer.Commands;
    using Web.Commands;
    using Web;
    using Microsoft.Phone.Controls;

    internal class Automator
    {
        #region Constructors and Destructors

        public Automator(UIElement visualRoot)
        {
            this.VisualRoot = visualRoot;
            this.WebElements = new AutomatorElements();
            this.ContextsRegistry = new ContextsRegistry();
            this.CurrentContext = ContextsRegistry.NativeAppContext;
        }

        #endregion

        #region Public Properties

        public UIElement VisualRoot { get; private set; }

        public AutomatorElements WebElements { get; private set; }

        public string CurrentContext { get; set; }

        public WebBrowser browser { get; set; }

        public ContextsRegistry ContextsRegistry { get; private set; }


        #endregion

        #region Public Methods and Operators

        public string ProcessCommand(string content)
        {
            var requestData = JsonConvert.DeserializeObject<Command>(content);
            var command = requestData.Name;
            var parameters = requestData.Parameters;

            string elementId = null;
            if (parameters == null)
            {
                throw new NullReferenceException("Parameters can not be NULL");
            }

            JToken elementIdObject;
            if (parameters.TryGetValue("ID", out elementIdObject))
            {
                elementId = elementIdObject.ToString();
            }

            CommandBase commandToExecute = null;

            if (command.Equals("ping"))
            {
                // Service command
                return "<pong>";
            }

            if (command.Equals(DriverCommand.Contexts))
            {
                commandToExecute = new GetWebContextsCommand();
            }
            else if (command.Equals(DriverCommand.SetContext))
            {
                commandToExecute = new SetContextCommand();
            }

            // TODO: Refactor similar to CommandExecutors in OuterDriver
            // TODO: Refactor similar to CommandExecutors in Driver
            if (commandToExecute == null)
            {
                commandToExecute = this.CurrentContext == ContextsRegistry.NativeAppContext
                                       ? GetNativeAppCommandToExecute(command, elementId, parameters)
                                       : this.GetWebCommandToExecute(requestData);
            }

           

            JToken sessionIdObject;
            commandToExecute.Session = parameters.TryGetValue("SESSIONID", out sessionIdObject)
                                           ? sessionIdObject.ToString()
                                           : string.Empty;

            // TODO: Replace passing Automator to command with passing some kind of configuration
            commandToExecute.Automator = this;
            commandToExecute.Parameters = parameters;

            var response = commandToExecute.Do();

            return response;
        }

        private CommandBase GetWebCommandToExecute(Command request)
        {
            var command = request.Name;
            WebCommandHandler commandToExecute;

            var context = this.ContextsRegistry.GetContext(this.CurrentContext);

            if (command.Equals(DriverCommand.Get))
            {
                commandToExecute = new GoToUrlCommandHandler();
            }
            else if (command.Equals(DriverCommand.FindElement))
            {
                commandToExecute = new FindElementCommandHandler();
            }
            else if (command.Equals(DriverCommand.ClickElement))
            {
                commandToExecute = new ClickElementCommandHandler();
            }
            else if (command.Equals(DriverCommand.SendKeysToElement))
            {
                commandToExecute = new SendKeysCommandHandler();
            }
            else if (command.Equals(DriverCommand.GetPageSource))
            {
                commandToExecute = new GetPageSourceCommandHandler();
            }
            else
            {
                throw new NotImplementedException("Not implemented: " + command);
            }

            commandToExecute.Atom = request.Atom;
            commandToExecute.Context = context;

            return commandToExecute;

        }

        private CommandBase GetNativeAppCommandToExecute(string command, string elementId, IDictionary<string, JToken> parameters)
        {
            CommandBase commandToExecute;
            // TODO: Refactor similar to CommandExecutors in Driver
            if (command.Equals(DriverCommand.GetAlertText))
            {
                commandToExecute = new AlertTextCommand();
            }
            else if (command.Equals(DriverCommand.AcceptAlert))
            {
                commandToExecute = new AlertCommand { Action = AlertCommand.With.Accept };
            }
            else if (command.Equals(DriverCommand.DismissAlert))
            {
                commandToExecute = new AlertCommand { Action = AlertCommand.With.Dismiss };
            }
            else if (command.Equals(DriverCommand.FindElement) || command.Equals(DriverCommand.FindChildElement))
            {
                commandToExecute = new ElementCommand { ElementId = elementId };
            }
            else if (command.Equals(DriverCommand.FindElements) || command.Equals(DriverCommand.FindChildElements))
            {
                commandToExecute = new ElementsCommand { ElementId = elementId };
            }
            else if (command.Equals(DriverCommand.ClickElement))
            {
                commandToExecute = new ClickCommand { ElementId = elementId };
            }
            else if (command.Equals(DriverCommand.SendKeysToElement))
            {
                var values = ((JArray)parameters["value"]).ToObject<List<string>>();
                var value = string.Empty;
                if (values.Any())
                {
                    value = values.Aggregate((aggregated, next) => aggregated + next);
                }

                commandToExecute = new ValueCommand { ElementId = elementId, KeyString = value };
            }
            else if (command.Equals(DriverCommand.GetElementText))
            {
                commandToExecute = new TextCommand { ElementId = elementId };
            }
            else if (command.Equals(DriverCommand.IsElementDisplayed))
            {
                commandToExecute = new DisplayedCommand { ElementId = elementId };
            }
            else if (command.Equals(DriverCommand.GetElementLocation))
            {
                commandToExecute = new LocationCommand { ElementId = elementId };
            }
            else if (command.Equals(DriverCommand.GetElementLocationOnceScrolledIntoView))
            {
                commandToExecute = new LocationInViewCommand { ElementId = elementId };
            }
            else if (command.Equals(DriverCommand.GetElementSize))
            {
                commandToExecute = new GetElementSizeCommand { ElementId = elementId };
            }
            else if (command.Equals(DriverCommand.GetElementRect))
            {
                commandToExecute = new GetElementRectCommand { ElementId = elementId };
            }
            else if (command.Equals(DriverCommand.GetPageSource))
            {
                commandToExecute = new PageSourceCommand { ElementId = elementId };
            }
            else if (command.Equals(DriverCommand.GetOrientation))
            {
                commandToExecute = new OrientationCommand();
            }
            else if (command.Equals(DriverCommand.GetElementAttribute))
            {
                commandToExecute = new GetElementAttributeCommand { ElementId = elementId };
            }
            else if (command.Equals(DriverCommand.ExecuteScript))
            {
                commandToExecute = new ExecuteCommand();
            }
            else if (command.Equals(ExtendedDriverCommand.InvokeAppBarItemCommand))
            {
                commandToExecute = new InvokeAppBarItemCommand();
            }
            else if (command.Equals(ExtendedDriverCommand.InvokeMethodCommand))
            {
                commandToExecute = new InvokeMethodCommand();
            }
            else
            {
                throw new NotImplementedException("Not implemented: " + command);
            }

            return commandToExecute;
        }

        #endregion
    }
}
