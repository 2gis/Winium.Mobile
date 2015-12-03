namespace Winium.StoreApps.InnerServer.Element
{
    #region

    using Newtonsoft.Json.Linq;

    using Winium.StoreApps.InnerServer.Element.Helpers;

    #endregion

    internal partial class WiniumElement
    {
        #region Methods

        internal void SetProperty(string attributeName, JToken value)
        {
            PropertiesAccessor.SetProperty(this.Element, attributeName, value);
        }

        internal bool TryGetAutomationProperty(string automationPropertyName, out object value)
        {
            return AutomationPropertiesAccessor.TryGetAutomationProperty(
                this.Element, 
                automationPropertyName, 
                out value);
        }

        internal bool TryGetProperty(string attributeName, out object value)
        {
            return PropertiesAccessor.TryGetProperty(this.Element, attributeName, out value);
        }

        #endregion
    }
}
