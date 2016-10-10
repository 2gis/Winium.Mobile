namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using Winium.Mobile.Common;

    #endregion

    internal class GetElementTagNameCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Public Methods and Operators

        protected override string DoImpl()
        {
            var element = this.Automator.ElementsRegistry.GetRegisteredElement(this.ElementId);

            return this.JsonResponse(ResponseStatus.Success, element.ClassName);
        }

        #endregion
    }
}
