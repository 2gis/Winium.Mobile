namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using System;
    using System.Reflection;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Winium.StoreApps.Common;

    #endregion

    internal class GetElementAttributeCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Methods

        protected override string DoImpl()
        {
            var element = this.Automator.ElementsRegistry.GetRegisteredElement(this.ElementId);

            JToken value;
            string attributeName = null;
            if (this.Parameters.TryGetValue("NAME", out value))
            {
                attributeName = value.ToString();
            }

            if (attributeName == null)
            {
                return this.JsonResponse();
            }

            /* GetAttribute command should return: null if no property was found,
             * property value as plain string if property is scalar or string,
             * JSON encoded property if property is Lists, Dictionary or other nonscalar types 
             */
            object propertyObject;

            if (!element.TryGetAutomationProperty(attributeName, out propertyObject))
            {
                element.TryGetProperty(attributeName, out propertyObject);
            }

            return this.JsonResponse(ResponseStatus.Success, SerializeObjectAsString(propertyObject));
        }

        private static bool IsTypeSerializedUsingToString(Type type)
        {
            // Strings should be serialized as plain strings
            return type == typeof(string) || type.GetTypeInfo().IsPrimitive;
        }

        private static string SerializeObjectAsString(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            // Serialize basic types as plain strings
            if (IsTypeSerializedUsingToString(obj.GetType()))
            {
                return obj.ToString();
            }

            // Serialize other data types as JSON
            return JsonConvert.SerializeObject(obj);
        }

        #endregion
    }
}
