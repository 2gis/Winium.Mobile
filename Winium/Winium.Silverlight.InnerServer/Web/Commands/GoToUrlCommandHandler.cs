namespace Winium.Silverlight.InnerServer.Web.Commands
{
    using Mobile.Common;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class GoToUrlCommandHandler : WebCommandHandler
    {
        private const string AlertHandler = @"
function() {
  window.__wd_alert = window.alert;
  window.alert = function(text) {
    window.external.notify('alert:' + text);
    window.__wd_alert(text);
    window.external.notify('clear');
  };
  window.__wd_confirm = window.confirm;
  window.confirm = function(text) {
    window.external.notify('confirm:' + text);
    var confirmValue = window.__wd_confirm(text);
    window.external.notify('clear');
    return confirmValue;
  };
}";

        private int retryCount = 3;
        private bool handleAlerts = false;

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            object url;
            if (!parameters.TryGetValue("url", out url))
            {
                return Response.CreateMissingParametersResponse("url");
            }

            Uri targetUri = null;
            if (!Uri.TryCreate(url.ToString(), UriKind.Absolute, out targetUri))
            {
                return Response.CreateErrorResponse(ResponseStatus.UnknownError, string.Format(CultureInfo.InvariantCulture, "Could not create valie URL from {0}", url.ToString()));
            }

            int timeoutInMilliseconds = environment.PageLoadTimeout;
            if (timeoutInMilliseconds < 0)
            {
                timeoutInMilliseconds = 15000;
            }
            else
            {
                // If a page load timeout has been set, don't retry the page load.
                this.retryCount = 1;
            }

            WebBrowserNavigationMonitor monitor = new WebBrowserNavigationMonitor(environment);
            for (int retries = 0; retries < this.retryCount; retries++)
            {
                monitor.MonitorNavigation(() =>
                {
                    environment.Browser.Dispatcher.BeginInvoke(() =>
                    {
                        environment.Browser.Navigate(targetUri);
                    });
                });

                if ((monitor.IsSuccessfullyNavigated && monitor.IsLoadCompleted) || monitor.IsNavigationError)
                {
                    break;
                }
            }

            if (monitor.IsNavigationTimedOut)
            {
                return Response.CreateErrorResponse(ResponseStatus.Timeout, "Timed out loading page");
            }

            if (!monitor.IsNavigationError)
            {
                environment.FocusedFrame = string.Empty;
            }

            if (this.handleAlerts)
            {
                this.EvaluateAtom(environment, AlertHandler);
            }

            return Response.CreateSuccessResponse();
        }
    }

}
