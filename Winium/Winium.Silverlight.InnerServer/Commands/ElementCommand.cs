namespace Winium.Silverlight.InnerServer.Commands
{
    using System.Linq;
    using System.Windows;

    using Winium.Mobile.Common;
    using Winium.Mobile.Common.Exceptions;
    using Winium.Silverlight.InnerServer.Commands.FindByHelpers;

    internal class ElementCommand : CommandBase
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

            var searchStrategy = new By(searchPolicy, searchValue);
            var webObjectId = this.FindElementBy(relativeElement, searchStrategy);

            if (webObjectId == null)
            {
                throw new AutomationException("Element cannot be found", ResponseStatus.NoSuchElement);
            }

            var webElement = new JsonElementContent(webObjectId);
            return this.JsonResponse(ResponseStatus.Success, webElement);
        }

        #endregion

        #region Methods

        private string FindElementBy(DependencyObject relativeElement, By searchStrategy)
        {
            string foundId = null;
            var descendant = Finder.GetDescendantsBy(relativeElement, searchStrategy);
            if (relativeElement == this.Automator.VisualRoot)
            {
                descendant = descendant.Concat(Finder.GetPopupsDescendantsBy(searchStrategy));
            }

            var element = (FrameworkElement)descendant.FirstOrDefault();
            if (element != null)
            {
                foundId = this.Automator.WebElements.RegisterElement(element);
            }

            return foundId;
        }

        #endregion
    }
}
