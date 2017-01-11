// <copyright file="WebBrowserNavigationMonitor.cs" company="Salesforce.com">
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

namespace Winium.StoreApps.InnerServer.Web
{
    using System;
    using System.Threading.Tasks;

    using Windows.UI.Core;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    /// <summary>
    /// Monitors navigation within the driver's browser.
    /// </summary>
    internal class WebBrowserNavigationMonitor
    {
        private readonly CommandEnvironment environment;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebBrowserNavigationMonitor"/> class.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> in which the navigation should be monitored.</param>
        public WebBrowserNavigationMonitor(CommandEnvironment environment)
        {
            this.environment = environment;
        }

        /// <summary>
        /// Gets a value indicating whether the navigation was successfully completed.
        /// </summary>
        public bool IsSuccessfullyNavigated { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether the page load was successfully completed.
        /// </summary>
        public bool IsLoadCompleted { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether an error was encountered during the navigation.
        /// </summary>
        public bool IsNavigationError { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether an the navigation timed out.
        /// </summary>
        public bool IsNavigationTimedOut { get; private set; } = false;

        /// <summary>
        /// Monitors a navigation event.
        /// </summary>
        /// <param name="navigationAction">The <see cref="Action"/> causing the navigation.</param>
        public void MonitorNavigation(Action navigationAction)
        {
            var ignoreTimeout = this.environment.PageLoadTimeout < 0;
            var timedOut = true;
            var timeout = TimeSpan.FromMilliseconds(this.environment.PageLoadTimeout);
            var endTime = DateTime.Now.Add(timeout);
            this.IsLoadCompleted = false;
            this.IsSuccessfullyNavigated = false;
            this.IsNavigationError = false;

            this.environment.Browser.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                    {
                        this.environment.Browser.NavigationCompleted += this.BrowserOnNavigationCompleted;
                    }).AsTask().Wait();
            
            navigationAction();

            while (ignoreTimeout || DateTime.Now < endTime)
            {
                if ((this.IsSuccessfullyNavigated && this.IsLoadCompleted) || this.IsNavigationError)
                {
                    timedOut = false;
                    break;
                }

                Task.Delay(100).Wait();
            }

            this.environment.Browser.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                () =>
                {
                    this.environment.Browser.NavigationCompleted -= this.BrowserOnNavigationCompleted;
                }).AsTask().Wait();

            this.IsNavigationTimedOut = timedOut;
        }

        private void BrowserOnNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                this.IsSuccessfullyNavigated = true;
                this.IsLoadCompleted = true;
                this.IsNavigationError = false;
            }
            else
            {
                this.IsNavigationError = true;
            }
        }
    }
}