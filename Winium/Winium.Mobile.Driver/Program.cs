namespace Winium.Mobile.Driver
{
    #region

    using System;
    using System.Collections.Generic;

    using Winium.Mobile.Connectivity.Emulator;
    using Winium.Mobile.Driver.Automator;
    using Winium.Mobile.Driver.CommandHelpers;
    using Winium.Mobile.Logging;

    #endregion

    internal class Program
    {
        #region Static Fields

        private static readonly List<IDisposable> AppLifetimeDisposables = new List<IDisposable>();

        private static Listener listener;

        #endregion

        #region Methods

        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                var options = new CommandLineOptions();
                CommandLine.Parser.Default.ParseArgumentsStrict(args, options);

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

                Capabilities.BoundDeviceName = options.BoundDeviceName;
                Capabilities.DefaultPingTimeout = options.PingTimeout;

                Logger.Info(versionInfo);

                if (!ExitHandler.SetHandler(OnExitHandler))
                {
                    Logger.Warn("Colud not set OnExit cleanup handlers.");
                }

                AppLifetimeDisposables.Add(EmulatorFactory.Instance);

                listener = new Listener(options.Port, options.UrlBase, options.NodeConfig);

                Console.WriteLine("Starting {0} on port {1}\n", appName, listener.Port);

                listener.StartListening();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.ToString());
                Environment.Exit(ex.HResult);
            }
        }

        private static bool OnExitHandler(ExitHandler.CtrlType signal)
        {
            listener.StopListening();
            foreach (var disposable in AppLifetimeDisposables)
            {
                disposable.Dispose();
            }

            return false;
        }

        #endregion
    }
}
