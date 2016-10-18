namespace Winium.Mobile.Driver.CommandHelpers
{
    #region

    using System.IO;
    using System.Reflection;

    using Newtonsoft.Json;

    #endregion

    public class BuildInfo
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

        #region Public Methods and Operators

        public override string ToString()
        {
            return string.Format("Version: {0}, Revision: {1}", this.Version, this.Revision);
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
