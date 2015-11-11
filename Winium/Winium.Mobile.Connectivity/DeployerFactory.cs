namespace Winium.Mobile.Connectivity
{
    #region

    using System.IO;

    #endregion

    public static class DeployerFactory
    {
        #region Public Methods and Operators

        public static IDeployer DeployerForPackage(FileInfo package, string desiredDevice, bool strict)
        {
            return new Deployer(desiredDevice, strict);
        }

        #endregion
    }
}
