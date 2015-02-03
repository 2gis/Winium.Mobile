namespace WindowsPhoneDriver.InnerDriver
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows;

    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;

    public class AutomationServer
    {
        #region Static Fields

        public static readonly AutomationServer Instance = new AutomationServer();

        #endregion

        #region Fields

        private Automator automator;

        private bool isServerActive;

        private StreamSocketListener listener;

        private int listeningPort;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Initializes and starts <see cref="AutomationServer"/> on default port (9998) with specified parameters.
        /// </summary>
        /// <remarks>
        /// Use it in conjuction with <see cref="Instance"/> to simplify inclusion of server in tested app.
        /// </remarks>
        /// <param name="visualRoot">
        /// </param>
        public void InitializeAndStart(UIElement visualRoot)
        {
            this.SetAutomator(visualRoot);
            this.Start(9998);
        }

        /// <summary>
        /// Initializes and starts <see cref="AutomationServer"/> with specified parameters.
        /// </summary>
        /// <remarks>
        /// Use it in conjuction with <see cref="Instance"/> to simplify inclusion of server in tested app.
        /// </remarks>
        /// <param name="visualRoot">
        /// </param>
        /// <param name="port">
        /// </param>
        public void InitializeAndStart(UIElement visualRoot, int port)
        {
            this.SetAutomator(visualRoot);
            this.Start(port);
        }

        public void SetAutomator(UIElement visualRoot)
        {
            this.automator = new Automator(visualRoot);
        }

        public async void Start(int port)
        {
            if (this.isServerActive)
            {
                return;
            }

            this.listeningPort = port;

            this.isServerActive = true;
            this.listener = new StreamSocketListener();
            this.listener.Control.QualityOfService = SocketQualityOfService.Normal;
            this.listener.ConnectionReceived += this.ListenerConnectionReceived;
            await this.listener.BindServiceNameAsync(this.listeningPort.ToString(CultureInfo.InvariantCulture));
        }

        public void Stop()
        {
            if (this.isServerActive)
            {
                this.listener.Dispose();
                this.isServerActive = false;
            }
        }

        #endregion

        #region Methods

        private async void HandleRequest(StreamSocket socket)
        {
            var reader = new DataReader(socket.InputStream) { InputStreamOptions = InputStreamOptions.Partial };
            var writer = new DataWriter(socket.OutputStream) { UnicodeEncoding = UnicodeEncoding.Utf8 };

            var acceptedRequest = new AcceptedRequest();
            await acceptedRequest.AcceptRequest(reader);

            string response;
            try
            {
                response = Common.HttpResponseHelper.ResponseString(
                    HttpStatusCode.OK, 
                    this.automator.ProcessCommand(acceptedRequest.Content));
            }
            catch (NotImplementedException ex)
            {
                response = Common.HttpResponseHelper.ResponseString(HttpStatusCode.NotImplemented, ex.Message);
            }

            writer.WriteString(response);
            await writer.StoreAsync();

            socket.Dispose();
        }

        private async void ListenerConnectionReceived(
            StreamSocketListener sender, 
            StreamSocketListenerConnectionReceivedEventArgs args)
        {
            await Task.Run(() => this.HandleRequest(args.Socket));
        }

        #endregion
    }
}
