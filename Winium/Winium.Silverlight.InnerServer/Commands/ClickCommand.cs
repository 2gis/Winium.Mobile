namespace Winium.Silverlight.InnerServer.Commands
{
    using Winium.Mobile.Common;

    internal class ClickCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string DoImpl()
        {
            // Warning: this method does not actually click, it gets coordinates for use in driver.
            var element = this.Automator.WebElements.GetRegisteredElement(this.ElementId);

            // TODO: Replace with implementation using AutomationPeer
            var coordinates = element.GetCoordinates(this.Automator.VisualRoot);
            var strCoordinates = coordinates.X + ":" + coordinates.Y;

            return this.JsonResponse(ResponseStatus.Success, strCoordinates);
        }

        #endregion
    }
}
