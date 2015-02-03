// Library needed to connect to the Windows Phone X Emulator
namespace WindowsPhoneDriver.OuterDriver.EmulatorHelpers
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Phone.Tools.Deploy;

    /// <summary>
    /// App Deploy for 8.1 or greater (uses  Microsoft.Phone.Tools.Deploy shipped with Microsoft SDKs\Windows Phone\v8.1\Tools\AppDeploy)
    /// </summary>
    /// TODO: do not copy Microsoft.Phone.Tools.Deploy assembly on build. Set Copy Local to false and use specified path to assembly.
    public class Deployer81 : IDeployer
    {
        #region Fields

        private readonly DeviceInfo deviceInfo;

        private IAppManifestInfo appManifestInfo;

        #endregion

        #region Constructors and Destructors

        public Deployer81(string desiredDevice)
        {
            var devices = Utils.GetDevices();

            this.deviceInfo =
                devices.FirstOrDefault(x => x.ToString().StartsWith(desiredDevice, StringComparison.OrdinalIgnoreCase) && !x.ToString().Equals("Device"));

            // Exclude device
            if (this.deviceInfo == null)
            {
                Logger.Warn("Desired target {0} not found. Using default instead.", desiredDevice);

                this.deviceInfo = devices.First(x => !x.ToString().Equals("Device"));
            }

            Logger.Info("Target emulator: {0}", this.DeviceName);
        }

        #endregion

        #region Public Properties

        public string DeviceName
        {
            get
            {
                return this.deviceInfo != null ? this.deviceInfo.ToString() : string.Empty;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Deploy(string appPath)
        {
            this.appManifestInfo = Utils.ReadAppManifestInfoFromPackage(appPath);

            GlobalOptions.LaunchAfterInstall = true;
            Utils.InstallApplication(this.deviceInfo, this.appManifestInfo, DeploymentOptions.None, appPath);

            Logger.Info("Successfully deployed using Microsoft.Phone.Tools.Deploy");
        }

        public void Disconnect()
        {
            // FIXME Temporary solution using private UninstallApplication method
            // Still using Microsoft.Phone.Tools.Deploy assembly is much easier than Smart Device connectivity
            var uninstallApplication = typeof(Utils).GetMethod(
                "UninstallApplication", 
                BindingFlags.NonPublic | BindingFlags.Static);
            uninstallApplication.Invoke(typeof(Utils), new object[] { this.deviceInfo, this.appManifestInfo.ProductId });
        }

        #endregion
    }
}
