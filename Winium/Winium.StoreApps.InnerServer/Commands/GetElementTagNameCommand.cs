namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using Winium.StoreApps.Common;

    #endregion

    internal class GetElementTagNameCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Public Methods and Operators

        protected override string DoImpl()
        {
            var element = this.Automator.WebElements.GetRegisteredElement(this.ElementId);
            var tagName = element.GetType().ToString();

            return this.JsonResponse(ResponseStatus.Success, tagName);
        }

        #endregion
    }
}
