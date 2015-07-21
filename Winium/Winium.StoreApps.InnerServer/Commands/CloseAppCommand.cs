namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using Windows.UI.Xaml;

    #endregion

    internal class CloseAppCommand : CommandBase
    {
        #region Public Methods and Operators

        protected override string DoImpl()
        {
            this.Automator.DoAfterResponseOnce = () => Application.Current.Exit();

            return this.JsonResponse();
        }

        #endregion
    }
}
