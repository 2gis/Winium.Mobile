// <copyright file="GoToUrlCommandHandler.cs" company="Salesforce.com">
//
// Copyright (c) 2014 Salesforce.com, Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the
// following conditions are met:
//
//    Redistributions of source code must retain the above copyright notice, this list of conditions and the following
//    disclaimer.
//
//    Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the
//    following disclaimer in the documentation and/or other materials provided with the distribution.
//
//    Neither the name of Salesforce.com nor the names of its contributors may be used to endorse or promote products
//    derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES,
// INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
// SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE
// USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>

namespace Winium.StoreApps.InnerServer.Web.Commands
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Windows.UI.Core;

    using Winium.Mobile.Common;

    #endregion

    /// <summary>
    /// Provides handling for the go to URL command.
    /// </summary>
    internal class GoToUrlCommandHandler : WebCommandAdapterHandler
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

        private bool handleAlerts = false;

        private int retryCount = 3;

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        protected override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            object url;
            if (!parameters.TryGetValue("url", out url))
            {
                return Response.CreateMissingParametersResponse("url");
            }

            Uri targetUri = null;
            if (!Uri.TryCreate(url.ToString(), UriKind.Absolute, out targetUri))
            {
                return Response.CreateErrorResponse(
                    ResponseStatus.UnknownError,
                    string.Format(CultureInfo.InvariantCulture, "Could not create valie URL from {0}", url.ToString()));
            }

            var timeoutInMilliseconds = environment.PageLoadTimeout;
            if (timeoutInMilliseconds < 0)
            {
                timeoutInMilliseconds = 15000;
            }
            else
            {
                // If a page load timeout has been set, don't retry the page load.
                this.retryCount = 1;
            }

            var monitor = new WebBrowserNavigationMonitor(environment);
            for (var retries = 0; retries < this.retryCount; retries++)
            {
                monitor.MonitorNavigation(
                    () =>
                        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

                            environment.Browser.Dispatcher.RunAsync(
                                CoreDispatcherPriority.Normal,
                                () =>
                                    {
                                        environment.Browser.Navigate(targetUri); 
                                        
                                    });

#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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
