namespace Winium.Silverlight.InnerServer
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Windows;

    using Winium.Mobile.Common;
    using Winium.Mobile.Common.Exceptions;

    internal class AutomatorElements
    {
        #region Static Fields

        private static int safeInstanceCount;

        #endregion

        #region Fields

        private readonly Dictionary<string, WeakReference> registeredElements;

        #endregion

        #region Constructors and Destructors

        public AutomatorElements()
        {
            this.registeredElements = new Dictionary<string, WeakReference>();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns FrameworkElement registered with specified key if any exists. Throws if no element is found.
        /// </summary>
        /// <param name="registeredKey">
        /// </param>
        /// <exception cref="AutomationException">
        /// Registered element is not found or element has been garbage collected.
        /// </exception>
        /// <returns>
        /// The <see cref="FrameworkElement"/>.
        /// </returns>
        public FrameworkElement GetRegisteredElement(string registeredKey)
        {
            WeakReference reference;
            if (this.registeredElements.TryGetValue(registeredKey, out reference))
            {
                var item = reference.Target as FrameworkElement;
                if (item != null)
                {
                    return item;
                }
            }

            throw new AutomationException("Stale element reference", ResponseStatus.StaleElementReference);
        }

        public string RegisterElement(FrameworkElement element)
        {
            var registeredKey = this.registeredElements.FirstOrDefault(x => x.Value.Target == element).Key;

            if (registeredKey == null)
            {
                Interlocked.Increment(ref safeInstanceCount);

                registeredKey = element.GetHashCode() + "-" + safeInstanceCount.ToString(string.Empty, CultureInfo.InvariantCulture);
                this.registeredElements.Add(registeredKey, new WeakReference(element));
            }

            return registeredKey;
        }

        #endregion
    }
}
