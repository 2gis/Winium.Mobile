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

namespace Winium.Silverlight.InnerServer.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;

    /// <summary>
    /// Monitors navigation within the driver's browser.
    /// </summary>

    internal class WebBrowserNavigationMonitor
    {
        private CommandEnvironment environment;
        private bool isNavigated = false;
        private bool isLoadCompleted = false;
        private bool isNavigationError = false;
        private bool isNavigationTimedOut = false;

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
        public bool IsSuccessfullyNavigated
        {
            get { return this.isNavigated; }
        }

        /// <summary>
        /// Gets a value indicating whether the page load was successfully completed.
        /// </summary>
        public bool IsLoadCompleted
        {
            get { return this.isLoadCompleted; }
        }

        /// <summary>
        /// Gets a value indicating whether an error was encountered during the navigation.
        /// </summary>
        public bool IsNavigationError
        {
            get { return this.isNavigationError; }
        }

        /// <summary>
        /// Gets a value indicating whether an the navigation timed out.
        /// </summary>
        public bool IsNavigationTimedOut
        {
            get { return this.isNavigationTimedOut; }
        }

        /// <summary>
        /// Monitors a navigation event.
        /// </summary>
        /// <param name="navigationAction">The <see cref="Action"/> causing the navigation.</param>
        public void MonitorNavigation(Action navigationAction)
        {
            bool ignoreTimeout = this.environment.PageLoadTimeout < 0;
            bool timedOut = true;
            TimeSpan timeout = TimeSpan.FromMilliseconds(this.environment.PageLoadTimeout);
            DateTime endTime = DateTime.Now.Add(timeout);
            this.isLoadCompleted = false;
            this.isNavigated = false;
            this.isNavigationError = false;
            this.environment.Browser.Navigated += this.BrowserNavigatedEventHandler;
            this.environment.Browser.LoadCompleted += this.BrowserLoadCompletedEventHandler;
            this.environment.Browser.NavigationFailed += this.BrowserNavigationFailedEventHandler;
            navigationAction();

            while (ignoreTimeout || DateTime.Now < endTime)
            {
                if ((this.isNavigated && this.isLoadCompleted) || this.isNavigationError)
                {
                    timedOut = false;
                    break;
                }

                System.Threading.Thread.Sleep(100);
            }

            this.environment.Browser.LoadCompleted -= this.BrowserLoadCompletedEventHandler;
            this.environment.Browser.Navigated -= this.BrowserNavigatedEventHandler;
            this.environment.Browser.NavigationFailed -= this.BrowserNavigationFailedEventHandler;

            this.isNavigationTimedOut = timedOut;
        }

        /// <summary>
        /// Sets the values if the navigation is successful.
        /// </summary>
        public void SetNavigationSuccessful()
        {
            this.isNavigated = true;
            this.isLoadCompleted = true;
            this.isNavigationError = false;
        }

        private void BrowserNavigationFailedEventHandler(object sender, NavigationFailedEventArgs e)
        {
            this.isNavigationError = true;
        }

        private void BrowserNavigatedEventHandler(object sender, NavigationEventArgs e)
        {
            this.isNavigated = true;
        }

        private void BrowserLoadCompletedEventHandler(object sender, NavigationEventArgs e)
        {
            this.isLoadCompleted = true;
        }
    }
}
