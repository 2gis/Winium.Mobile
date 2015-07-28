namespace Winium.StoreApps.InnerServer.Element
{
    #region

    using System;
    using System.Linq;
    using System.Reflection;

    using Newtonsoft.Json.Linq;

    using Winium.StoreApps.Common.Exceptions;

    #endregion

    internal partial class WiniumElement
    {
        #region Methods

        internal object GetAttribute(string attributeName)
        {
            object targetObject;
            PropertyInfo targetPropertyInfo;

            if (this.GetAttributeTarget(attributeName, out targetObject, out targetPropertyInfo))
            {
                return targetPropertyInfo.GetValue(targetObject, null);
            }

            throw new AutomationException("Could not access attribute {0}.", attributeName);
        }

        internal void SetAttribute(string attributeName, JToken value)
        {
            object targetObject;
            PropertyInfo targetPropertyInfo;

            if (this.GetAttributeTarget(attributeName, out targetObject, out targetPropertyInfo))
            {
                targetPropertyInfo.SetValue(targetObject, value.ToObject(targetPropertyInfo.PropertyType));
            }
            else
            {
                throw new AutomationException("Could not access attribute {0}.", attributeName);
            }
        }

        private bool GetAttributeTarget(
            string attributeName, 
            out object targetObject, 
            out PropertyInfo targetPropertyInfo)
        {
            targetObject = null;
            targetPropertyInfo = null;

            object parent = null;
            var curObject = (object)this.Element;
            PropertyInfo propertyInfo = null;

            var properties = attributeName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

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
