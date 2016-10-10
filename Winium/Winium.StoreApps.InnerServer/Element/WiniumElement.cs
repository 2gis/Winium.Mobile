namespace Winium.StoreApps.InnerServer.Element
{
    #region

    using System;

    using Newtonsoft.Json.Linq;

    using Windows.UI.Xaml;

    using Winium.Mobile.Common;
    using Winium.Mobile.Common.Exceptions;
    using Winium.StoreApps.InnerServer.Commands.Helpers;
    using Winium.StoreApps.InnerServer.Element.Helpers;

    #endregion

    internal partial class WiniumElement : IEquatable<WiniumElement>
    {
        #region Fields

        private readonly WeakReference<FrameworkElement> weakElement;

        #endregion

        #region Constructors and Destructors

        public WiniumElement(FrameworkElement element)
        {
            this.weakElement = new WeakReference<FrameworkElement>(element);
        }

        #endregion

        #region Public Properties

        public string AutomationId
        {
            get
            {
                return this.Element.AutomationId();
            }
        }

        public string AutomationName
        {
            get
            {
                return this.Element.AutomationName();
            }
        }

        public string ClassName
        {
            get
            {
                return this.Element.ClassName();
            }
        }

        public FrameworkElement Element
        {
            get
            {
                FrameworkElement target;
                if (this.weakElement.TryGetTarget(out target))
                {
                    return target;
                }

                throw new AutomationException("Stale element reference", ResponseStatus.StaleElementReference);
            }
        }

        public bool IsEnabled
        {
            get
            {
                return this.Element.GetAutomationPeer().IsEnabled();
            }
        }

        public bool IsStale
        {
            get
            {
                FrameworkElement element;
                return !this.weakElement.TryGetTarget(out element);
            }
        }

        public string XName
        {
            get
            {
                return this.Element.Name;
            }
        }

        #endregion

        #region Public Methods and Operators

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((WiniumElement)obj);
        }

        public bool Equals(WiniumElement other)
        {
            FrameworkElement thisElement;
            FrameworkElement otherElement;
            if (this.weakElement.TryGetTarget(out thisElement) && other.weakElement.TryGetTarget(out otherElement))
            {
                return thisElement == otherElement;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return this.weakElement != null ? this.weakElement.GetHashCode() : 0;
        }

        public void SetProperty(string attributeName, JToken value)
        {
            PropertiesAccessor.SetProperty(this.Element, attributeName, value);
        }

        public bool TryGetAutomationProperty(string automationPropertyName, out object value)
        {
            const string Prefix = "AutomationProperties.";
            value = null;
            if (!automationPropertyName.StartsWith(Prefix))
            {
                return false;
            }

            automationPropertyName = automationPropertyName.Remove(0, Prefix.Length);

            return AutomationPropertiesAccessor.TryGetAutomationProperty(
                this.Element, 
                automationPropertyName, 
                out value);
        }

        public bool TryGetDependencyProperty(string propertyName, out object value)
        {
            return DependencyPropertiesAccessor.TryGetDependencyProperty(this.Element, propertyName, out value);
        }

        public bool TryGetProperty(string attributeName, out object value)
        {
            return PropertiesAccessor.TryGetProperty(this.Element, attributeName, out value);
        }

        #endregion
    }
}
