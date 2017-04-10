namespace Winium.Mobile.Driver.Web
{
    #region

    using System.Collections.Generic;

    using WindowsPhoneDriverBrowser;

    using Winium.Mobile.Common;

    #endregion

    internal static class DriverCommandToAtomMapping
    {
        private static readonly Dictionary<string, string> Mapping;

        static DriverCommandToAtomMapping()
        {
            Mapping = new Dictionary<string, string>
                          {
                                  { DriverCommand.ClearElement, WebDriverAtoms.Clear },
                                  { DriverCommand.ClickElement, WebDriverAtoms.Click },
                                  { DriverCommand.ExecuteAsyncScript, WebDriverAtoms.ExecuteAsyncScript },
                                  { DriverCommand.ExecuteScript, WebDriverAtoms.ExecuteScript },
                                  { DriverCommand.FindChildElement, WebDriverAtoms.FindElement },
                                  { DriverCommand.FindChildElements, WebDriverAtoms.FindElements },
                                  { DriverCommand.FindElement, WebDriverAtoms.FindElement },
                                  { DriverCommand.FindElements, WebDriverAtoms.FindElements },
                                  { DriverCommand.GetActiveElement, WebDriverAtoms.ActiveElement },
                                  { DriverCommand.GetElementAttribute, WebDriverAtoms.GetAttributeValue },
                                  { DriverCommand.GetElementLocation, WebDriverAtoms.GetTopLeftCoordinates },
                                  { DriverCommand.GetElementSize, WebDriverAtoms.GetSize },
                                  { DriverCommand.GetElementText, WebDriverAtoms.GetText },
                                  { DriverCommand.GetElementValueOfCssProperty, WebDriverAtoms.GetValueOfCssProperty },
                                  { DriverCommand.GetPageSource, WebDriverAtoms.ExecuteScript },
                                  { DriverCommand.SendKeysToElement, WebDriverAtoms.Type }
                          };
        }

        public static string GetAtomOrDefault(string commandName)
        {
            string atom;
            Mapping.TryGetValue(commandName, out atom);
            return atom;
        }
    }
}
