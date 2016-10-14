using Winium.StoreApps.InnerServer.Commands.Helpers;

namespace Winium.StoreApps.InnerServer.Element.Helpers
{
    #region

    using System;
    using System.Linq;
    using System.Reflection;

    using Newtonsoft.Json.Linq;

    using Windows.UI.Xaml;

    using Winium.StoreApps.Common.Exceptions;

    #endregion

    internal static class ExtensionPropertyAccessor
    {
        #region Public Methods and Operators

        public static bool TryGetProperty(FrameworkElement element, string propertyName, out object value)
        {
            value = GetExtensionProperty(propertyName, element);

            return value != null;
        }

        #endregion

        #region Methods

        private static object GetExtensionProperty(string propertyName, FrameworkElement element)
        {
            MethodInfo method = typeof(FrameworkElementExtensions).GetRuntimeMethods().First(m => m.Name.Equals(propertyName));
            object[] parameters = {element };
            return method?.Invoke(null, parameters);
        }

        #endregion
    }
}