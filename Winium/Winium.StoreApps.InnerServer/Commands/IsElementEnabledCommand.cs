namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using Winium.Mobile.Common;

    #endregion

    internal class IsElementEnabledCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Methods

        protected override string DoImpl()
        {
            var element = this.Automator.ElementsRegistry.GetRegisteredElement(this.ElementId);
            var enabled = element.IsEnabled;

            return this.JsonResponse(ResponseStatus.Success, enabled);
        }

        #endregion
    }
}
