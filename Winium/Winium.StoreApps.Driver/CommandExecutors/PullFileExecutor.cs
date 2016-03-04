namespace Winium.StoreApps.Driver.CommandExecutors
{
    using System;
    using System.IO;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.Driver.Automator;

    internal class PullFileExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var path = Automator.GetValue<string>(this.ExecutedCommand.Parameters, "path");

            var localPath = Path.GetTempFileName();

            try
            {
                this.Automator.Deployer.ReceiveFile("Local", path, localPath);

                var bytes = File.ReadAllBytes(localPath);
                var data = Convert.ToBase64String(bytes);
                return this.JsonResponse(ResponseStatus.Success, data);
            }
            finally
            {
                File.Delete(localPath);
            }
        }

        #endregion
    }
}