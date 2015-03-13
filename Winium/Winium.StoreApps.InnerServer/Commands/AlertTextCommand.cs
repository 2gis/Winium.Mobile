namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using System.Linq;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Media;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.Common.Exceptions;
    using Winium.StoreApps.InnerServer.Commands.Helpers;

    #endregion

    internal class AlertTextCommand : CommandBase
    {
        #region Public Methods and Operators

        public override string DoImpl()
        {
            var message = string.Empty;

            // TODO: new parameter - Window
            var popups = VisualTreeHelper.GetOpenPopups(Window.Current).ToList();
            var children = popups.Select(x => x.Child).ToList();
            if (!children.Any())
            {
                throw new AutomationException("No alert is displayed", ResponseStatus.NoAlertOpenError);
            }

            foreach (var popupChild in children)
            {
                message += FirstTextInChild(popupChild);
                if (!string.IsNullOrEmpty(message))
                {
                    break;
                }
            }

            return this.JsonResponse(ResponseStatus.Success, message);
        }

        #endregion

        #region Methods

        private static string FirstTextInChild(DependencyObject popupChild)
        {
            var elements = Finder.GetDescendantsBy(popupChild, new By("tag name", "System.Windows.Controls.TextBlock"));

            return
                elements.Select(o => o as TextBlock)
                    .Where(textBlock => textBlock != null)
                    .Select(textBlock => textBlock.Text)
                    .FirstOrDefault(x => !string.IsNullOrEmpty(x));
        }

        #endregion
    }
}
