namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using System.Linq;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.Common.Exceptions;
    using Winium.StoreApps.InnerServer.Commands.Helpers;
    using Winium.StoreApps.InnerServer.Element;

    #endregion

    internal class ElementCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Public Methods and Operators

        protected override string DoImpl()
        {
            var searchValue = this.Parameters["value"].ToString();
            var searchPolicy = this.Parameters["using"].ToString();

            var searchStrategy = new By(searchPolicy, searchValue);

            WiniumElement winiumElement;

            if (this.ElementId == null)
            {
                winiumElement  = WiniumVirtualRoot.Current.Find(TreeScope.Descendants, searchStrategy.Predicate).FirstOrDefault();
            }
            else
            {
                var parentElement = this.Automator.ElementsRegistry.GetRegisteredElement(this.ElementId);
                winiumElement = parentElement.Find(TreeScope.Descendants, searchStrategy.Predicate).FirstOrDefault();
            }

            if (winiumElement == null)
            {
                throw new AutomationException("Element cannot be found", ResponseStatus.NoSuchElement);
            }

            var webObjectId = this.Automator.ElementsRegistry.RegisterElement(winiumElement);

            var webElement = new JsonWebElementContent(webObjectId);
            return this.JsonResponse(ResponseStatus.Success, webElement);
        }

        #endregion
    }
}
