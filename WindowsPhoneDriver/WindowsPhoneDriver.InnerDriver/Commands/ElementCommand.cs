namespace WindowsPhoneDriver.InnerDriver.Commands
{
    using System.Linq;
    using System.Windows;

    using WindowsPhoneDriver.Common;
    using WindowsPhoneDriver.Common.Exceptions;
    using WindowsPhoneDriver.InnerDriver.Commands.FindByHelpers;

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

            var webElement = new JsonWebElementContent(webObjectId);
            return this.JsonResponse(ResponseStatus.Success, webElement);
        }

        #endregion

        #region Methods

        private string FindElementBy(DependencyObject relativeElement, By searchStrategy)
        {
            string foundId = null;

            var element = (FrameworkElement)Finder.GetDescendantsBy(relativeElement, searchStrategy).FirstOrDefault();
            if (element != null)
            {
                foundId = this.Automator.WebElements.RegisterElement(element);
            }

            return foundId;
        }

        #endregion
    }
}
