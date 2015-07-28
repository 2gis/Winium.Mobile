namespace Winium.StoreApps.InnerServer.Commands.Helpers
{
    #region

    using System;

    using Windows.UI.Xaml;

    #endregion

    internal class By
    {
        #region Constants

        private const string ClassNameStrategy = "class name";

        private const string IdStrategy = "id";

        private const string NameStrategy = "name";

        private const string TagNameStrategy = "tag name";

        private const string XNameStrategy = "xname";

        #endregion

        #region Constructors and Destructors

        public By(string strategy, string value)
        {
            if (strategy.Equals(TagNameStrategy) || strategy.Equals(ClassNameStrategy))
            {
                this.Predicate = x =>
                    {
                        var className = (x as FrameworkElement).ClassName();
                        return className.Equals(value);
                    };
            }
            else if (strategy.Equals(IdStrategy))
            {
                this.Predicate = x =>
                    {
                        var automationId = (x as FrameworkElement).AutomationId();
                        return automationId != null && automationId.Equals(value);
                    };
            }
            else if (strategy.Equals(NameStrategy))
            {
                this.Predicate = x =>
                    {
                        var automationName = (x as FrameworkElement).AutomationName();
                        return automationName != null && automationName.Equals(value);
                    };
            }
            else if (strategy.Equals(XNameStrategy))
            {
                // TODO: transitional. to be depricated
                this.Predicate = x =>
                    {
                        var frameworkElement = x as FrameworkElement;
                        return frameworkElement != null && frameworkElement.Name.Equals(value);
                    };
            }
            else
            {
                throw new NotImplementedException(
                    string.Format("{0} is not valid or implemented searching strategy.", strategy));
            }
        }

        #endregion

        #region Public Properties

        public Predicate<DependencyObject> Predicate { get; private set; }

        #endregion

        #region Public Methods and Operators

        public static By ClassName(string value)
        {
            return new By(ClassNameStrategy, value);
        }

        public static By XName(string value)
        {
            return new By(XNameStrategy, value);
        }

        #endregion
    }
}
