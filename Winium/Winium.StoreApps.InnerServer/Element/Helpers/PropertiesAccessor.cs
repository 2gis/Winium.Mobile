namespace Winium.StoreApps.InnerServer.Element.Helpers
{
    #region

    using System;
    using System.Linq;
    using System.Reflection;

    using Newtonsoft.Json.Linq;

    using Windows.UI.Xaml;

    using Winium.Mobile.Common.Exceptions;

    #endregion

    internal static class PropertiesAccessor
    {
        #region Public Methods and Operators

        public static void SetProperty(FrameworkElement element, string propertyName, JToken value)
        {
            object targetObject;
            PropertyInfo targetPropertyInfo;

            if (GetPropertyTarget(element, propertyName, out targetObject, out targetPropertyInfo))
            {
                targetPropertyInfo.SetValue(targetObject, value.ToObject(targetPropertyInfo.PropertyType));
            }
            else
            {
                throw new AutomationException("Could not access attribute {0}.", propertyName);
            }
        }

        public static bool TryGetProperty(FrameworkElement element, string propertyName, out object value)
        {
            value = null;
            object targetObject;
            PropertyInfo targetPropertyInfo;

            if (!GetPropertyTarget(element, propertyName, out targetObject, out targetPropertyInfo))
            {
                return false;
            }

            value = targetPropertyInfo.GetValue(targetObject, null);
            return true;
        }

        #endregion

        #region Methods

        private static bool GetPropertyTarget(
            object sourceObject, 
            string propertyName, 
            out object targetObject, 
            out PropertyInfo targetPropertyInfo)
        {
            targetObject = null;
            targetPropertyInfo = null;

            object parent = null;
            var curObject = sourceObject;
            PropertyInfo propertyInfo = null;

            var properties = propertyName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (!properties.Any())
            {
                return false;
            }

            foreach (var property in properties)
            {
                parent = curObject;
                propertyInfo = curObject.GetType().GetRuntimeProperty(property);
                if (propertyInfo == null)
                {
                    return false;
                }

                curObject = propertyInfo.GetValue(curObject, null);
            }

            targetObject = parent;
            targetPropertyInfo = propertyInfo;

            return true;
        }

        #endregion
    }
}
