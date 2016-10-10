namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using Winium.Mobile.Common;

    #endregion

    internal class DisplayedCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Public Methods and Operators

        protected override string DoImpl()
        {
            var element = this.Automator.ElementsRegistry.GetRegisteredElement(this.ElementId);
            var displayed = element.IsUserVisible();

            return this.JsonResponse(ResponseStatus.Success, displayed);
        }

        #endregion
    }
}
