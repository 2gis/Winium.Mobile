namespace WindowsPhoneDriver.OuterDriver.CommandExecutors
{
    using System;
    using System.Net;

    using Newtonsoft.Json;

    using OpenQA.Selenium.Remote;

    using WindowsPhoneDriver.Common;
    using WindowsPhoneDriver.Common.Exceptions;
    using WindowsPhoneDriver.OuterDriver.Automator;

    internal class CommandExecutorBase
    {
        #region Public Properties

        public Command ExecutedCommand { get; set; }

        #endregion

        #region Properties

        protected Automator Automator { get; set; }

        #endregion

        #region Public Methods and Operators

        public string Do()
        {
            if (this.ExecutedCommand == null)
            {
                throw new NullReferenceException("ExecutedCommand property must be set before calling Do");
            }

            try
            {
                var session = this.ExecutedCommand.SessionId == null ? null : this.ExecutedCommand.SessionId.ToString();
                this.Automator = Automator.InstanceForSession(session);
                return HttpResponseHelper.ResponseString(HttpStatusCode.OK, this.DoImpl());
            }
            catch (AutomationException ex)
            {
                return HttpResponseHelper.ResponseString(HttpStatusCode.OK, this.JsonResponse(ex.Status, ex.Message));
            }
            catch (InnerDriverRequestException ex)
            {
                // Bad status returned by Inner Driver when trying to forward command
                return HttpResponseHelper.ResponseString(ex.StatusCode, ex.Message);
            }
            catch (NotImplementedException exception)
            {
                return HttpResponseHelper.ResponseString(HttpStatusCode.NotImplemented, exception.Message);
            }
            catch (Exception exception)
            {
                return HttpResponseHelper.ResponseString(
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

        protected string JsonResponse(ResponseStatus status, object value)
        {
            return JsonConvert.SerializeObject(new JsonResponse(this.Automator.Session, status, value));
        }

        #endregion
    }
}
