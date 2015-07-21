namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Automation.Peers;
    using Windows.UI.Xaml.Automation.Provider;
    using Windows.UI.Xaml.Controls;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.Common.Exceptions;
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
            var element = this.Automator.WebElements.GetRegisteredElement(this.ElementId);
            var control = element as Control;
            if (control == null)
            {
                throw new AutomationException("Element referenced is not of control type.", ResponseStatus.UnknownError);
            }

            TrySetText(control, this.KeyString);
            return this.JsonResponse();
        }

        #endregion

        #region Methods

        private static void TrySetText(Control element, string text)
        {
            // TODO Research why TextBox does not support IValueProvider
            var provider = element.GetProviderOrDefault<IValueProvider>(PatternInterface.Value);

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
            element.Focus(FocusState.Pointer);
        }

        #endregion
    }
}
