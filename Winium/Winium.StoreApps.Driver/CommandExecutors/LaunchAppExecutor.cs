namespace Winium.StoreApps.Driver.CommandExecutors
{
    using Winium.StoreApps.Driver.Automator;

    internal class LaunchAppExecutor : CommandExecutorBase
    {
        #region Public Methods and Operators

        public static void LaunchApp(Automator automator)
        {
            automator.Deployer.Launch();
            automator.ConnectToApp();
        }

        #endregion

        #region Methods

        protected override string DoImpl()
        {
            LaunchApp(this.Automator);

            return this.JsonResponse();
        }

        #endregion
    }
}
