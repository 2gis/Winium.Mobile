namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using Windows.UI.Xaml.Automation.Peers;
    using Windows.UI.Xaml.Automation.Provider;
    using Windows.UI.Xaml.Controls;
    using Common;

    #endregion

    internal class ClickCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { private get; set; }

        #endregion

        #region Public Methods and Operators

        protected override string DoImpl()
        {
            var element = this.Automator.ElementsRegistry.GetRegisteredElement(this.ElementId);
            Button button = element.Element as Button;
            if (button != null)
            {
                ButtonAutomationPeer peer = new ButtonAutomationPeer(button);
                IInvokeProvider invokeProv = peer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
                if (invokeProv != null)
                {
                    invokeProv.Invoke();
                    return this.JsonResponse(ResponseStatus.Success, "");
                }

                return this.JsonResponse(ResponseStatus.UnknownError, "Failed to create invocation provider : " + this.ElementId);
            }

            return this.JsonResponse(ResponseStatus.UnknownError, "Element is not a button" + this.ElementId);
        }

        #endregion
    }
}