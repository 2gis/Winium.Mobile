namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using System.Collections.Generic;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.InnerServer.Element;

    #endregion

    internal class LocationCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Public Methods and Operators

        protected override string DoImpl()
        {
            var element = this.Automator.WebElements.GetRegisteredElement(this.ElementId);
            var coordinates = new WiniumElement(element).GetCoordinates();
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
