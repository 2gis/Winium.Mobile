namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using System;
    using System.Reflection;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.Common.Exceptions;
    using Winium.StoreApps.InnerServer.Commands.Helpers;

    #endregion

    internal class GetElementAttributeCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string DoImpl()
        {
            var element = this.Automator.WebElements.GetRegisteredElement(this.ElementId);

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
            try
            {
                var propertyObject = element.GetAttribute(attributeName);

                return this.JsonResponse(ResponseStatus.Success, SerializeObjectAsString(propertyObject));
            }
            catch (AutomationException)
            {
                return this.JsonResponse();
            }
        }

        #endregion

        #region Methods

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

            // Serialize basic types as palin strings
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
