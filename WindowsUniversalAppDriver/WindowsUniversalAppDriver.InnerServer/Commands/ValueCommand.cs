namespace WindowsUniversalAppDriver.InnerServer.Commands
{
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Automation.Peers;
    using Windows.UI.Xaml.Automation.Provider;
    using Windows.UI.Xaml.Controls;

    using WindowsUniversalAppDriver.Common;
    using WindowsUniversalAppDriver.Common.Exceptions;

    internal class ValueCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        public string KeyString { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string DoImpl()
        {
            var element = this.Automator.WebElements.GetRegisteredElement(this.ElementId);
            var textbox = element as TextBox;
            if (textbox == null)
            {
                throw new AutomationException("Element referenced is not a TextBox.", ResponseStatus.UnknownError);
            }

            TrySetText(textbox, this.KeyString);
            return this.JsonResponse(ResponseStatus.Success, null);
        }

        #endregion

        #region Methods

        private static void TrySetText(TextBox textbox, string text)
        {
            // TODO: why IValueProvider is null in UniApp?
            var peer = new TextBoxAutomationPeer(textbox);
            var valueProvider = peer.GetPattern(PatternInterface.Value) as IValueProvider;
            if (valueProvider != null)
            {
                valueProvider.SetValue(text);
            }
            else
            {
                textbox.Text = text;
                textbox.SelectionStart = text.Length;
            }

            // TODO: new parameter - FocusState
            textbox.Focus(FocusState.Pointer);
        }

        #endregion
    }
}
