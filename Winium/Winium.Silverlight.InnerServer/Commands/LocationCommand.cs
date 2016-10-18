namespace Winium.Silverlight.InnerServer.Commands
{
    using System.Collections.Generic;

    using Winium.Mobile.Common;

    internal class LocationCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string DoImpl()
        {
            var element = this.Automator.WebElements.GetRegisteredElement(this.ElementId);
            var coordinates = element.GetCoordinates(this.Automator.VisualRoot);
            var coordinatesDict = new Dictionary<string, int>
                                      {
                                          { "x", (int)coordinates.X }, 
                                          { "y", (int)coordinates.Y }
                                      };

            return this.JsonResponse(ResponseStatus.Success, coordinatesDict);
        }

        #endregion
    }
}
