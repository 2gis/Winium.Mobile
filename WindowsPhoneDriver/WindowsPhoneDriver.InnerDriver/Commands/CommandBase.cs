﻿namespace WindowsPhoneDriver.InnerDriver.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows;

    using Newtonsoft.Json;

    using WindowsPhoneDriver.Common;
    using WindowsPhoneDriver.Common.Exceptions;

    internal class CommandBase
    {
        #region Public Properties

        public Automator Automator { get; set; }

        public Dictionary<string, object> Parameters { get; set; }

        public string Session { get; set; }

        #endregion

        #region Public Methods and Operators

        public static void BeginInvokeSync(Action action)
        {
            Exception exception = null;
            var waitEvent = new AutoResetEvent(false);

            Deployment.Current.Dispatcher.BeginInvoke(
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
                BeginInvokeSync(() => { response = this.DoImpl(); });
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
