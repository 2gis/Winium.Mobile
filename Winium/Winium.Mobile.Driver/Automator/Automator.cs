namespace Winium.Mobile.Driver.Automator
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Winium.Mobile.Connectivity;
    using Winium.Mobile.Connectivity.Emulator;
    using Winium.Mobile.Common;
    using Winium.Mobile.Common.Exceptions;
    using Winium.Mobile.Driver.CommandHelpers;
    using Winium.Mobile.Logging;

    #endregion

    internal class Automator
    {
        #region Static Fields

        private static readonly object LockObject = new object();

        private static volatile Automator instance;

        #endregion

        #region Public Properties

        public Capabilities ActualCapabilities { get; set; }

        public Requester CommandForwarder { get; set; }

        public IDeployer Deployer { get; set; }

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
                connected = this.TryConnectToApp(this.ActualCapabilities.PingTimeout);

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

        public void Deploy()
        {
            var appPath = this.ActualCapabilities.App;
            var debugDoNotDeploy = this.ActualCapabilities.DebugConnectToRunningApp;

            if (string.IsNullOrWhiteSpace(appPath))
            {
                throw new AutomationException("app capability is not set.", ResponseStatus.SessionNotCreatedException);
            }

            this.InitializeDeployer();
            this.ActualCapabilities.DeviceName = this.Deployer.DeviceName;

            this.InitializeApplication(appPath);
            
            this.EmulatorController = this.CreateEmulatorController(debugDoNotDeploy);

            this.InnerIp = this.EmulatorController.GetIpAddress();

            this.LaunchAppIfNeeded();
        }

        private void LaunchAppIfNeeded()
        {
            if (!this.ActualCapabilities.AutoLaunch)
            {
                return;
            }

            this.Deployer.Launch();
            this.ConnectToApp();
        }

        private void InitializeApplication(string appPath)
        {
            if (this.ActualCapabilities.DebugConnectToRunningApp)
            {
                this.Deployer.UsePreInstalledApplication(appPath);
            }
            else
            {
                this.Deployer.Install(appPath, this.ActualCapabilities.Dependencies);
                var expandedFiles = new FilesCapabilityExpander().ExpandFiles(this.ActualCapabilities.Files).ToList();
                this.Deployer.SendFiles(expandedFiles);
            }
        }

        private void InitializeDeployer()
        {
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

            var appFileInfo = new FileInfo(this.ActualCapabilities.App);
            this.Deployer = DeployerFactory.DeployerForPackage(
                appFileInfo,
                this.ActualCapabilities.DeviceName,
                strictMatchDeviceName);

            if (!this.ActualCapabilities.DeviceName.Equals(this.Deployer.DeviceName, StringComparison.OrdinalIgnoreCase))
            {
                Logger.Warn(
                    "Device was found using partail deviceName '{0}',"
                    + " this behavior might be deprecated in favor of specifying strict deviceName or platformVersion (when implemented).",
                    this.ActualCapabilities.DeviceName);
            }
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
            catch (VirtualMachineException)
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

        private bool TryConnectToApp(int timeout)
        {
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
                Logger.Warn("ConnectionInformation ({0}) was not found. Make sure that you are using same versions of Winium.Mobile.Driver and Winium.*.InnerServer.", ConnectionInformation.FileName);

                return false;
            }

            var port = Convert.ToInt32(connectionInformation.RemotePort);
            this.CommandForwarder = new Requester(this.InnerIp, port);
            var pingCommand = new Command("ping");

            var responseBody = this.CommandForwarder.ForwardCommand(pingCommand, false, timeout);
            if (!responseBody.StartsWith("<pong>", StringComparison.Ordinal))
            {
                return false;
            }

            Logger.Info("Received connection information from device {0}.", connectionInformation);
            
            return true;
        }

        #endregion
    }
}
