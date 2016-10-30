// <copyright file="CommandEnvironment.cs" company="Salesforce.com">
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
    using System.Collections.Generic;

    using Windows.UI.Xaml.Controls;

    using Winium.StoreApps.InnerServer.Element;

    internal class CommandEnvironment
    {
        /// <summary>
        /// The key used to denote a window object.
        /// </summary>
        public const string WindowObjectKey = "WINDOW";

        /// <summary>
        /// The key used to denote an element object.
        /// </summary>
        public const string ElementObjectKey = "ELEMENT";

        /// <summary>
        /// The global window handle string used, since the driver only supports one window.
        /// </summary>
        public const string GlobalWindowHandle = "WPDriverWindowHandle";

        private readonly WiniumElement element;

        public CommandEnvironment(WiniumElement element)
        {
            this.PageLoadTimeout = -1;
            this.AsyncScriptTimeout = -1;
            this.FocusedFrame = string.Empty;
            this.AlertType = string.Empty;
            this.AlertText = string.Empty;
            this.MouseState = new Dictionary<string, object>();
            this.KeyboardState = null;
            this.element = element;

            this.Browser.ScriptNotify += this.BrowserScriptNotifyEventHandler;
            this.Browser.NavigationStarting += this.BrowserNavigationStartingEventHandler;
        }

        public WiniumElement Element
        {
            get
            {
                return this.element;
            }
        }

        public WebView Browser {
            get
            {
                return this.element.Element as WebView;
            }
        }

        /// <summary>
        /// Gets or sets the keyboard state of the driver.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Values are typed correctly for JSON serialization/deserialization.")]
        public Dictionary<string, object> KeyboardState { get; set; }

        /// <summary>
        /// Gets or sets the mouse state of the driver.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Values are typed correctly for JSON serialization/deserialization.")]
        public Dictionary<string, object> MouseState { get; set; }

        /// <summary>
        /// Gets the text of the active alert.
        /// </summary>
        public string AlertText { get; private set; }

        /// <summary>
        /// Gets the type of the active alert.
        /// </summary>
        public string AlertType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether execution of the next command should be blocked.
        /// </summary>
        public bool IsBlocked { get; private set; }

        /// <summary>
        /// Gets or sets the ID of the currently focused frame in the browser.
        /// </summary>
        public string FocusedFrame { get; set; }

        /// <summary>
        /// Gets or sets the implicit wait timeout in milliseconds.
        /// </summary>
        public int ImplicitWaitTimeout { get; set; }

        /// <summary>
        /// Gets or sets the asynchronous script timeout in milliseconds.
        /// </summary>
        public int AsyncScriptTimeout { get; set; }

        /// <summary>
        /// Gets or sets the page load timeout in milliseconds.
        /// </summary>
        public int PageLoadTimeout { get; set; }

        /// <summary>
        /// Creates a serializable object for the currently focused frame.
        /// </summary>
        /// <returns>A <see>
        ///         <cref>Dictionary{string, object}</cref>
        ///     </see>
        ///     representing the currently focused
        /// frame that can be serialized into a format the atoms will expect.</returns>
        public Dictionary<string, object> CreateFrameObject()
        {
            if (string.IsNullOrEmpty(this.FocusedFrame))
            {
                return null;
            }

            var returnValue = new Dictionary<string, object>();
            returnValue[WindowObjectKey] = this.FocusedFrame;
            return returnValue;
        }

        /// <summary>
        /// Clears the alert status of the driver.
        /// </summary>
        public void ClearAlertStatus()
        {
            this.IsBlocked = false;
            this.AlertType = string.Empty;
            this.AlertText = string.Empty;
        }

        private void BrowserNavigationStartingEventHandler(object sender, WebViewNavigationStartingEventArgs e)
        {
            this.MouseState.Clear();
            var clientXY = new Dictionary<string, object>();
            clientXY["x"] = 0;
            clientXY["y"] = 0;
            this.MouseState["clientXY"] = clientXY;
            this.MouseState["element"] = null;
        }

        private void BrowserScriptNotifyEventHandler(object sender, NotifyEventArgs e)
        {
            if (!this.IsBlocked)
            { 
                var valueParts = e.Value.Split(new[] { ':' }, 2);
                this.AlertType = valueParts[0];
                this.AlertText = valueParts[1];
                this.IsBlocked = true;
            }
            else
            {
                this.ClearAlertStatus();
            }
        }
    }
}
