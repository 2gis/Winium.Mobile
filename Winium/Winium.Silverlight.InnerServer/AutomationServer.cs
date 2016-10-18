namespace Winium.Silverlight.InnerServer
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows;

    using Windows.Networking.Sockets;
    using Windows.Storage;
    using Windows.Storage.Streams;

    using Newtonsoft.Json;

    using Winium.Mobile.Common;

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

        #region Public Methods and Operators

        public string Port
        {
            get
            {
                return this.listener.Information.LocalPort;
            }
        }

        /// <summary>
        /// Initializes and starts <see cref="AutomationServer"/> with specified parameters.
        /// </summary>
        /// <remarks>
        /// Use it in conjuction with <see cref="Instance"/> to simplify inclusion of server in tested app.
        /// </remarks>
        /// <param name="visualRoot">
        /// </param>
        public void InitializeAndStart(UIElement visualRoot)
        {
            this.SetAutomator(visualRoot);
            this.Start();
        }

        public void SetAutomator(UIElement visualRoot)
        {
            this.automator = new Automator(visualRoot);
        }

        public async void Start()
        {
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

        private async Task WriteConnectionData()
        {
            var file = await ApplicationData.Current.TemporaryFolder.CreateFileAsync(ConnectionInformation.FileName, CreationCollisionOption.ReplaceExisting);

            var information = new ConnectionInformation { RemotePort = this.listener.Information.LocalPort };
            await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(information));
        }

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
