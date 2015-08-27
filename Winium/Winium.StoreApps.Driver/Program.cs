namespace Winium.StoreApps.Driver
{
    #region

    using System;

    using Winium.StoreApps.Driver.CommandHelpers;

    #endregion

    internal class Program
    {
        #region Methods

        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                var options = new CommandLineOptions();
                if (!CommandLine.Parser.Default.ParseArguments(args, options))
                {
                    Environment.Exit(1);
                }

                var appName = typeof(Program).Assembly.GetName().Name;
                var versionInfo = string.Format("{0}, {1}", appName, new BuildInfo());

                if (options.Version)
                {
                    Console.WriteLine(versionInfo);
                    Environment.Exit(0);
                }

                if (options.LogPath != null)
                {
                    Logger.TargetFile(options.LogPath, options.Verbose);
                }
                else
                {
                    Logger.TargetConsole(options.Verbose);
                }

                Logger.Info(versionInfo);

                var listeningPort = options.Port;
                var listener = new Listener(listeningPort);
                Listener.UrnPrefix = options.UrlBase;

                Console.WriteLine("Starting {0} on port {1}\n", appName, listeningPort);

                listener.StartListening();
            }
            catch (Exception ex)
            {
                Logger.Fatal("Failed to start driver: {0}", ex);
                Environment.Exit(ex.HResult);
            }
        }

        #endregion
    }
}
