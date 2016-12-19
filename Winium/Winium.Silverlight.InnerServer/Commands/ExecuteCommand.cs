namespace Winium.Silverlight.InnerServer.Commands
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;

    using Newtonsoft.Json.Linq;

    using Winium.Mobile.Common;
    using Winium.Mobile.Common.Exceptions;

    #endregion

    internal class ExecuteCommand : CommandBase
    {
        #region Constants

        private const string HelpArgumentsErrorMsg = "Arguments error. See {0} for more information.";

        private const string HelpUnknownScriptMsg = "Unknown script command '{0} {1}'. See {2} for supported commands.";

        private const string HelpUrlAttributeScript =
            "https://github.com/2gis/Winium.Mobile/wiki/Command-Execute-Script#set-property-on-element";

        private const string HelpUrlAutomationScript =
            "https://github.com/2gis/Winium.Mobile/wiki/Command-Execute-Script#use-automationpeerspatterninterface-on-element";

        private const string HelpUrlScript =
            "https://github.com/2gis/Winium.Mobile/wiki/Command-Execute-Script#supported-scripts";

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

            object response;
            switch (prefix)
            {
                case "automation:":
                    response = this.ExecuteAutomationScript(command);
                    break;
                default:
                    var msg = string.Format(HelpUnknownScriptMsg, prefix, command, HelpUrlScript);
                    throw new AutomationException(msg, ResponseStatus.JavaScriptError);
            }

            return this.JsonResponse(ResponseStatus.Success, response);
        }

        #endregion

        #region Methods

        private static object AutomationInvokeAction(FrameworkElement element)
        {
            var invokeProvider = element.GetProvider<IInvokeProvider>(PatternInterface.Invoke);
            invokeProvider.Invoke();

            return null;
        }

        private static object AutomationScrollAction(FrameworkElement element, IDictionary<string, JToken> parameters)
        {
            var scrollProvider = element.GetProvider<IScrollProvider>(PatternInterface.Scroll);

            var scrollInfo = parameters["args"].ElementAtOrDefault(1);
            if (scrollInfo == null)
            {
                var msg = string.Format(HelpArgumentsErrorMsg, HelpUrlAutomationScript);
                throw new AutomationException(msg, ResponseStatus.JavaScriptError);
            }

            var horizontalAmount = ParseScrollAmount(scrollInfo["horizontal"] ?? scrollInfo["h"]);
            var verticalAmount = ParseScrollAmount(scrollInfo["vertical"] ?? scrollInfo["v"]);
            var count = (scrollInfo["count"] ?? 1).Value<int>();

            if (horizontalAmount != ScrollAmount.NoAmount && !scrollProvider.HorizontallyScrollable)
            {
                throw new AutomationException("Element is not horizontally scrollable.", ResponseStatus.JavaScriptError);
            }

            if (verticalAmount != ScrollAmount.NoAmount && !scrollProvider.VerticallyScrollable)
            {
                throw new AutomationException("Element is not vertically scrollable.", ResponseStatus.JavaScriptError);
            }

            for (var i = 0; i < count; ++i)
            {
                scrollProvider.Scroll(horizontalAmount, verticalAmount);
            }

            return null;
        }

        private static ScrollAmount ParseScrollAmount(JToken jToken)
        {
            ScrollAmount scrollAmount;
            if (Enum.TryParse(jToken != null ? jToken.ToString() : "NoAmount", true, out scrollAmount))
            {
                return scrollAmount;
            }

            var msg = string.Format(HelpArgumentsErrorMsg, HelpUrlAutomationScript);
            throw new AutomationException(msg, ResponseStatus.JavaScriptError);
        }

        private static object TogglePatternToggle(FrameworkElement element)
        {
            var provider = element.GetProvider<IToggleProvider>(PatternInterface.Toggle);
            provider.Toggle();

            return null;
        }

        private static string TogglePatternToggleState(FrameworkElement element)
        {
            var provider = element.GetProvider<IToggleProvider>(PatternInterface.Toggle);
            return provider.ToggleState.ToString();
        }

        private static string GetClickablePoint(FrameworkElement element)
        {
            var peer = element.GetAutomationPeer();
            return peer.GetClickablePoint().ToString();
        }

        private static bool IsOffscreen(FrameworkElement element)
        {
            var peer = element.GetAutomationPeer();
            return peer.IsOffscreen();
        }

        private object ExecuteAutomationScript(string command)
        {
            var elementId = ((JArray)this.Parameters["args"])[0]["ELEMENT"].ToString();
            var element = this.Automator.WebElements.GetRegisteredElement(elementId);

            switch (command)
            {
                case "InvokePattern.Invoke":
                case "invoke":
                    return AutomationInvokeAction(element);
                case "ScrollPattern.Scroll":
                case "scroll":
                    return AutomationScrollAction(element, this.Parameters);
                case "TogglePattern.Toggle":
                    return TogglePatternToggle(element);
                case "TogglePattern.ToggleState":
                    return TogglePatternToggleState(element);
                case "GetClickablePoint":
                    return GetClickablePoint(element);
                case "IsOffscreen":
                    return IsOffscreen(element);
                default:
                    var msg = string.Format(HelpUnknownScriptMsg, "automation:", command, HelpUrlAutomationScript);
                    throw new AutomationException(msg, ResponseStatus.JavaScriptError);
            }
        }

        #endregion
    }
}
