namespace Winium.Silverlight.InnerServer.Commands
{
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    using Winium.Mobile.Common;
    using Winium.Mobile.Common.Exceptions;
    using Winium.Silverlight.InnerServer.Commands.FindByHelpers;

    internal class AlertTextCommand : CommandBase
    {
        #region Public Methods and Operators

        public override string DoImpl()
        {
            var message = string.Empty;

            var popups = VisualTreeHelper.GetOpenPopups().ToList();
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
