namespace WindowsUniversalAppDriver.InnerServer.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Windows.UI.Core;
    using Windows.UI.Xaml;

    using Newtonsoft.Json;

    using WindowsUniversalAppDriver.Common;
    using WindowsUniversalAppDriver.Common.Exceptions;
    using WindowsUniversalAppDriver.InnerServer;

    internal class CommandBase
    {
        #region Public Properties

        public Automator Automator { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public string Session { get; set; }

        #endregion

        #region Public Methods and Operators

        public static void BeginInvokeSync(UIElement root, Action action)
        {
            Exception exception = null;
            var waitEvent = new AutoResetEvent(false);

            root.Dispatcher.RunAsync(
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
                BeginInvokeSync(Automator.VisualRoot, () => { response = this.DoImpl(); });
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

        public virtual string DoImpl()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The JsonResponse with SUCCESS status and NULL value.
        /// </summary>
        public string JsonResponse()
        {
            return JsonConvert.SerializeObject(new JsonResponse(this.Session, ResponseStatus.Success, null));
        }

        public string JsonResponse(ResponseStatus status, object value)
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
