namespace Winium.StoreApps.InnerServer.Commands.Helpers
{
    #region

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Automation.Peers;
    using Windows.UI.Xaml.Automation.Provider;
    using Windows.UI.Xaml.Controls;

    using Winium.Mobile.Common;
    using Winium.Mobile.Common.Exceptions;

    #endregion

    internal static class ContorlExtensions
    {
        internal static void TrySetText(this Control element, string text)
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
    }
}
