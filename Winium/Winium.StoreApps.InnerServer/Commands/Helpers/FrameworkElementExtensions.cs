namespace Winium.StoreApps.InnerServer.Commands.Helpers
{
    #region

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Automation;
    using Windows.UI.Xaml.Automation.Peers;

    using Winium.Mobile.Common.Exceptions;

    #endregion

    internal static class FrameworkElementExtensions
    {
        #region Constants

        private const string HelpNotSupportInterfaceMsg = "Element does not support {0} control pattern interface.";

        #endregion

        #region Methods

        internal static string AutomationId(this FrameworkElement element)
        {
            return element == null ? null : element.GetValue(AutomationProperties.AutomationIdProperty) as string;
        }

        internal static string AutomationName(this FrameworkElement element)
        {
            return element == null ? null : element.GetValue(AutomationProperties.NameProperty) as string;
        }

        internal static string ClassName(this FrameworkElement element)
        {
            return element == null ? null : element.GetType().ToString();
        }

        internal static AutomationPeer GetAutomationPeer(this FrameworkElement element)
        {
            var peer = FrameworkElementAutomationPeer.CreatePeerForElement(element);
            if (peer == null)
            {
                throw new AutomationException("Element does not support AutomationPeer.");
            }

            return peer;
        }

        internal static T GetProviderOrDefault<T>(this FrameworkElement element, PatternInterface patternInterface)
            where T : class
        {
            var peer = GetAutomationPeer(element);

            return peer == null ? null : peer.GetPattern(patternInterface) as T;
        }

        internal static T GetProvider<T>(this FrameworkElement element, PatternInterface patternInterface)
            where T : class
        {
            var provider = element.GetProviderOrDefault<T>(patternInterface);
            if (provider != null)
            {
                return provider;
            }

            throw new AutomationException(string.Format(HelpNotSupportInterfaceMsg, typeof(T).Name));
        }

        #endregion
    }
}
