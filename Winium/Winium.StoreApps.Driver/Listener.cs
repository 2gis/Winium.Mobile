﻿namespace Winium.StoreApps.Driver
{
    #region

    using System;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.Logging;

    #endregion

    public class Listener
    {
        #region Fields

        private readonly Uri baseAddress;

        private readonly NodeRegistrar nodeRegistrar;

        private UriDispatchTables dispatcher;

        private CommandExecutorDispatchTable executorDispatcher;

        private TcpListener listener;

        #endregion

        #region Constructors and Destructors

        public Listener(int listenerPort, string urlBase, string nodeConfigFile)
        {
            urlBase = NormalizePrefix(urlBase);
            this.Port = listenerPort;

            if (!string.IsNullOrWhiteSpace(nodeConfigFile))
            {
                if (!urlBase.Equals("wd/hub"))
                {
                    Logger.Warn(
                        "--url-base '{0}' will be overriden and set to 'wd/hub' because --nodeconfig option was specified", 
                        urlBase);
                }

                urlBase = "wd/hub";

                this.nodeRegistrar = new NodeRegistrar(nodeConfigFile, "localhost", this.Port);
            }

            this.baseAddress = new UriBuilder("http", "localhost", this.Port, urlBase).Uri;
        }

        #endregion

        #region Public Properties

        public int Port { get; private set; }

        #endregion

        #region Public Methods and Operators

        public void StartListening()
        {
            try
            {
                this.listener = new TcpListener(IPAddress.Any, this.Port);
                this.dispatcher = new UriDispatchTables(this.baseAddress);
                this.executorDispatcher = new CommandExecutorDispatchTable();

                // Start listening for client requests.
                this.listener.Start();
                Logger.Info("RemoteWebDriver instances should connect to: {0}", this.baseAddress);

                if (this.nodeRegistrar != null)
                {
                    this.nodeRegistrar.Register();
                }

                // Enter the listening loop
                while (true)
                {
                    Logger.Debug("Waiting for a connection...");

                    // Perform a blocking call to accept requests. 
                    var client = this.listener.AcceptTcpClient();

                    // Get a stream object for reading and writing
                    using (var stream = client.GetStream())
                    {
                        var acceptedRequest = HttpRequest.ReadFromStreamWithoutClosing(stream);
                        Logger.Debug("ACCEPTED REQUEST {0}", acceptedRequest.StartingLine);

                        var response = this.HandleRequest(acceptedRequest);
                        using (var writer = new StreamWriter(stream))
                        {
                            try
                            {
                                writer.Write(response);
                                writer.Flush();
                            }
                            catch (IOException ex)
                            {
                                Logger.Error("Error occured while writing response: {0}", ex);
                            }
                        }

                        // Shutdown and end connection
                    }

                    client.Close();
                    Logger.Debug("Client closed\n");
                }
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode != SocketError.Interrupted)
                {
                    throw;
                }
                else
                {
                    Logger.Debug(ex.ToString());
                }
            }
            finally
            {
                // Stop listening for new clients.
                this.listener.Stop();
            }
        }

        public void StopListening()
        {
            this.listener.Stop();
        }

        #endregion

        #region Methods

        private static string NormalizePrefix(string prefix)
        {
            return string.IsNullOrWhiteSpace(prefix) ? string.Empty : prefix.Trim('/');
        }

        private string HandleRequest(HttpRequest acceptedRequest)
        {
            var firstHeaderTokens = acceptedRequest.StartingLine.Split(' ');
            var method = firstHeaderTokens[0];
            var resourcePath = firstHeaderTokens[1];

            var uriToMatch = new Uri(this.baseAddress, resourcePath);
            var matched = this.dispatcher.Match(method, uriToMatch);

            if (matched == null)
            {
                Logger.Warn("Unknown command recived: {0}", uriToMatch);
                return HttpResponseHelper.ResponseString(HttpStatusCode.NotFound, "Unknown command " + uriToMatch);
            }

            var commandName = matched.Data.ToString();
            try
            {
                var commandToExecute = new Command(commandName, acceptedRequest.MessageBody);
                foreach (string variableName in matched.BoundVariables.Keys)
                {
                    commandToExecute.Parameters[variableName] = matched.BoundVariables[variableName];
                }

                var commandResponse = this.ProcessCommand(commandToExecute);
                return HttpResponseHelper.ResponseString(commandResponse.HttpStatusCode, commandResponse.Content);
            }
            catch (Newtonsoft.Json.JsonReaderException exception)
            {
                Logger.Error("{0}\r\nRAW REQUEST BODY:\r\n{1}", exception.ToString(), acceptedRequest.MessageBody);
                return HttpResponseHelper.ResponseString(HttpStatusCode.BadRequest, exception.ToString());
            }
        }

        private CommandResponse ProcessCommand(Command command)
        {
            Logger.Info("COMMAND {0}\r\n{1}", command.Name, command.Parameters.ToString());
            var executor = this.executorDispatcher.GetExecutor(command.Name);
            executor.ExecutedCommand = command;
            var response = executor.Do();
            Logger.Debug("RESPONSE:\r\n{0}", response);

            return response;
        }

        #endregion
    }
}
