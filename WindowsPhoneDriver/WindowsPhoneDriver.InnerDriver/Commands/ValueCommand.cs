namespace WindowsPhoneDriver.InnerDriver.Commands
{
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls;

    using WindowsPhoneDriver.Common;
    using WindowsPhoneDriver.Common.Exceptions;

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
            var peer = new TextBoxAutomationPeer(textbox);
            var valueProvider = peer.GetPattern(PatternInterface.Value) as IValueProvider;
            if (valueProvider != null)
            {
                valueProvider.SetValue(text);
            }

            textbox.Focus();
        }

        #endregion
    }
}
