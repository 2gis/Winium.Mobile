// Library needed to connect to the Windows Phone X Emulator
namespace Winium.StoreApps.Driver.EmulatorHelpers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Phone.Tools.Deploy;
    using Microsoft.SmartDevice.Connectivity.Interface;
    using Microsoft.SmartDevice.MultiTargeting.Connectivity;

    using Winium.StoreApps.Common.Exceptions;

    #endregion

    /// <summary>
    /// App Deploy for 8.1 or greater (uses  Microsoft.Phone.Tools.Deploy shipped with Microsoft SDKs\Windows Phone\v8.1\Tools\AppDeploy)
    /// </summary>
    /// TODO: do not copy Microsoft.Phone.Tools.Deploy assembly on build. Set Copy Local to false and use specified path to assembly.
    public class Deployer
    {
        #region Fields

        private readonly string appPath;

        private readonly ConnectableDevice connectableDevice;

        private readonly DeviceInfo deviceInfo;

        private IAppManifestInfo appManifestInfo;

        private IDevice device;

        private bool installed;

        private IRemoteApplication remoteApplication;

        #endregion

        #region Constructors and Destructors

        public Deployer(string desiredDevice, bool strict, string appPath)
        {
            this.appPath = appPath;

            this.deviceInfo = Devices.Instance.GetMatchingDevice(desiredDevice, strict);

            if (this.deviceInfo == null)
            {
                throw new AutomationException(
                    string.Format(
                        "Could not find a device to launch. You requested '{0}', but the available devices were:\n{1}", 
                        desiredDevice, 
                        Devices.Instance));
            }

            var propertyInfo = this.deviceInfo.GetType().GetTypeInfo().GetDeclaredProperty("DeviceId");
            var deviceId = (string)propertyInfo.GetValue(this.deviceInfo);
            this.connectableDevice =
                new MultiTargetingConnectivity(CultureInfo.CurrentUICulture.LCID).GetConnectableDevice(deviceId);

            Logger.Info("Target emulator: '{0}'", this.DeviceName);
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

        #region Properties

        private IAppManifestInfo AppManifestInfo
        {
            get
            {
                return this.appManifestInfo
                       ?? (this.appManifestInfo = Utils.ReadAppManifestInfoFromPackage(this.appPath));
            }
        }

        private IDevice Device
        {
            get
            {
                return this.device ?? (this.device = this.connectableDevice.Connect(true));
            }
        }

        private IRemoteApplication RemoteApplication
        {
            get
            {
                return this.remoteApplication
                       ?? (this.remoteApplication = this.Device.GetApplication(this.AppManifestInfo.ProductId));
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Install()
        {
            Utils.InstallApplication(this.deviceInfo, this.AppManifestInfo, DeploymentOptions.None, this.appPath);
            this.installed = true;

            Logger.Info("Successfully deployed using Microsoft.Phone.Tools.Deploy");
        }

        public void Launch()
        {
            this.Device.Activate();

            this.RemoteApplication.Launch();
        }

        public void ReceiveFile(string isoStoreRoot, string sourceDeviceFilePath, string targetDesktopFilePath)
        {
            this.RemoteApplication.GetIsolatedStore(isoStoreRoot)
                .ReceiveFile(sourceDeviceFilePath, targetDesktopFilePath, true);
        }

        public void SendFiles(Dictionary<string, string> files)
        {
            if (files == null || !files.Any())
            {
                return;
            }

            var isolatedStore = this.RemoteApplication.GetIsolatedStore("Local");
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

        public void InstallDependencies(List<string> dependencies )
        {
            if (dependencies == null || !dependencies.Any())
            {
                return;
            }

            foreach (var dependency in dependencies)
            {
                InstallDependency(dependency);
            }
        }

        public void InstallDependency(string path)
        {
            var appManifest = Utils.ReadAppManifestInfoFromPackage(path);
            Utils.InstallApplication(this.deviceInfo, appManifest, DeploymentOptions.None, path);
        }

        public void Terminate()
        {
            throw new NotImplementedException("Deployer.Terminate");
        }

        public void Uninstall()
        {
            if (!this.installed)
            {
                Logger.Debug("Could not uninstall application that is already uninstalled.");
                return;
            }

            this.remoteApplication.Uninstall();
            this.remoteApplication = null;

            this.device.Disconnect();
        }

        #endregion
    }
}
