namespace Winium.StoreApps.InnerServer
{
    #region

    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;

    using Winium.Mobile.Common;
    using Winium.Mobile.Common.Exceptions;
    using Winium.StoreApps.InnerServer.Element;

    #endregion

    internal class ElementsRegistry
    {
        #region Static Fields

        private static int safeInstanceCount;

        #endregion

        #region Fields

        private readonly Dictionary<string, WiniumElement> registredElements;

        #endregion

        #region Constructors and Destructors

        public ElementsRegistry()
        {
            this.registredElements = new Dictionary<string, WiniumElement>();
        }

        #endregion

        #region Public Methods and Operators

        public WiniumElement GetRegisteredElement(string registredKey)
        {
            WiniumElement item;
            if (this.registredElements.TryGetValue(registredKey, out item))
            {
                if (!item.IsStale)
                {
                    return item;
                }

                this.registredElements.Remove(registredKey);
            }

            throw new AutomationException("Stale element reference", ResponseStatus.StaleElementReference);
        }

        public string RegisterElement(WiniumElement element)
        {
            var registeredKey = this.registredElements.FirstOrDefault(x => x.Value.Equals(element)).Key;

            if (registeredKey == null)
            {
                registeredKey = GenerateGuid(element);
                this.registredElements.Add(registeredKey, element);
            }

            return registeredKey;
        }

        #endregion

        #region Methods

        private static string GenerateGuid(WiniumElement element)
        {
            Interlocked.Increment(ref safeInstanceCount);
            return element.GetHashCode() + "-" + safeInstanceCount.ToString(string.Empty, CultureInfo.InvariantCulture);
        }

        #endregion
    }
}
