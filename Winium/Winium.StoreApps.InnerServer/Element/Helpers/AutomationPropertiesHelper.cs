namespace Winium.StoreApps.InnerServer.Element.Helpers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Automation;

    #endregion

    internal class AutomationPropertiesHelper
    {
        #region Static Fields

        private static readonly Lazy<AutomationPropertiesHelper> LazyInstance =
            new Lazy<AutomationPropertiesHelper>(() => new AutomationPropertiesHelper());

        #endregion

        #region Fields

        private readonly Dictionary<string, DependencyProperty> properties;

        #endregion

        #region Constructors and Destructors

        private AutomationPropertiesHelper()
        {
            this.properties = typeof(AutomationProperties).GetRuntimeProperties()
                .ToDictionary(f => f.Name, f => (DependencyProperty)f.GetValue(null));
        }

        #endregion

        #region Public Properties

        public static AutomationPropertiesHelper Instance
        {
            get
            {
                return LazyInstance.Value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public bool TryGetAutomationProperty(string name, out DependencyProperty property)
        {
            return this.properties.TryGetValue(name, out property);
        }

        #endregion
    }
}
