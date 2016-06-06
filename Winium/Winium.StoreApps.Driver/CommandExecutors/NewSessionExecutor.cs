﻿namespace Winium.StoreApps.Driver.CommandExecutors
{
    #region

    using System;

    using Newtonsoft.Json;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.Common.Exceptions;
    using Winium.StoreApps.Driver.Automator;

    #endregion

    internal class NewSessionExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            try
            {
                this.Automator.Session = Guid.NewGuid().ToString();

                // It is easier to reparse desired capabilities as JSON instead of re-mapping keys to attributes and calling type conversions, 
                // so we will take possible one time performance hit by serializing Dictionary and deserializing it as Capabilities object
                var serializedCapability =
                    JsonConvert.SerializeObject(this.ExecutedCommand.Parameters["desiredCapabilities"]);
                this.Automator.ActualCapabilities = Capabilities.CapabilitiesFromJsonString(serializedCapability);

                this.Automator.Deploy();
                

                return this.JsonResponse(ResponseStatus.Success, this.Automator.ActualCapabilities);
            }
            catch (Exception ex)
            {
                throw new AutomationException(ex.Message, ex, ResponseStatus.SessionNotCreatedException);
            }
        }

        #endregion
    }
}
