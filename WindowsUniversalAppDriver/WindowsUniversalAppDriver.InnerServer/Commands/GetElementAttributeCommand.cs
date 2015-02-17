namespace WindowsUniversalAppDriver.InnerServer.Commands
{
    using System;
    using System.Reflection;

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

            object propertyValueObject = element;
            foreach (var property in properties)
            {
                propertyValueObject = GetProperty(propertyValueObject, property);
                if (propertyValueObject == null)
                {
                    break;
                }
            }

            var propertyValue = propertyValueObject == null ? null : propertyValueObject.ToString();

            return this.JsonResponse(ResponseStatus.Success, propertyValue);
        }

        private static object GetProperty(object obj, string propertyName)
        {
            var property = obj.GetType().GetRuntimeProperty(propertyName);
            
            return property == null ? null : property.GetValue(obj, null);
        }

        #endregion
    }
}
