namespace Winium.Mobile.Driver.CommandExecutors
{
    using Winium.Mobile.Driver.Automator;

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
