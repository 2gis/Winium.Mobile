namespace Winium.StoreApps.InnerServer.Element
{
    #region

    using System;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Automation;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.Common.Exceptions;

    #endregion

    internal partial class WiniumElement
    {
        #region Fields

        private readonly WeakReference<FrameworkElement> weakElement;

        #endregion

        #region Constructors and Destructors

        public WiniumElement(FrameworkElement element)
        {
            this.weakElement = new WeakReference<FrameworkElement>(element);
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
        
        #endregion

        #region Public Properties

        public string AutomationId
        {
            get
            {
                return this.Element.GetValue(AutomationProperties.AutomationIdProperty) as string;
            }
        }

        public string AutomationName
        {
            get
            {
                return this.Element.GetValue(AutomationProperties.NameProperty) as string;
            }
        }

        public string XName
        {
            get
            {
                return this.Element.Name;
            }
        }

        public string ClassName
        {
            get
            {
                return this.Element.GetType().ToString();
            }
        }

        #endregion
    }
}
