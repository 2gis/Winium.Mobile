namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using System.Linq;

    using Windows.UI.Xaml.Automation.Peers;
    using Windows.UI.Xaml.Automation.Provider;
    using Windows.UI.Xaml.Controls;

    using Winium.Mobile.Common;
    using Winium.Mobile.Common.Exceptions;
    using Winium.StoreApps.InnerServer.Commands.Helpers;
    using Winium.StoreApps.InnerServer.Element;

    #endregion

    internal class AlertCommand : CommandBase
    {
        #region Enums

        public enum With
        {
            Accept, 

            Dismiss
        }

        #endregion

        #region Public Properties

        public With Action { private get; set; }

        #endregion

        #region Methods

        protected override string DoImpl()
        {
            var buttonName = this.Action == With.Accept ? "Button1Host" : "Button2Host";

            var popup = WiniumVirtualRoot.Current.OpenPopups.FirstOrDefault();
            if (popup == null || !popup.ClassName.EndsWith(".ContentDialog"))
            {
                throw new AutomationException("No alert is displayed", ResponseStatus.NoAlertOpenError);
            }

            var hostStrategy = By.XName(buttonName);
            var buttonHost = popup.Find(TreeScope.Descendants, hostStrategy.Predicate).FirstOrDefault();

            if (buttonHost != null)
            {
                var btnStrategy = By.ClassName(typeof(Button).FullName);
                var button = buttonHost.Find(TreeScope.Children, btnStrategy.Predicate).FirstOrDefault();

                if (button != null)
                {
                    button.Element.GetProvider<IInvokeProvider>(PatternInterface.Invoke).Invoke();

                    return this.JsonResponse();
                }
            }

            throw new AutomationException("Could not find {0} in ContentDialog", buttonName);
        }

        #endregion
    }
}
