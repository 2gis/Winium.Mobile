namespace Winium.StoreApps.InnerServer.Element.Helpers
{
    #region

    using Windows.UI.Xaml;

    #endregion

    internal static class AutomationPropertiesAccessor
    {
        #region Public Methods and Operators

        public static bool TryGetAutomationProperty(FrameworkElement element, string propertyName, out object value)
        {
            propertyName = string.Format("{0}Property", propertyName);
            value = null;
            DependencyProperty property;

            if (!AutomationPropertiesHelper.Instance.TryGetAutomationProperty(propertyName, out property))
            {
                return false;
            }

            value = element.GetValue(property);
            return true;
        }

        #endregion
    }
}
