using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Winium.StoreApps.Common.Exceptions;

namespace Winium.StoreApps.Driver.CommandExecutors
{
    using System;
    using System.Threading;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.Driver.Automator;

    internal class ReadLocalTextFileExecutor : CommandExecutorBase
    {
        #region Public Methods and Operators

        public static string ReadFile(Automator automator, Command command)
        {
            var args = command.Parameters["args"] as JArray;
            if (args == null || args.Count == 0) {
                throw new AutomationException("Missing file name", ResponseStatus.UnknownCommand);
            }

            var filename = args[0].ToString();
            var filePath = Path.GetTempFileName();
            automator.Deployer.ReceiveFile("Local", filename, filePath);
            using (var file = File.OpenText(filePath)) {
                return file.ReadToEnd();
            }
        }

        #endregion

        #region Methods

        protected override string DoImpl()
        {
            var fileContent = ReadFile(this.Automator, this.ExecutedCommand);
            return this.JsonResponse(ResponseStatus.Success, fileContent);
        }

        #endregion
    }
}
