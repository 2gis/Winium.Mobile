namespace Winium.Mobile.Connectivity
{
    #region

    using System.Collections.Generic;

    #endregion

    public interface IDeployer
    {
        #region Public Properties

        string DeviceName { get; }

        AppType AppType { get; }

        #endregion

        #region Public Methods and Operators

        void Install(string appPath, List<string> dependencies);

        void UsePreInstalledApplication(string appPath);

        void Launch();

        void ReceiveFile(string isoStoreRoot, string sourceDeviceFilePath, string targetDesktopFilePath);

        void SendFiles(List<KeyValuePair<string, string>> files);

        void SendFile(string isoStoreRoot, string sourceDesktopFilePath, string targetDeviceFilePath);

        void Terminate();

        void Uninstall();

        #endregion
    }
}
