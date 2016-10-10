namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using System.Linq;

    using Windows.UI.Xaml.Controls;

    using Winium.Mobile.Common;
    using Winium.Mobile.Common.Exceptions;
    using Winium.StoreApps.InnerServer.Commands.Helpers;
    using Winium.StoreApps.InnerServer.Element;

    #endregion

    internal class AlertTextCommand : CommandBase
    {
        #region Public Methods and Operators

        protected override string DoImpl()
        {
            var popup = WiniumVirtualRoot.Current.OpenPopups.FirstOrDefault();
            if (popup == null || !popup.ClassName.EndsWith(".ContentDialog"))
            {
                throw new AutomationException("No alert is displayed", ResponseStatus.NoAlertOpenError);
            }

            var strategy = By.ClassName(typeof(TextBlock).FullName);
            var textBoxes = popup.Find(TreeScope.Descendants, strategy.Predicate);
            var message = string.Join("\n", textBoxes.Select(x => x.GetText()));
            
            return this.JsonResponse(ResponseStatus.Success, message);
        }

        #endregion

        #region Methods

        #endregion
    }
}
