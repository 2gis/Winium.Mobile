namespace Winium.StoreApps.Driver.CommandExecutors
{
    #region

    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;

    using Microsoft.Xde.Wmi;

    using Newtonsoft.Json;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.Common.Exceptions;
    using Winium.StoreApps.Driver.Automator;
    using Winium.StoreApps.Driver.EmulatorHelpers;

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

            var innerIp = this.InitializeApplication(this.Automator.ActualCapabilities.DebugConnectToRunningApp);

            var connectionInformation = this.GetConnectionInformation();
            Logger.Info("Received connection information from device {0}", connectionInformation);

            this.Automator.CommandForwarder = new Requester(innerIp, connectionInformation.RemotePort);

            long timeout = this.Automator.ActualCapabilities.LaunchTimeout;

            const int PingStep = 500;
            var stopWatch = new Stopwatch();
            while (timeout > 0)
            {
                stopWatch.Restart();

                Logger.Trace("Ping inner driver");
                var pingCommand = new Command("ping");
                var responseBody = this.Automator.CommandForwarder.ForwardCommand(pingCommand, false, 2000);
                if (responseBody.StartsWith("<pong>", StringComparison.Ordinal))
                {
                    break;
                }

                Thread.Sleep(PingStep);
                stopWatch.Stop();
                timeout -= stopWatch.ElapsedMilliseconds;
            }

            // TODO throw AutomationException with SessionNotCreatedException if timeout and uninstall the app
            Console.WriteLine();

            // Gives sometime to load visuals (needed only in case of slow emulation)
            Thread.Sleep(this.Automator.ActualCapabilities.LaunchDelay);

            return this.JsonResponse(ResponseStatus.Success, this.Automator.ActualCapabilities);
        }

        private ConnectionInformation GetConnectionInformation()
        {
            var filePath = Path.GetTempFileName();
            this.Automator.Deployer.ReceiveFile("Temp", ConnectionInformation.FileName, filePath);
            using (var file = File.OpenText(filePath))
            {
                var serializer = new JsonSerializer();
                return (ConnectionInformation)serializer.Deserialize(file, typeof(ConnectionInformation));
            }
        }

        private EmulatorController CreateEmulatorController(bool withFallback)
        {
            try
            {
                return new EmulatorController(this.Automator.ActualCapabilities.DeviceName);
            }
            catch (XdeVirtualMachineException)
            {
                if (!withFallback)
                {
                    throw;
                }

                this.Automator.ActualCapabilities.DeviceName =
                    this.Automator.ActualCapabilities.DeviceName.Split('(')[0];
                return new EmulatorController(this.Automator.ActualCapabilities.DeviceName);
            }
        }

        private string InitializeApplication(bool debugDoNotDeploy = false)
        {
            var appPath = this.Automator.ActualCapabilities.App;

            if (string.IsNullOrWhiteSpace(appPath))
            {
                throw new AutomationException("app capability is not set.", ResponseStatus.SessionNotCreatedException);
            }

            this.Automator.Deployer = new Deployer81(this.Automator.ActualCapabilities.DeviceName, appPath);
            if (!debugDoNotDeploy)
            {
                this.Automator.Deployer.Install();
                this.Automator.Deployer.SendFiles(this.Automator.ActualCapabilities.Files);
                this.Automator.Deployer.Launch();
            }

            this.Automator.ActualCapabilities.DeviceName = this.Automator.Deployer.DeviceName;
            var emulatorController = this.CreateEmulatorController(debugDoNotDeploy);
            this.Automator.EmulatorController = emulatorController;

            return this.Automator.EmulatorController.GetIpAddress();
        }

        #endregion
    }
}
