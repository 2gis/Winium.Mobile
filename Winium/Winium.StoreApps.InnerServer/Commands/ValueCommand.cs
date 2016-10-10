namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Automation.Peers;
    using Windows.UI.Xaml.Automation.Provider;
    using Windows.UI.Xaml.Controls;

    using Winium.Mobile.Common;
    using Winium.Mobile.Common.Exceptions;
    using Winium.StoreApps.InnerServer.Commands.Helpers;

    #endregion

    internal class ValueCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        public string KeyString { get; set; }

        #endregion

        #region Public Methods and Operators

        protected override string DoImpl()
        {
            var element = this.Automator.ElementsRegistry.GetRegisteredElement(this.ElementId);
            var control = element.Element as Control;
            if (control == null)
            {
                throw new AutomationException("Element referenced is not of control type.", ResponseStatus.UnknownError);
            }

            control.TrySetText(this.KeyString);
            return this.JsonResponse();
        }

        #endregion
    }
}
