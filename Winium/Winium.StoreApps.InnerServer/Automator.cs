namespace Winium.StoreApps.InnerServer
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Windows.UI.Core;
    using Windows.UI.Xaml;

    using Winium.Mobile.Common;
    using Winium.StoreApps.InnerServer.Commands;

    #endregion

    internal class Automator
    {
        #region Constructors and Destructors

        public Automator()
        {
            if (Window.Current == null)
            {
                throw new InvalidOperationException("This method must be called from UI thread.");
            }

            this.UiThreadDispatcher = Window.Current.Dispatcher;
            this.ElementsRegistry = new ElementsRegistry();
            this.DoAfterResponseOnce = null;
        }

        #endregion

        #region Public Properties

        public Action DoAfterResponseOnce { get; set; }

        public CoreDispatcher UiThreadDispatcher { get; private set; }

        public ElementsRegistry ElementsRegistry { get; private set; }

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

            CommandBase commandToExecute;

            if (command.Equals("ping"))
            {
                // Service command
                return "<pong>";
            }

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
            else if (command.Equals(DriverCommand.SendKeysToElement))
            {
                var values = parameters["value"].ToObject<List<string>>();
                var value = string.Empty;
                if (values.Any())
                {
                    value = values.Aggregate((aggregated, next) => aggregated + next);
                }

                commandToExecute = new ValueCommand { ElementId = elementId, KeyString = value };
            }
            else if (command.Equals(DriverCommand.ClearElement))
            {
                commandToExecute = new ClearCommand { ElementId = elementId };
            }
            else if (command.Equals(DriverCommand.GetElementText))
            {
                commandToExecute = new TextCommand { ElementId = elementId };
            }
            else if (command.Equals(DriverCommand.IsElementDisplayed))
            {
                commandToExecute = new DisplayedCommand { ElementId = elementId };
            }
            else if (command.Equals(DriverCommand.IsElementEnabled))
            {
                commandToExecute = new IsElementEnabledCommand { ElementId = elementId };
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
            else if (command.Equals(DriverCommand.GetElementTagName))
            {
                commandToExecute = new GetElementTagNameCommand { ElementId = elementId };
            }
            else if (command.Equals(DriverCommand.CloseApp))
            {
                commandToExecute = new CloseAppCommand();
            }
            else if (command.Equals(ExtendedDriverCommand.InvokeMethodCommand))
            {
                commandToExecute = new InvokeMethodCommand();
            }
            else
            {
                throw new NotImplementedException("Not implemented: " + command);
            }

            JToken sessionIdObject;
            if (parameters.TryGetValue("SESSIONID", out sessionIdObject))
            {
                commandToExecute.Session = sessionIdObject.ToString();
            }
            else
            {
                commandToExecute.Session = string.Empty;
            }

            // TODO: Replace passing Automator to command with passing some kind of configuration
            commandToExecute.Automator = this;
            commandToExecute.Parameters = parameters;

            var response = commandToExecute.Do();

            return response;
        }

        #endregion
    }
}
