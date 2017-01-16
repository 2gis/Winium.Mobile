using System.Collections.Generic;

namespace Winium.StoreApps.InnerServer
{
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    using Windows.UI.Xaml.Controls;

    using Element;
    using Web;

    using Winium.Mobile.Common;
    using Winium.Mobile.Common.Exceptions;

    internal class ContextsRegistry
    {
        private static int safeInstanceCount;

        private readonly Dictionary<string, CommandEnvironment> contexts;

        public const string NativeAppContext = "NATIVE_APP";

        public ContextsRegistry()
        {
            this.contexts = new Dictionary<string, CommandEnvironment>();
        }

        public IEnumerable<string> GetAllContexts()
        {
            var webViews = WiniumVirtualRoot.Current.Find(TreeScope.Descendants, x => x is WebView);

            // FIXME ToList is required here to actually register contexts, but it causes "The application called an interface that was marshalled for a different thread" later on
            return webViews.Select(this.RegisterContext); 
        }

        private string RegisterContext(WiniumElement element)
        {
            var existing = this.contexts.FirstOrDefault(x => Equals(x.Value.Element, element)).Key;

            if (existing != null)
            {
                return existing;
            }

            var id = GenerateId(element);
            this.contexts.Add(id, new CommandEnvironment(element));

            return id;
        }

        public CommandEnvironment GetContext(string id)
        {
            this.GetAllContexts();
            CommandEnvironment context;
            if (this.contexts.TryGetValue(id, out context))
            {
                if (!context.Element.IsStale)
                {
                    return context;
                }
            }
            throw new AutomationException("Stale element reference", ResponseStatus.StaleElementReference);
        }

        private static string GenerateId(WiniumElement element)
        {
            Interlocked.Increment(ref safeInstanceCount);

            var friendlyName = !string.IsNullOrWhiteSpace(element.AutomationId)
                                   ? element.AutomationId
                                   : safeInstanceCount.ToString(CultureInfo.InvariantCulture);

            return string.Format("WEBVIEW_{0}", friendlyName);
        }
    }
}
