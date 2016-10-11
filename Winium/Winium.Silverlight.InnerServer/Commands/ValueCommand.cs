namespace Winium.Silverlight.InnerServer.Commands
{
    using System.Windows.Automation.Peers;
    using System.Windows.Automation.Provider;
    using System.Windows.Controls;

    using Winium.Mobile.Common;
    using Winium.Mobile.Common.Exceptions;

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
            var control = element as Control;
            if (control == null)
            {
                throw new AutomationException("Element referenced is not of control type.", ResponseStatus.UnknownError);
            }

            TrySetText(control, this.KeyString);
            return this.JsonResponse(ResponseStatus.Success, null);
        }

        #endregion

        #region Methods

        private static void TrySetText(Control element, string text)
        {
            var peer = FrameworkElementAutomationPeer.FromElement(element);
            var provider = peer == null ? null : peer.GetPattern(PatternInterface.Value) as IValueProvider;

            if (provider != null)
            {
                provider.SetValue(text);
            }
            else if (element is TextBox)
            {
                var textBox = element as TextBox;
                textBox.Text = text;
                textBox.SelectionStart = text.Length;
            }
            else if (element is PasswordBox)
            {
                var passwordBox = element as PasswordBox;
                passwordBox.Password = text;
            }
            else
            {
                throw new AutomationException("Element does not support SendKeys.", ResponseStatus.UnknownError);
            }

            // TODO: new parameter - FocusState
            element.Focus();
        }


        #endregion
    }
}
