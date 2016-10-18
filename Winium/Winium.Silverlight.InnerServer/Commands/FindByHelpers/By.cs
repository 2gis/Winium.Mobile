namespace Winium.Silverlight.InnerServer.Commands.FindByHelpers
{
    using System;
    using System.Windows;

    internal class By
    {
        #region Constructors and Destructors

        public By(string strategy, string value)
        {
            if (strategy.Equals("tag name") || strategy.Equals("class name"))
            {
                this.Predicate = x => x.GetType().ToString().Equals(value);
            }
            else if (strategy.Equals("id"))
            {
                this.Predicate = x =>
                {
                    var automationId = (x as FrameworkElement).AutomationId();
                    return automationId != null && automationId.Equals(value);
                };
            }
            else if (strategy.Equals("name"))
            {
                this.Predicate = x =>
                    {
                        var automationName = (x as FrameworkElement).AutomationName();
                        return automationName != null && automationName.Equals(value);
                    };
            }
            else if (strategy.Equals("xname"))
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
                throw new NotImplementedException(string.Format("{0} is not valid or implemented searching strategy.", strategy));
            }
        }

        #endregion

        #region Public Properties

        public Predicate<DependencyObject> Predicate { get; private set; }

        #endregion
    }
}
