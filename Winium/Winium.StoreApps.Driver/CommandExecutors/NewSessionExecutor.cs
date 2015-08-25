namespace Winium.StoreApps.Driver.CommandExecutors
{
    #region

    using Newtonsoft.Json;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.Driver.Automator;

    #endregion

    internal class NewSessionExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            // It is easier to reparse desired capabilities as JSON instead of re-mapping keys to attributes and calling type conversions, 
            // so we will take possible one time performance hit by serializing Dictionary and deserializing it as Capabilities object
            var serializedCapability =
                JsonConvert.SerializeObject(this.ExecutedCommand.Parameters["desiredCapabilities"]);
            this.Automator.ActualCapabilities = Capabilities.CapabilitiesFromJsonString(serializedCapability);

            this.Automator.InitializeApp();
            if (this.Automator.ActualCapabilities.AutoLaunch)
            {
                this.Automator.Deployer.Launch();
                this.Automator.ConnectToApp();
            }

            return this.JsonResponse(ResponseStatus.Success, this.Automator.ActualCapabilities);
        }

        #endregion
    }
}
