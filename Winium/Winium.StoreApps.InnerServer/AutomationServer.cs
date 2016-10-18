namespace Winium.StoreApps.InnerServer
{
    #region

    using System;
    using System.Net;
    using System.Threading.Tasks;

    using Newtonsoft.Json;

    using Windows.Networking.Sockets;
    using Windows.Storage;
    using Windows.Storage.Streams;

    using Winium.Mobile.Common;

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

        #endregion

        #region Public Properties

        public string Port
        {
            get
            {
                return this.listener.Information.LocalPort;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Initialize <see cref="AutomationServer"/>.
        /// This method must be called on UI thread.
        /// </summary>
        public void Initialize()
        {
            this.automator = new Automator();
        }

        /// <summary>
        /// Initialize and starts <see cref="AutomationServer"/> with specified parameters.
        /// </summary>
        /// <remarks>
        /// Use it in conjuction with <see cref="Instance"/> to simplify inclusion of server in tested app.
        /// </remarks>
        public void InitializeAndStart()
        {
            // TODO Consider adding ability to specify InnerServer port and turn off dynamic port
            this.Initialize();
            this.Start();
        }

        /// <summary>
        /// Start <see cref="AutomationServer"/> on specified port.
        /// </summary>
        public async void Start()
        {
            if (this.automator == null)
            {
                throw new InvalidOperationException("Initialize must be called before starting the server.");
            }

            if (this.isServerActive)
            {
                return;
            }

            this.listener = new StreamSocketListener();
            this.listener.Control.QualityOfService = SocketQualityOfService.Normal;
            this.listener.ConnectionReceived += this.ListenerConnectionReceived;
            await this.listener.BindServiceNameAsync(string.Empty);
            await this.WriteConnectionData();
            this.isServerActive = true;
        }

        /// <summary>
        /// Stop <see cref="AutomationServer"/>.
        /// </summary>
        public void Stop()
        {
            if (!this.isServerActive)
            {
                return;
            }

            this.listener.Dispose();
            this.isServerActive = false;
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
            catch (NotImplementedException exception)
            {
                response = HttpResponseHelper.ResponseString(HttpStatusCode.NotImplemented, exception.Message);
            }
            catch (Exception exception)
            {
                response = HttpResponseHelper.ResponseString(HttpStatusCode.InternalServerError, exception.Message);
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

        private async Task WriteConnectionData()
        {
            var file =
                await
                ApplicationData.Current.TemporaryFolder.CreateFileAsync(
                    ConnectionInformation.FileName, 
                    CreationCollisionOption.ReplaceExisting);

            var information = new ConnectionInformation { RemotePort = this.listener.Information.LocalPort };
            await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(information));
        }

        #endregion
    }
}
