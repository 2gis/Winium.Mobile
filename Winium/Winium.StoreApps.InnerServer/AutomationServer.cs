namespace Winium.StoreApps.InnerServer
{
    #region

    using System;
    using System.Globalization;
    using System.Net;
    using System.Threading.Tasks;

    using Windows.Networking.Sockets;
    using Windows.Storage.Streams;

    using Winium.StoreApps.Common;

    #endregion

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
        /// Initialize and starts <see cref="AutomationServer"/> with specified parameters.
        /// </summary>
        /// <remarks>
        /// Use it in conjuction with <see cref="Instance"/> to simplify inclusion of server in tested app.
        /// </remarks>
        public void InitializeAndStart()
        {
            this.Initialize();
            this.Start(9998);
        }

        /// <summary>
        /// Initialize and starts <see cref="AutomationServer"/> on default port.
        /// </summary>
        /// <remarks>
        /// Use it in conjuction with <see cref="Instance"/> to simplify inclusion of server in tested app.
        /// </remarks>
        /// <param name="port">
        /// </param>
        public void InitializeAndStart(int port)
        {
            this.Initialize();
            this.Start(port);
        }

        /// <summary>
        /// Initialize <see cref="AutomationServer"/>.
        /// This method must be called on UI thread.
        /// </summary>
        public void Initialize()
        {
            this.automator = new Automator();
        }

        /// <summary>
        /// Start <see cref="AutomationServer"/> on specified port.
        /// </summary>
        /// <param name="port"></param>
        public async void Start(int port)
        {
            if (this.automator == null)
            {
                throw new InvalidOperationException("Initialize must be called before starting the server.");
            }

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

        /// <summary>
        /// Stop <see cref="AutomationServer"/>.
        /// </summary>
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
                response = HttpResponseHelper.ResponseString(
                    HttpStatusCode.OK, 
                    this.automator.ProcessCommand(acceptedRequest.Content));
            }
            catch (NotImplementedException ex)
            {
                response = HttpResponseHelper.ResponseString(HttpStatusCode.NotImplemented, ex.Message);
            }

            writer.WriteString(response);
            await writer.StoreAsync();

            socket.Dispose();

            if (this.automator.DoAfterResponseOnce == null)
            {
                return;
            }

            var localDoAfterResponseOnce = this.automator.DoAfterResponseOnce;
            this.automator.DoAfterResponseOnce = null;

            localDoAfterResponseOnce();
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
