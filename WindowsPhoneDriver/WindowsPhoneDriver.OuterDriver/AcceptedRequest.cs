namespace WindowsPhoneDriver.OuterDriver
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Net.Sockets;

    internal class AcceptedRequest
    {
        #region Public Properties

        public string Content { get; private set; }

        public string Request { get; private set; }

        #endregion

        #region Properties

        private Dictionary<string, string> Headers { get; set; }

        #endregion

        #region Public Methods and Operators

        public void AcceptRequest(NetworkStream stream)
        {
            this.Request = ReadString(stream);
            Logger.Debug("ACCEPTED REQUEST {0}", this.Request);

            this.Headers = ReadHeaders(stream);

            this.Content = ReadContent(stream, this.Headers);
        }

        #endregion

        #region Methods

        private static string ReadContent(NetworkStream stream, IReadOnlyDictionary<string, string> headers)
        {
            string contentLengthString;
            var hasContentLength = headers.TryGetValue("Content-Length", out contentLengthString);
            var content = string.Empty;
            if (hasContentLength)
            {
                content = ReadContent(stream, Convert.ToInt32(contentLengthString, CultureInfo.InvariantCulture));
            }

            return content;
        }

        // reads the content of a request depending on its length
        private static string ReadContent(NetworkStream s, int contentLength)
        {
            var readBuffer = new byte[contentLength];
            var readBytes = s.Read(readBuffer, 0, readBuffer.Length);
            return System.Text.Encoding.ASCII.GetString(readBuffer, 0, readBytes);
        }

        private static Dictionary<string, string> ReadHeaders(Stream stream)
        {
            var headers = new Dictionary<string, string>();
            string header;
            while (!string.IsNullOrEmpty(header = ReadString(stream)))
            {
                var splitHeader = header.Split(':');
                headers.Add(splitHeader[0], splitHeader[1].Trim(' '));
            }

            return headers;
        }

        private static string ReadString(Stream stream)
        {
            // StreamReader reader = new StreamReader(stream);
            var data = string.Empty;
            while (true)
            {
                var nextChar = stream.ReadByte();
                if (nextChar == '\n')
                {
                    break;
                }

                if (nextChar == '\r')
                {
                    continue;
                }

                data += Convert.ToChar(nextChar);
            }

            return data;
        }

        #endregion
    }
}
