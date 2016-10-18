namespace Winium.Mobile.Common.Exceptions
{
    #region

    using System;

    #endregion

    public class AutomationException : Exception
    {
        #region Fields

        private ResponseStatus responseStatus = ResponseStatus.UnknownError;

        #endregion

        #region Constructors and Destructors

        public AutomationException()
        {
        }

        public AutomationException(string message, ResponseStatus status)
            : base(message)
        {
            this.Status = status;
        }

        public AutomationException(string message, params object[] args)
            : base(string.Format(message, args))
        {
        }

        public AutomationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public AutomationException(string message, Exception innerException, ResponseStatus status)
            : base(message, innerException)
        {
            this.Status = status;
        }

        #endregion

        #region Public Properties

        public ResponseStatus Status
        {
            get
            {
                return this.responseStatus;
            }

            set
            {
                this.responseStatus = value;
            }
        }

        #endregion
    }
}
