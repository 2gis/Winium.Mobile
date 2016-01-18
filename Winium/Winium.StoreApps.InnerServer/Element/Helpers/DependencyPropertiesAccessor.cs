namespace Winium.StoreApps.InnerServer.Element.Helpers
{
    #region

    using System;
    using System.Linq;
    using System.Reflection;

    using Windows.UI.Xaml;

    #endregion

    internal static class DependencyPropertiesAccessor
    {
        #region Public Methods and Operators

        public static bool TryGetDependencyProperty(FrameworkElement element, string propertyName, out object value)
        {
            value = null;
            propertyName = string.Format("{0}Property", propertyName);

            var dependencyPorperty = GetDependencyProperty(element, element.GetType(), propertyName);

            if (dependencyPorperty == null)
            {
                return false;
            }

            value = element.GetValue(dependencyPorperty);
            return true;
        }

        #endregion

        #region Methods

        private static DependencyProperty GetDependencyProperty(DependencyObject obj, Type type, string name)
        {
            /* Despite  msdn stating that GetRuntimeFields and GetRuntimeProperties return inhereted fiels and properties,
             * it does not for some cases of dependency properties (e.g. ContentProperty and Button), so we will traverse inheritance manually
           */
            var dp = typeof(DependencyProperty);
            while (type != null)
            {
                var propertyInfo =
                    type.GetRuntimeProperties().FirstOrDefault(x => x.Name == name && x.PropertyType == dp);
                if (propertyInfo != null && propertyInfo.PropertyType == dp)
                {
                    return propertyInfo.GetValue(obj) as DependencyProperty;
                }

                var fieldInfo = type.GetRuntimeFields().FirstOrDefault(x => x.Name == name && x.FieldType == dp);
                if (fieldInfo != null && fieldInfo.FieldType == dp)
                {
                    return fieldInfo.GetValue(obj) as DependencyProperty;
                }

                type = type.GetTypeInfo().BaseType;
            }

            return null;
        }

        #endregion
    }
}
