namespace Winium.StoreApps.Driver
{
    #region

    using CommandLine;
    using CommandLine.Text;

    #endregion

    internal class CommandLineOptions
    {
        #region Public Properties

        [Option("log-path", Required = false, 
            HelpText = "write server log to file instead of stdout, increases log level to INFO")]
        public string LogPath { get; set; }

        [Option("port", Required = false, DefaultValue = 9999, HelpText = "port to listen on")]
        public int Port { get; set; }

        [Option("url-base", Required = false, HelpText = "base URL path prefix for commands, e.g. wd/url")]
        public string UrlBase { get; set; }

        [Option("verbose", Required = false, HelpText = "log verbosely")]
        public bool Verbose { get; set; }

        [Option("version", Required = false, HelpText = "print version number and exit")]
        public bool Version { get; set; }

        [Option("bound-device-name", Required = false, HelpText = "strict name of emulator to bind with driver. Driver will be able to start sessions only on this device, if session will specify deviceName that is not a substring of bound device name, then an error will occur. Use this option to run tests in parallel on differen driver-emulator pairs connected to Selenium Grid on same host.")]
        public string BoundDeviceName { get; set; }

        [Option("nodeconfig", Required = false, HelpText = "configuration JSON file to register driver with selenium grid")]
        public string NodeConfig { get; set; }

        [Option("dependency", Required = false, HelpText = "dependencies to be installed before main app")]
        public string Dependency { get; set; }

        #endregion

        #region Public Methods and Operators

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, current => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        #endregion
    }
}
