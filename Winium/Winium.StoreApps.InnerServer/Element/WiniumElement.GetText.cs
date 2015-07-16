namespace Winium.StoreApps.InnerServer.Element
{
    #region

    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    #endregion

    internal partial class WiniumElement
    {
        #region Methods

        internal string GetText()
        {
            var element = this.Element;

            var propertyNames = new List<string> { "Text", "Content" };

            foreach (var textProperty in from propertyName in propertyNames
                                         select element.GetType().GetRuntimeProperty(propertyName)
                                         into textProperty
                                         where textProperty != null
                                         let value = textProperty.GetValue(element)
                                         where value is string
                                         select textProperty)
            {
                return textProperty.GetValue(element).ToString();
            }

            return string.Empty;
        }

        #endregion
    }
}
