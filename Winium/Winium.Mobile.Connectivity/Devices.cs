namespace Winium.Mobile.Connectivity
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Phone.Tools.Deploy.Patched;

    #endregion

    public class Devices
    {
        #region Static Fields

        private static readonly Lazy<Devices> LazyInstance = new Lazy<Devices>(() => new Devices());

        #endregion

        #region Constructors and Destructors

        private Devices()
        {
            this.AvailableDevices = Utils.GetDevices().Where(x => !x.ToString().Equals("Device")).ToList();
        }

        #endregion

        #region Public Properties

        public static Devices Instance
        {
            get
            {
                return LazyInstance.Value;
            }
        }

        #endregion

        #region Properties

        private IEnumerable<DeviceInfo> AvailableDevices { get; set; }

        #endregion

        #region Public Methods and Operators

        public DeviceInfo GetMatchingDevice(string desiredName, bool strict)
        {
            DeviceInfo deviceInfo;
            if (strict)
            {
                deviceInfo =
                    this.AvailableDevices.FirstOrDefault(
                        x => x.ToString().Equals(desiredName, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                deviceInfo =
                    this.AvailableDevices.FirstOrDefault(
                        x => x.ToString().StartsWith(desiredName, StringComparison.OrdinalIgnoreCase));
            }

            return deviceInfo;
        }

        public bool IsValidDeviceName(string desiredName)
        {
            var deviceInfo = this.GetMatchingDevice(desiredName, true);
            return deviceInfo != null;
        }

        public override string ToString()
        {
            return string.Join("\n", this.AvailableDevices.Select(x => x.ToString()));
        }

        #endregion
    }
}
