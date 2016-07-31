namespace Winium.StoreApps.Driver.CommandExecutors
{
    using System;
    using System.IO;

    using Winium.StoreApps.Driver.Automator;

    internal class PushFileExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var path = Automator.GetValue<string>(this.ExecutedCommand.Parameters, "path");
            var data = Automator.GetValue<string>(this.ExecutedCommand.Parameters, "data");

            var localPath = Path.GetTempFileName();

            try
            {
                File.WriteAllBytes(localPath, Convert.FromBase64String(data));
                this.Automator.Deployer.SendFile("Local", localPath, path);
            }
            finally
            {
                File.Delete(localPath);
            }

            return this.JsonResponse();
        }

        #endregion
    }
}