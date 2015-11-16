// Type: Microsoft.Phone.Tools.Deploy.DeviceInfo
// Assembly: Microsoft.Phone.Tools.Deploy, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86EA135B-1DD4-4D0E-BB40-E685CB8C02D7
// Assembly location: C:\Program Files (x86)\Microsoft SDKs\Windows Phone\v8.1\Tools\AppDeploy\Microsoft.Phone.Tools.Deploy.dll
namespace Microsoft.Phone.Tools.Deploy.Patched
{
    public class DeviceInfo
    {
        #region Constructors and Destructors

        internal DeviceInfo(string deviceid, string deviceName)
        {
            this.DeviceId = deviceid;
            this.DeviceName = deviceName;
        }

        #endregion

        #region Properties

        internal string DeviceId { get; private set; }

        internal string DeviceName { get; private set; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return this.DeviceName;
        }

        #endregion
    }
}
