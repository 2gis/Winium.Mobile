namespace WindowsUniversalAppDriver.EmulatorHelpers
{
    using System.Collections.Generic;

    internal interface IDeployer
    {
        #region Public Properties

        string DeviceName { get; }

        #endregion

        #region Public Methods and Operators

        void Install();

        void SendFiles(Dictionary<string, string> files);

        void ReciveFiles(Dictionary<string, string> files);

        void Launch();

        void Terminate();

        void Uninstall();

        #endregion
    }
}
