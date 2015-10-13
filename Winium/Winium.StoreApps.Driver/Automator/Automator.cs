namespace Winium.StoreApps.Driver.Automator
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Threading;

    using Microsoft.Xde.Wmi;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.Common.Exceptions;
    using Winium.StoreApps.Driver.EmulatorHelpers;

    #endregion

    internal class Automator
    {
        #region Static Fields

        private static readonly object LockObject = new object();

        private static volatile Automator instance;

        #endregion

        #region Constructors and Destructors

        #endregion

        #region Public Properties

        public Capabilities ActualCapabilities { get; set; }

        public Requester CommandForwarder { get; set; }

        public Deployer Deployer { get; set; }

        public EmulatorController EmulatorController { get; set; }

        public string InnerIp { get; set; }

        public string Session { get; set; }

        #endregion

        #region Public Methods and Operators

        public static T GetValue<T>(IDictionary<string, JToken> parameters, string key) where T : class
        {
            JToken valueObject;
            if (!parameters.TryGetValue(key, out valueObject))
            {
                return null;
            }

            try
            {
                return valueObject.ToObject<T>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Automator InstanceForSession(string sessionId)
        {
            if (instance == null)
            {
                lock (LockObject)
                {
                    if (instance == null)
                    {
                        // TODO: Add actual support for sessions. Temporary return single Automator for any session
                        instance = new Automator();
                    }
                }
            }

            return instance;
        }

        public void ConnectToApp()
        {
            const int PingThrottling = 100;
            long launchTimeout = this.ActualCapabilities.LaunchTimeout;
            var stopWatch = new Stopwatch();
            var launchTime = new Stopwatch();
            var connected = false;

            launchTime.Start();
            Thread.Sleep(250);
            while (launchTimeout > 0)
            {
                stopWatch.Restart();
                Logger.Trace("Pinging InnerServer");
                connected = this.TryConnectToApp();

                if (connected)
                {
                    break;
                }

                Thread.Sleep(PingThrottling);
                stopWatch.Stop();
                launchTimeout -= stopWatch.ElapsedMilliseconds;
            }

            stopWatch.Stop();
            launchTime.Stop();
            Logger.Debug("Connected in {0} seconds.", launchTime.Elapsed.TotalSeconds);

            if (!connected)
            {
                throw new AutomationException(
                    "Could not connect to the InnerServer.", 
                    ResponseStatus.SessionNotCreatedException);
            }

            // Gives sometime to load visuals (needed only in case of slow emulation)
            Thread.Sleep(this.ActualCapabilities.LaunchDelay);

            // TODO throw AutomationException with SessionNotCreatedException if timeout and uninstall the app
        }

        public void InitializeApp()
        {
            var appPath = this.ActualCapabilities.App;
            var debugDoNotDeploy = this.ActualCapabilities.DebugConnectToRunningApp;

            if (string.IsNullOrWhiteSpace(appPath))
            {
                throw new AutomationException("app capability is not set.", ResponseStatus.SessionNotCreatedException);
            }

            var strictMatchDeviceName = Capabilities.BoundDeviceName != null;
            if (strictMatchDeviceName)
            {
                if (Capabilities.BoundDeviceName.StartsWith(
                    this.ActualCapabilities.DeviceName, 
                    StringComparison.OrdinalIgnoreCase))
                {
                    this.ActualCapabilities.DeviceName = Capabilities.BoundDeviceName;
                }
                else
                {
                    throw new AutomationException(
                        string.Format(
                            "Driver was bound to '{0}' at launch with --bound-device-name option, but another device '{1}' was requested by session.", 
                            Capabilities.BoundDeviceName, 
                            this.ActualCapabilities.DeviceName));
                }
            }

            this.Deployer = new Deployer(this.ActualCapabilities.DeviceName, strictMatchDeviceName, appPath);
            if (!debugDoNotDeploy)
            {
                this.Deployer.InstallDependencies(this.ActualCapabilities.Dependencies);
                this.Deployer.Install();
                this.Deployer.SendFiles(this.ActualCapabilities.Files);
            }

            if (!this.ActualCapabilities.DeviceName.Equals(this.Deployer.DeviceName, StringComparison.OrdinalIgnoreCase))
            {
                Logger.Warn(
                    "Device was found using partail deviceName '{0}',"
                    + " this behavior might be deprecated in favor of specifying strict deviceName or platformVersion (when implemented).", 
                    this.ActualCapabilities.DeviceName);
            }

            this.ActualCapabilities.DeviceName = this.Deployer.DeviceName;
            this.EmulatorController = this.CreateEmulatorController(debugDoNotDeploy);

            this.InnerIp = this.EmulatorController.GetIpAddress();
        }

        public Point? RequestElementLocation(JToken element)
        {
            var command = new Command(
                DriverCommand.GetElementLocationOnceScrolledIntoView, 
                new Dictionary<string, JToken> { { "ID", element } });

            var responseBody = this.CommandForwarder.ForwardCommand(command);

            var deserializeObject = JsonConvert.DeserializeObject<JsonResponse>(responseBody);

            if (deserializeObject.Status != ResponseStatus.Success)
            {
                return null;
            }

            var locationObject = deserializeObject.Value as JObject;
            if (locationObject == null)
            {
                return null;
            }

            var location = locationObject.ToObject<Dictionary<string, int>>();

            if (location == null)
            {
                return null;
            }

            var x = location["x"];
            var y = location["y"];
            return new Point(x, y);
        }

        #endregion

        #region Methods

        private EmulatorController CreateEmulatorController(bool withFallback)
        {
            try
            {
                return new EmulatorController(this.ActualCapabilities.DeviceName);
            }
            catch (XdeVirtualMachineException)
            {
                if (!withFallback)
                {
                    throw;
                }

                this.ActualCapabilities.DeviceName = this.ActualCapabilities.DeviceName.Split('(')[0];
                return new EmulatorController(this.ActualCapabilities.DeviceName);
            }
        }

        private ConnectionInformation GetConnectionInformation()
        {
            var filePath = Path.GetTempFileName();
            this.Deployer.ReceiveFile("Temp", ConnectionInformation.FileName, filePath);
            using (var file = File.OpenText(filePath))
            {
                var serializer = new JsonSerializer();
                var connectionInformation =
                    (ConnectionInformation)serializer.Deserialize(file, typeof(ConnectionInformation));
                return connectionInformation;
            }
        }

        private bool TryConnectToApp()
        {
            var usingFallbackPort = false;
            const string FallbackPort = "9998";
            ConnectionInformation connectionInformation;
            try
            {
                connectionInformation = this.GetConnectionInformation();
                if (connectionInformation == null)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                // TODO Limit catch clause from broad Exception, to specific ones
                // We expect FileNotFound exception, but in rare cases other exceptions can be thrown by SmartDevice API
                usingFallbackPort = true;
                connectionInformation = new ConnectionInformation { RemotePort = FallbackPort };
            }

            this.CommandForwarder = new Requester(this.InnerIp, connectionInformation.RemotePort);
            var pingCommand = new Command("ping");
            var responseBody = this.CommandForwarder.ForwardCommand(pingCommand, false, 1500);
            if (responseBody.StartsWith("<pong>", StringComparison.Ordinal))
            {
                Logger.Info("Received connection information from device {0}.", connectionInformation);
                if (usingFallbackPort)
                {
                    Logger.Warn(
                        "DEPRICATION: ConnectionInformation file was not found."
                        + " Make sure that you are using same versions of Driver and InnerServer."
                        + " Fallback to standard innerPort == {0}. This will be depricated in later versions.", 
                        FallbackPort);
                }

                return true;
            }

            return false;
        }

        #endregion
    }
}
