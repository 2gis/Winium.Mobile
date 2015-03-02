namespace WindowsUniversalAppDriver.CommandExecutors
{
    #region

    using System;
    using System.Net;

    using Newtonsoft.Json;

    using OpenQA.Selenium.Remote;

    using WindowsUniversalAppDriver.Automator;
    using WindowsUniversalAppDriver.Common;
    using WindowsUniversalAppDriver.Common.Exceptions;

    #endregion

    internal class CommandExecutorBase
    {
        #region Public Properties

        public Command ExecutedCommand { get; set; }

        #endregion

        #region Properties

        protected Automator Automator { get; set; }

        #endregion

        #region Public Methods and Operators

        public CommandResponse Do()
        {
            if (this.ExecutedCommand == null)
            {
                throw new NullReferenceException("ExecutedCommand property must be set before calling Do");
            }

            try
            {
                var session = this.ExecutedCommand.SessionId == null ? null : this.ExecutedCommand.SessionId.ToString();
                this.Automator = Automator.InstanceForSession(session);
                return CommandResponse.Create(HttpStatusCode.OK, this.DoImpl());
            }
            catch (AutomationException ex)
            {
                return CommandResponse.Create(HttpStatusCode.OK, this.JsonResponse(ex.Status, ex.Message));
            }
            catch (InnerDriverRequestException ex)
            {
                // Bad status returned by Inner Driver when trying to forward command
                return CommandResponse.Create(ex.StatusCode, ex.Message);
            }
            catch (NotImplementedException exception)
            {
                return CommandResponse.Create(
                    HttpStatusCode.NotImplemented, 
                    this.JsonResponse(ResponseStatus.UnknownCommand, exception.Message));
            }
            catch (Exception exception)
            {
                return CommandResponse.Create(
                    HttpStatusCode.OK, 
                    this.JsonResponse(ResponseStatus.UnknownError, "Unknown error: " + exception.Message));
            }
        }

        #endregion

        #region Methods

        protected virtual string DoImpl()
        {
            throw new InvalidOperationException("DoImpl should never be called in CommandExecutorBase");
        }

        /// <summary>
        /// The JsonResponse with SUCCESS status and NULL value.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string JsonResponse()
        {
            return this.JsonResponse(ResponseStatus.Success, null);
        }

        protected string JsonResponse(ResponseStatus status, object value)
        {
            return JsonConvert.SerializeObject(new JsonResponse(this.Automator.Session, status, value));
        }

        #endregion
    }
}
