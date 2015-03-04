// Library needed to connect to the Windows Phone X Emulator
namespace WindowsUniversalAppDriver.EmulatorHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Phone.Tools.Deploy;
    using Microsoft.SmartDevice.Connectivity.Interface;
    using Microsoft.SmartDevice.MultiTargeting.Connectivity;

    /// <summary>
    /// App Deploy for 8.1 or greater (uses  Microsoft.Phone.Tools.Deploy shipped with Microsoft SDKs\Windows Phone\v8.1\Tools\AppDeploy)
    /// </summary>
    /// TODO: do not copy Microsoft.Phone.Tools.Deploy assembly on build. Set Copy Local to false and use specified path to assembly.
    public class Deployer81 : IDeployer
    {
        #region Fields

        private readonly DeviceInfo deviceInfo;

        private readonly ConnectableDevice connectableDevice;

        private readonly string appPath;

        private IAppManifestInfo appManifestInfo;

        private IRemoteApplication application;

        #endregion

        #region Constructors and Destructors

        public Deployer81(string desiredDevice, string appPath)
        {
            this.appPath = appPath;

            var devices = Utils.GetDevices();

            this.deviceInfo =
                devices.FirstOrDefault(
                    x =>
                    x.ToString().StartsWith(desiredDevice, StringComparison.OrdinalIgnoreCase)
                    && !x.ToString().Equals("Device"));

            // Exclude device
            if (this.deviceInfo == null)
            {
                Logger.Warn("Desired target {0} not found. Using default instead.", desiredDevice);

                this.deviceInfo = devices.First(x => !x.ToString().Equals("Device"));
            }

            var propertyInfo = deviceInfo.GetType().GetTypeInfo().GetDeclaredProperty("DeviceId");
            var deviceId = (string)propertyInfo.GetValue(deviceInfo);
            this.connectableDevice = new MultiTargetingConnectivity(CultureInfo.CurrentUICulture.LCID).GetConnectableDevice(deviceId);

            Logger.Info("Target emulator: {0}", this.DeviceName);
        }

        #endregion

        #region Public Properties

        public string DeviceName
        {
            get
            {
                return this.deviceInfo.ToString();
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Install()
        {
            this.appManifestInfo = Utils.ReadAppManifestInfoFromPackage(appPath);
            
            Utils.InstallApplication(this.deviceInfo, this.appManifestInfo, DeploymentOptions.None, appPath);

            var device = this.connectableDevice.Connect(true);
            this.application = device.GetApplication(appManifestInfo.ProductId);
            device.Activate();
            
            Logger.Info("Successfully deployed using Microsoft.Phone.Tools.Deploy");
        }

        public void SendFiles(Dictionary<string, string> files)
        {
            if (files == null || !files.Any())
            {
                return;
            }

            var isolatedStore = this.application.GetIsolatedStore("Local");
            foreach (var file in files)
            {
                var phoneDirectoryName = Path.GetDirectoryName(file.Value);
                var phoneFileName = Path.GetFileName(file.Value);
                if (string.IsNullOrEmpty(phoneFileName))
                {
                    phoneFileName = Path.GetFileName(file.Key);
                }

                isolatedStore.SendFile(file.Key, Path.Combine(phoneDirectoryName, phoneFileName), true);
            }
        }

        public void ReciveFiles(Dictionary<string, string> files)
        {
            throw new NotImplementedException("Deployer81.ReciveFiles");
        }

        public void Launch()
        {
            this.application.Launch();
        }

        public void Terminate()
        {
            throw new NotImplementedException("Deployer81.Terminate");
        }

        public void Uninstall()
        {
            if (this.application == null)
            { 
                Logger.Debug("Could not uninstall application that is already uninstalled.");
                return;
            }

            this.application.Uninstall();
            this.application = null;
        }

        #endregion
    }
}
