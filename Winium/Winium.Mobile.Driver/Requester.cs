namespace Winium.Mobile.Driver
{
    #region

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;

    using Newtonsoft.Json;

    using Winium.Mobile.Common;
    using Winium.Mobile.Common.Exceptions;
    using Winium.Mobile.Logging;

    #endregion

    internal class Requester
    {
        #region Fields

        private readonly UriBuilder uriBuilder;

        #endregion

        #region Constructors and Destructors

        public Requester(string ip, int port)
        {
            this.uriBuilder = new UriBuilder("http", ip, port, string.Empty);
        }

        #endregion

        #region Public Methods and Operators

        public string ForwardCommand(Command commandToForward, bool verbose = true, int timeout = 0)
        {
            var serializedCommand = JsonConvert.SerializeObject(commandToForward);
            var response = this.SendRequest(serializedCommand, verbose, timeout);
            if (response.Key == HttpStatusCode.OK)
            {
                return response.Value;
            }

            throw new InnerServerRequestException(response.Value, response.Key);
        }

        #endregion

        #region Methods

        private static HttpWebRequest CreateWebRequest(Uri uri, string content, int timeout)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.ContentType = "application/json";
            request.Method = "POST";

            if (timeout != 0)
            {
                request.Timeout = timeout;
            }

            if (!string.IsNullOrEmpty(content))
            {
                var writer = new StreamWriter(request.GetRequestStream());
                writer.Write(content);
                writer.Close();
            }

            return request;
        }

        private KeyValuePair<HttpStatusCode, string> SendRequest(string requestContent, bool verbose, int timeout)
        {
            var result = string.Empty;
            StreamReader reader = null;
            HttpWebResponse response = null;
            var status = HttpStatusCode.OK;
            try
            {
                // TODO Refactor error handling
                var uri = this.uriBuilder.Uri;

                if (verbose)
                {
                    Logger.Debug("Sending request to inner server: {0}", uri);
                }

                var request = CreateWebRequest(uri, requestContent, timeout);

                try
                {
                    response = request.GetResponse() as HttpWebResponse;
                }
                catch (WebException ex)
                {
                    response = ex.Response as HttpWebResponse;
                }

                if (response != null)
                {
                    status = response.StatusCode;
                    var stream = response.GetResponseStream();
                    if (stream == null)
                    {
                        throw new NullReferenceException("No response stream.");
                    }

                    // read and return the response
                    reader = new StreamReader(stream);
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                if (verbose)
                {
                    // No need to log exceptions raised when sending service commands like ping.
                    Logger.Error("Error occurred while trying to send request to inner server: {0}", ex);
                    throw;
                }
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }

                if (reader != null)
                {
                    reader.Close();
                }
            }

            return new KeyValuePair<HttpStatusCode, string>(status, result);
        }

        #endregion
    }
}
