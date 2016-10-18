namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using System.Collections.Generic;
    using System.Linq;

    using Winium.Mobile.Common;
    using Winium.StoreApps.InnerServer.Commands.Helpers;
    using Winium.StoreApps.InnerServer.Element;

    #endregion

    internal class ElementsCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Methods

        protected override string DoImpl()
        {
            var searchValue = this.Parameters["value"].ToString();
            var searchPolicy = this.Parameters["using"].ToString();

            var searchStrategy = new By(searchPolicy, searchValue);
            List<WiniumElement> result;

            if (this.ElementId == null)
            {
                result = WiniumVirtualRoot.Current.Find(TreeScope.Descendants, searchStrategy.Predicate).ToList();
            }
            else
            {
                var parentElement = this.Automator.ElementsRegistry.GetRegisteredElement(this.ElementId);
                result = parentElement.Find(TreeScope.Descendants, searchStrategy.Predicate).ToList();
            }

            var registredObjects = new List<JsonElementContent>();

            foreach (var winiumElement in result)
            {
                var registeredKey = this.Automator.ElementsRegistry.RegisterElement(winiumElement);
                registredObjects.Add(new JsonElementContent(registeredKey));
            }

            return this.JsonResponse(ResponseStatus.Success, registredObjects.ToArray());
        }

        #endregion
    }
}
