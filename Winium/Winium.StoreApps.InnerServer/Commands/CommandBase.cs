namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Windows.UI.Core;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.Common.Exceptions;

    #endregion

    internal class CommandBase
    {
        #region Public Properties

        public Automator Automator { get; set; }

        public IDictionary<string, JToken> Parameters { get; set; }

        public string Session { get; set; }

        #endregion

        #region Public Methods and Operators

        private static void BeginInvokeSync(CoreDispatcher dispatcher, Action action)
        {
            Exception exception = null;
            var waitEvent = new AutoResetEvent(false);

            // TODO Research dispatcher.RunIdleAsync
            dispatcher.RunAsync(
                CoreDispatcherPriority.Normal, 
                () =>
                    {
                        try
                        {
                            action();
                        }
                        catch (Exception ex)
                        {
                            exception = ex;
                        }

                        waitEvent.Set();
                    });
            waitEvent.WaitOne();

            if (exception != null)
            {
                throw exception;
            }
        }

        public string Do()
        {
            if (this.Automator == null)
            {
                throw new InvalidOperationException("Automator must be set before Do() is called");
            }

            var response = string.Empty;
            try
            {
                BeginInvokeSync(this.Automator.UiThreadDispatcher, () => { response = this.DoImpl(); });
            }
            catch (AutomationException exception)
            {
                response = this.JsonResponse(exception.Status, exception.Message);
            }
            catch (Exception exception)
            {
                response = this.JsonResponse(ResponseStatus.UnknownError, "Unknown error: " + exception.Message);
            }

            return response;
        }

        protected virtual string DoImpl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The JsonResponse with SUCCESS status and NULL value.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected string JsonResponse()
        {
            return JsonConvert.SerializeObject(new JsonResponse(this.Session, ResponseStatus.Success, null));
        }

        protected string JsonResponse(ResponseStatus status, object value)
        {
            if (status != ResponseStatus.Success && value == null)
            {
                value = string.Format("WebDriverException {0}", Enum.GetName(typeof(ResponseStatus), status));
            }

            return JsonConvert.SerializeObject(new JsonResponse(this.Session, status, value));
        }

        #endregion
    }
}
