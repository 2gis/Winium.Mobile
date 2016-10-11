namespace Winium.Silverlight.InnerServer.Commands
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls;
    using System.Windows.Media;

    using Winium.Mobile.Common;
    using Winium.Mobile.Common.Exceptions;
    using Winium.Silverlight.InnerServer.Commands.FindByHelpers;

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

            var popups = VisualTreeHelper.GetOpenPopups();
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
                return null;
            }

            throw new AutomationException("No alert is displayed", ResponseStatus.NoAlertOpenError);
        }

        #endregion
    }
}
