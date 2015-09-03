namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using System;
    using System.Collections.Generic;

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

        public string Do()
        {
            if (this.Automator == null)
            {
                throw new InvalidOperationException("Automator must be set before Do() is called");
            }

            var response = string.Empty;
            try
            {
                InvokeSync(this.Automator.UiThreadDispatcher, () => { response = this.DoImpl(); });
            }
            catch (AutomationException exception)
            {
                response = this.JsonResponse(exception.Status, exception);
            }
            catch (Exception exception)
            {
                response = this.JsonResponse(ResponseStatus.UnknownError, exception);
            }

            return response;
        }

        #endregion

        #region Methods

        protected virtual string DoImpl()
        {
            throw new NotImplementedException();
        }

        protected string JsonResponse(ResponseStatus status = ResponseStatus.Success, object value = null)
        {
            if (status != ResponseStatus.Success && value == null)
            {
                value = string.Format("WebDriverException {0}", Enum.GetName(typeof(ResponseStatus), status));
            }

            return JsonConvert.SerializeObject(new JsonResponse(this.Session, status, value), Formatting.Indented);
        }

        private static void InvokeSync(CoreDispatcher dispatcher, Action action)
        {
            Exception exception = null;

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
                    }).AsTask().Wait();

            if (exception != null)
            {
                throw new Exception("An exception occured while execuiting a command. ", exception);
            }
        }

        #endregion
    }
}
