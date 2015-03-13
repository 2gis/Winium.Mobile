namespace Winium.StoreApps.InnerServer.Commands
{
    #region

    using System.Collections.Generic;
    using System.Linq;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.InnerServer.Commands.Helpers;

    #endregion

    internal class ElementsCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string DoImpl()
        {
            var searchValue = this.Parameters["value"].ToString();
            var searchPolicy = this.Parameters["using"].ToString();

            DependencyObject relativeElement = this.ElementId == null
                                                   ? this.Automator.VisualRoot
                                                   : this.Automator.WebElements.GetRegisteredElement(this.ElementId);

            var result = new List<JsonWebElementContent>();
            var searchStrategy = new By(searchPolicy, searchValue);
            var foundObjectsIdList = this.FindElementsBy(relativeElement, searchStrategy);
            result.AddRange(foundObjectsIdList.Select(foundObjectId => new JsonWebElementContent(foundObjectId)));

            if (this.ElementId == null)
            {
                var popups = VisualTreeHelper.GetOpenPopups(Window.Current);
                foreach (var popupChild in popups.Select(popup => popup.Child))
                {
                    foundObjectsIdList = this.FindElementsBy(popupChild, searchStrategy);
                    result.AddRange(
                        foundObjectsIdList.Select(foundObjectId => new JsonWebElementContent(foundObjectId)));
                }
            }

            return this.JsonResponse(ResponseStatus.Success, result.ToArray());
        }

        #endregion

        #region Methods

        private IEnumerable<string> FindElementsBy(DependencyObject relativeElement, By searchStrategy)
        {
            var foundIds = new List<string>();

            foundIds.AddRange(
                Finder.GetDescendantsBy(relativeElement, searchStrategy)
                    .Select(element => this.Automator.WebElements.RegisterElement((FrameworkElement)element)));
            return foundIds;
        }

        #endregion
    }
}
