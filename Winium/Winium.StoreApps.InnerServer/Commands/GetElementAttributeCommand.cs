namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using System;
    using System.Reflection;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    using Winium.Mobile.Common;
    using Winium.Mobile.Common.CommandSettings;
    using Winium.StoreApps.InnerServer.Commands.Helpers;
    using Winium.StoreApps.InnerServer.Element;

    #endregion

    internal class GetElementAttributeCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Methods

        internal static object GetPropertyCascade(
            WiniumElement element,
            string key,
            ElementAttributeAccessModifier options)
        {
            object propertyObject;
            if (element.TryGetAutomationProperty(key, out propertyObject))
            {
                return propertyObject;
            }

            if (options == ElementAttributeAccessModifier.AutomationProperties)
            {
                return null;
            }

            if (element.TryGetDependencyProperty(key, out propertyObject))
            {
                return propertyObject;
            }

            if (options == ElementAttributeAccessModifier.DependencyProperties)
            {
                return null;
            }

            if (element.TryGetProperty(key, out propertyObject))
            {
                return propertyObject;
            }

            return null;
        }

        protected override string DoImpl()
        {
            var element = this.Automator.ElementsRegistry.GetRegisteredElement(this.ElementId);

            string attributeName;
            if (!this.Parameters.TryGetValue("NAME", out attributeName))
            {
                return this.JsonResponse();
            }

            ElementAttributeCommandSettings settings;
            if (!this.Parameters.TryGetValue(CommandSettings.ElementAttributeSettingsParameter, out settings))
            {
                settings = new ElementAttributeCommandSettings();
            }

            /* GetAttribute command should return: null if no property was found,
             * property value as plain string if property is scalar or string,
             * JSON encoded property if property is Lists, Dictionary or other nonscalar types 
             */
            var value = GetPropertyCascade(element, attributeName, settings.AccessModifier);

            return this.JsonResponse(ResponseStatus.Success, SerializeObjectAsString(value, settings.EnumAsString));
        }

        private static bool IsTypeSerializedUsingToString(Type type)
        {
            // Strings should be serialized as plain strings
            return type == typeof(string) || type.GetTypeInfo().IsPrimitive;
        }

        private static string SerializeObjectAsString(object obj, bool enumAsString)
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

            if (obj is Enum)
            {
                return (obj as Enum).ToString(enumAsString ? "G" : "D");
            }

            // Serialize other data types as JSON
            var settings = new JsonSerializerSettings();
            if (enumAsString)
            {
                settings.Converters.Add(new StringEnumConverter());
            }

            return JsonConvert.SerializeObject(obj, settings);
        }

        #endregion
    }
}
