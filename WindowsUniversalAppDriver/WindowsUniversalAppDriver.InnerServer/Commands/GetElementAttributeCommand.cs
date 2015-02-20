namespace WindowsUniversalAppDriver.InnerServer.Commands
{
    using System;
    using System.Reflection;

    using Newtonsoft.Json;

    using WindowsUniversalAppDriver.Common;

    internal class GetElementAttributeCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string DoImpl()
        {
            var element = this.Automator.WebElements.GetRegisteredElement(this.ElementId);

            object value;
            string attributeName = null;
            if (this.Parameters.TryGetValue("NAME", out value))
            {
                attributeName = value.ToString();
            }

            if (attributeName == null)
            {
                return this.JsonResponse(ResponseStatus.Success, null);
            }

            var properties = attributeName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            object propertyObject = element;
            foreach (var property in properties)
            {
                propertyObject = GetProperty(propertyObject, property);
                if (propertyObject == null)
                {
                    break;
                }
            }

            /* GetAttribute command should return: null if no property was found,
             * property value as plain string if property is scalar or string,
             * JSON encoded property if property is Lists, Dictionary or other nonscalar types 
             */
            var propertyValue = SerializeObjectAsString(propertyObject);

            return this.JsonResponse(ResponseStatus.Success, propertyValue);
        }

        #endregion

        #region Methods

        private static object GetProperty(object obj, string propertyName)
        {
            var property = obj.GetType().GetRuntimeProperty(propertyName);

            return property == null ? null : property.GetValue(obj, null);
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
