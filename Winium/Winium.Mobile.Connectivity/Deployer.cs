﻿// Library needed to connect to the Windows Phone X Emulator
namespace Winium.Mobile.Connectivity
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
    using Winium.StoreApps.Logging;

    using DeviceInfo = Microsoft.Phone.Tools.Deploy.Patched.DeviceInfo;
    using Utils = Microsoft.Phone.Tools.Deploy.Patched.Utils;

    #endregion

    /// <summary>
    /// App Deploy for 8.1 or greater (uses  Microsoft.Phone.Tools.Deploy shipped with Microsoft SDKs\Windows Phone\v8.1\Tools\AppDeploy)
    /// </summary>
    /// TODO: do not copy Microsoft.Phone.Tools.Deploy assembly on build. Set Copy Local to false and use specified path to assembly.
    public class Deployer : IDeployer
    {
        #region Constants

        private const int MaxRetries = 3;

        #endregion

        #region Fields

        private readonly DeviceInfo deviceInfo;

        private bool installed;

        #endregion

        #region Constructors and Destructors

        public Deployer(string desiredDevice, bool strict)
        {
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
            var connectableDevice =
                new MultiTargetingConnectivity(CultureInfo.CurrentUICulture.LCID).GetConnectableDevice(deviceId);
            this.Device = connectableDevice.Connect(true);
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

        private IDevice Device { get; set; }

        private IRemoteApplication RemoteApplication { get; set; }

        #endregion

        #region Public Methods and Operators

        public void Install(string appPath, List<string> dependencies)
        {
            this.InstallDependencies(dependencies);
            this.InstallApp(appPath);
        }

        public void UsePreInstalledApplication(string appPath)
        {
            var appManifest = Utils.ReadAppManifestInfoFromPackage(appPath);
            this.RemoteApplication = this.Device.GetApplication(appManifest.ProductId);
        }

        public void Launch()
        {
            this.Device.Activate();
            Retry.WithRetry(() => this.RemoteApplication.Launch(), MaxRetries);
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

            this.RemoteApplication.Uninstall();
            this.RemoteApplication = null;

            this.Device.Disconnect();
        }

        #endregion

        #region Methods

        private void InstallApp(string appPath)
        {
            var appManifestInfo = this.InstallApplicationPackage(appPath);
            this.installed = true;
            this.RemoteApplication = this.Device.GetApplication(appManifestInfo.ProductId);
        }

        private IAppManifestInfo InstallApplicationPackage(string path)
        {
            var appManifest = Utils.ReadAppManifestInfoFromPackage(path);
            Utils.InstallApplication(this.deviceInfo, appManifest, DeploymentOptions.None, path);

            Logger.Info("{0} was successfully deployed using Microsoft.Phone.Tools.Deploy", appManifest.Name);
            return appManifest;
        }

        private void InstallDependencies(List<string> dependencies)
        {
            if (dependencies == null || !dependencies.Any())
            {
                return;
            }

            foreach (var dependency in dependencies)
            {
                this.InstallApplicationPackage(dependency);
            }
        }

        #endregion
    }
}
