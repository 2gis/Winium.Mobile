namespace Winium.Mobile.Common.Exceptions
{
    #region

    using System;
    using System.Net;

    #endregion

    public class InnerServerRequestException : Exception
    {
        #region Constructors and Destructors

        public InnerServerRequestException()
        {
        }

        public InnerServerRequestException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            this.StatusCode = statusCode;
        }

        public InnerServerRequestException(string message, params object[] args)
            : base(string.Format(message, args))
        {
        }

        public InnerServerRequestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        #endregion

        #region Public Properties

        public HttpStatusCode StatusCode { get; set; }

        #endregion
    }
}
