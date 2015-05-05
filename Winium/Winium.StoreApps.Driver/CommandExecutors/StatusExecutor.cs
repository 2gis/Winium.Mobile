namespace Winium.StoreApps.Driver.CommandExecutors
{
    #region

    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;

    using Newtonsoft.Json;

    using Winium.StoreApps.Common;

    #endregion

    internal class StatusExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var response = new Dictionary<string, object> { { "build", new BuildVersion() }, };
            return this.JsonResponse(ResponseStatus.Success, response);
        }

        #endregion
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", 
        Justification = "Reviewed. Suppression is OK here.")]
    public class BuildVersion
    {
        #region Static Fields

        private static string revision;

        private static string version;

        #endregion

        #region Public Properties

        [JsonProperty("revision")]
        public string Revision
        {
            get
            {
                return revision ?? (revision = ReadGitRevision());
            }
        }

        [JsonProperty("version")]
        public string Version
        {
            get
            {
                return version ?? (version = Assembly.GetExecutingAssembly().GetName().Version.ToString());
            }
        }

        #endregion

        #region Methods

        private static string ReadGitRevision()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = Assembly.GetExecutingAssembly().GetName().Name + ".version.txt";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    return null;
                }

                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadLine();
                }
            }
        }

        #endregion
    }
}
