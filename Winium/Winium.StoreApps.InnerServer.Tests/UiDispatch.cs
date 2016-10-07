namespace Winium.StoreApps.InnerServer.Tests
{
    #region

    using System;
    using System.Threading.Tasks;

    using Windows.ApplicationModel.Core;
    using Windows.UI.Core;

    #endregion

    public static class UiDispatch
    {
        #region Public Methods and Operators

        public static Task Invoke(Action action)
        {
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            return dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => { action(); }).AsTask();
        }

        #endregion
    }
}
