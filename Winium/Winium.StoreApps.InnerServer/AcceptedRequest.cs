namespace Winium.StoreApps.InnerServer
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading.Tasks;

    using Windows.Storage.Streams;

    #endregion

    internal class AcceptedRequest
    {
        #region Constructors and Destructors

        public AcceptedRequest()
        {
            this.Request = string.Empty;
            this.Headers = new Dictionary<string, string>();
            this.Content = string.Empty;
        }

        #endregion

        #region Public Properties

        public string Content { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public string Request { get; private set; }

        #endregion

        #region Public Methods and Operators

        public async Task AcceptRequest(DataReader reader)
        {
            this.Request = await StreamReadLine(reader);

            // read HTTP headers
            this.Headers = await ReadHeaders(reader);

            // read request contents
            var contentLength = GetContentLength(this.Headers);
            if (contentLength != 0)
            {
                this.Content = await ReadContent(reader, contentLength);
            }
        }

        #endregion

        #region Methods

        private static uint GetContentLength(Dictionary<string, string> headers)
        {
            uint contentLength = 0;
            string contentLengthString;
            var hasContentLength = headers.TryGetValue("Content-Length", out contentLengthString);
            if (hasContentLength)
            {
                contentLength = Convert.ToUInt32(contentLengthString, CultureInfo.InvariantCulture);
            }

            return contentLength;
        }

        private static async Task<string> ReadContent(DataReader reader, uint contentLength)
        {
            await reader.LoadAsync(contentLength);
            string content = reader.ReadString(contentLength);
            return content;
        }

        private static async Task<Dictionary<string, string>> ReadHeaders(DataReader reader)
        {
            var headers = new Dictionary<string, string>();
            string header;
            while (!string.IsNullOrEmpty(header = await StreamReadLine(reader)))
            {
                string[] splitHeader = header.Split(':');
                headers.Add(splitHeader[0], splitHeader[1].Trim(' '));
            }

            return headers;
        }

        private static async Task<string> StreamReadLine(DataReader reader)
        {
            var data = string.Empty;
            while (true)
            {
                await reader.LoadAsync(1);
                int nextChar = reader.ReadByte();
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
