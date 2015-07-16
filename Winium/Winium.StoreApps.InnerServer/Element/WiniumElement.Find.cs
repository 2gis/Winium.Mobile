namespace Winium.StoreApps.InnerServer.Element
{
    #region

    using System;
    using System.Collections.Generic;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;

    #endregion

    internal partial class WiniumElement : ISearchable
    {
        #region Public Methods and Operators

        public IEnumerable<WiniumElement> Find(TreeScope scope, Predicate<FrameworkElement> predicate)
        {
            if (!Enum.IsDefined(typeof(TreeScope), scope))
            {
                throw new ArgumentException("One of TreeScope.Children or TreeScope.Descendants should be set");
            }

            foreach (var descendant in IterDescendants(this.Element, !scope.HasFlag(TreeScope.Descendants)))
            {
                if (predicate(descendant))
                {
                    yield return new WiniumElement(descendant);
                }
            }
        }

        #endregion

        #region Methods

        private static IEnumerable<FrameworkElement> IterDescendants(FrameworkElement rootItem, bool childrenOnly)
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(rootItem);

            for (var i = 0; i < childrenCount; ++i)
            {
                var child = VisualTreeHelper.GetChild(rootItem, i) as FrameworkElement;

                yield return child;

                if (!childrenOnly)
                {
                    foreach (var descendant in IterDescendants(child, false))
                    {
                        yield return descendant;
                    }
                }
            }
        }

        #endregion
    }
}
