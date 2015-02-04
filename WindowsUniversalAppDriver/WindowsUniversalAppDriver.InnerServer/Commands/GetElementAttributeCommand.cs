namespace WindowsUniversalAppDriver.InnerServer.Commands
{
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
            var attributeName = (string)null;
            if (this.Parameters.TryGetValue("NAME", out value))
            {
                attributeName = value.ToString();
            }

            if (attributeName == null)
            {
                return this.JsonResponse(ResponseStatus.Success, null);
            }

            var property = element.GetType().GetRuntimeProperty(attributeName);
            if (property == null)
            {
                return this.JsonResponse(ResponseStatus.Success, null);
            }

            var attributeValue = property.GetValue(element, null).ToString();

            return this.JsonResponse(ResponseStatus.Success, attributeValue);
        }

        #endregion
    }
}
