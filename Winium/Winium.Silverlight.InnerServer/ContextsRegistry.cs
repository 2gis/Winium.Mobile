namespace Winium.Silverlight.InnerServer
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Phone.Controls;
    using Mobile.Common;
    using Mobile.Common.Exceptions;
    using Web;
    using System.Windows.Media;
    using System.Windows;
    using Commands.FindByHelpers;
    using System.Linq;
    using System.Threading;
    using Commands;
    using System.Globalization;

    internal class ContextsRegistry
    {
        private static int safeInstanceCount;

        private readonly Dictionary<string, CommandEnvironment> contexts;

        public const string NativeAppContext = "NATIVE_APP";

        public WebBrowser browser { get; set; }

        public ContextsRegistry()
        {
            this.contexts = new Dictionary<string, CommandEnvironment>();
        }

        public IEnumerable<string> GetAllContexts(WebBrowser browser)
        {
            this.browser = browser;
            RegisterContext(this.browser);
            return contexts.Keys;
        }

        private void RegisterContext(DependencyObject element)
        {
            var existing = this.contexts.FirstOrDefault(x => Equals(x.Value, element)).Key;

            if (existing != null)
            {
                return;
            }

            var id = GenerateId((FrameworkElement)element);
            this.contexts.Add(id, new CommandEnvironment(this.browser));
        }

        public CommandEnvironment GetContext(string id)
        {
            if (this.contexts.Count == 1 )
            {
                this.GetAllContexts(this.browser);
            }
            
            CommandEnvironment context;
            if (this.contexts.TryGetValue(id, out context))
            {
                if (context != null)
                {
                    return context;
                }
            }
            throw new AutomationException("Stale element reference", ResponseStatus.StaleElementReference);
        }

        private static string GenerateId(FrameworkElement element)
        {
            Interlocked.Increment(ref safeInstanceCount);

            var friendlyName = !string.IsNullOrWhiteSpace(element.AutomationId())
                                   ? element.AutomationId()
                                   : safeInstanceCount.ToString(CultureInfo.InvariantCulture);

            return string.Format("WEBVIEW_{0}", friendlyName);
        }
    }

}
