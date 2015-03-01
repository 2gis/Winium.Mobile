namespace WindowsUniversalAppDriver.InnerServer.Commands
{
    using System.Linq;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Automation.Peers;
    using Windows.UI.Xaml.Automation.Provider;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;

    using WindowsUniversalAppDriver.Common;
    using WindowsUniversalAppDriver.Common.Exceptions;
    using WindowsUniversalAppDriver.InnerServer.Commands.Helpers;

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

        public With Action { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string DoImpl()
        {
            var buttonName = this.Action == With.Accept ? "LeftButton" : "RightButton";

            // TODO: new parameter - Window
            var popups = VisualTreeHelper.GetOpenPopups(Window.Current);
            foreach (var popup in popups)
            {
                var popupChild = popup.Child;
                var element = (FrameworkElement)Finder.GetDescendantsBy(popupChild, new By("name", buttonName)).FirstOrDefault() as Button;
                if (element == null)
                {
                    continue;
                }

                var peer = new ButtonAutomationPeer(element);
                var invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                if (invokeProv == null)
                {
                    continue;
                }

                invokeProv.Invoke();
                return this.JsonResponse();
            }

            throw new AutomationException("No alert is displayed", ResponseStatus.NoAlertOpenError);
        }

        #endregion
    }
}
