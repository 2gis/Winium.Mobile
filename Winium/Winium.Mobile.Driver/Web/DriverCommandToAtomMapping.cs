using System.Collections.Generic;

namespace Winium.StoreApps.Driver.Web
{
    using WindowsPhoneDriverBrowser;

    using Winium.Mobile.Common;

    internal static class DriverCommandToAtomMapping
    {
        public static string GetAtomOrDefault(string commandName)
        {
            string atom;
            Mapping.TryGetValue(commandName, out atom);
            return atom;
        }

        private static readonly Dictionary<string, string> Mapping = new Dictionary<string, string>
                                                                        {
                                                                            //{ DriverCommand.AcceptAlert, WebDriverAtoms. },
                                                                            //{ DriverCommand.AddCookie, WebDriverAtoms. },
                                                                                {
                                                                                    DriverCommand.ClearElement,
                                                                                    WebDriverAtoms.Clear
                                                                                },
                                                                                {
                                                                                    DriverCommand.ClickElement,
                                                                                    WebDriverAtoms.Click
                                                                                },
                                                                            //{ DriverCommand.Close, WebDriverAtoms. },
                                                                            //{ DriverCommand.DefineDriverMapping, WebDriverAtoms. },
                                                                            //{ DriverCommand.DeleteAllCookies, WebDriverAtoms. },
                                                                            //{ DriverCommand.DeleteCookie, WebDriverAtoms. },
                                                                            //{ DriverCommand.DescribeElement, WebDriverAtoms. },
                                                                            //{ DriverCommand.DismissAlert, WebDriverAtoms. },
                                                                            //{ DriverCommand.ElementEquals, WebDriverAtoms. },
                                                                                {
                                                                                    DriverCommand.ExecuteAsyncScript,
                                                                                    WebDriverAtoms.ExecuteAsyncScript
                                                                                },
                                                                                {
                                                                                    DriverCommand.ExecuteScript,
                                                                                    WebDriverAtoms.ExecuteScript
                                                                                },
                                                                                {
                                                                                    DriverCommand.FindChildElement,
                                                                                    WebDriverAtoms.FindElement
                                                                                },
                                                                                {
                                                                                    DriverCommand.FindChildElements,
                                                                                    WebDriverAtoms.FindElements
                                                                                },
                                                                                {
                                                                                    DriverCommand.FindElement,
                                                                                    WebDriverAtoms.FindElement
                                                                                },
                                                                                {
                                                                                    DriverCommand.FindElements,
                                                                                    WebDriverAtoms.FindElements
                                                                                },
                                                                            //{ DriverCommand.Get, WebDriverAtoms. },
                                                                                {
                                                                                    DriverCommand.GetActiveElement,
                                                                                    WebDriverAtoms.ActiveElement
                                                                                },
                                                                            //{ DriverCommand.GetAlertText, WebDriverAtoms. },
                                                                            //{ DriverCommand.GetAllCookies, WebDriverAtoms. },
                                                                            //{ DriverCommand.GetCurrentUrl, WebDriverAtoms. },
                                                                            //{ DriverCommand.GetCurrentWindowHandle, WebDriverAtoms. },
                                                                                {
                                                                                    DriverCommand.GetElementAttribute,
                                                                                    WebDriverAtoms.GetAttributeValue
                                                                                },
                                                                                {
                                                                                    DriverCommand.GetElementLocation,
                                                                                    WebDriverAtoms.GetTopLeftCoordinates
                                                                                },
                                                                            //{ DriverCommand.GetElementLocationOnceScrolledIntoView, WebDriverAtoms. },
                                                                                {
                                                                                    DriverCommand.GetElementSize,
                                                                                    WebDriverAtoms.GetSize
                                                                                },
                                                                            //{ DriverCommand.GetElementTagName, WebDriverAtoms. },
                                                                                {
                                                                                    DriverCommand.GetElementText,
                                                                                    WebDriverAtoms.GetText
                                                                                },
                                                                                {
                                                                                    DriverCommand
                                                                                        .GetElementValueOfCssProperty,
                                                                                    WebDriverAtoms.GetValueOfCssProperty
                                                                                },
                                                                            //{ DriverCommand.GetOrientation, WebDriverAtoms. },
                                                                           { DriverCommand.GetPageSource, WebDriverAtoms.ExecuteScript },
                                                                            //{ DriverCommand.GetSessionCapabilities, WebDriverAtoms. },
                                                                            //{ DriverCommand.GetSessionList, WebDriverAtoms. },
                                                                            //{ DriverCommand.GetTitle, WebDriverAtoms. },
                                                                            //{ DriverCommand.GetWindowHandles, WebDriverAtoms. },
                                                                            //{ DriverCommand.GetWindowPosition, WebDriverAtoms. },
                                                                            //{ DriverCommand.SetContext, WebDriverAtoms. }
                                                                        };
    }
}