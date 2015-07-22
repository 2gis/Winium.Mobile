namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using Winium.StoreApps.Common;
    using Winium.StoreApps.InnerServer.Element;

    #endregion

    internal class TextCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Public Methods and Operators

        protected override string DoImpl()
        {
            var element = this.Automator.WebElements.GetRegisteredElement(this.ElementId);
            var text = element.GetText();

            return this.JsonResponse(ResponseStatus.Success, text);
        }

        #endregion
    }
}
