namespace Winium.StoreApps.Driver.EmulatorHelpers
{
    #region

    using System.Collections.Generic;

    #endregion

    internal interface IDeployer
    {
        #region Public Properties

        string DeviceName { get; }

        #endregion

        #region Public Methods and Operators

        void Install();

        void Launch();

        void ReceiveFile(string isoStoreRoot, string sourceDeviceFilePath, string targetDesktopFilePath);

        void SendFiles(Dictionary<string, string> files);

        void Terminate();

        void Uninstall();

        #endregion
    }
}
