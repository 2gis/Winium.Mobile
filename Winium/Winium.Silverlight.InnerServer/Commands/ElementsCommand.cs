namespace Winium.Silverlight.InnerServer.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;

    using Winium.Mobile.Common;
    using Winium.Silverlight.InnerServer.Commands.FindByHelpers;

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

            var result = new List<JsonElementContent>();
            var searchStrategy = new By(searchPolicy, searchValue);
            var foundObjectsIdList = this.FindElementsBy(relativeElement, searchStrategy);
            result.AddRange(foundObjectsIdList.Select(foundObjectId => new JsonElementContent(foundObjectId)));

            return this.JsonResponse(ResponseStatus.Success, result.ToArray());
        }

        #endregion

        #region Methods

        private IEnumerable<string> FindElementsBy(DependencyObject relativeElement, By searchStrategy)
        {
            var foundIds = new List<string>();
            var descendant = Finder.GetDescendantsBy(relativeElement, searchStrategy);
            if (relativeElement == this.Automator.VisualRoot)
            {
                descendant = descendant.Concat(Finder.GetPopupsDescendantsBy(searchStrategy));
            }

            foundIds.AddRange(
                descendant.Select(element => this.Automator.WebElements.RegisterElement((FrameworkElement)element)));
            return foundIds;
        }

        #endregion
    }
}
