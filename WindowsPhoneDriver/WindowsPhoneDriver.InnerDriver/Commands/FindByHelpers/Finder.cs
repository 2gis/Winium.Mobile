namespace WindowsPhoneDriver.InnerDriver.Commands.FindByHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media;

    internal class Finder
    {
        #region Methods

        internal static IEnumerable<DependencyObject> GetDescendantsBy(DependencyObject rootItem, By searchStrategy)
        {
            var predicate = searchStrategy.Predicate;

            return GetDescendantsByPredicate(rootItem, predicate);
        }

        private static IEnumerable<DependencyObject> GetDescendantsByPredicate(
            DependencyObject rootItem, 
            Predicate<DependencyObject> predicate)
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(rootItem);
            for (var i = 0; i < childrenCount; ++i)
            {
                var child = VisualTreeHelper.GetChild(rootItem, i);
                if (predicate(child))
                {
                    yield return child;
                }

                foreach (var grandChild in GetDescendantsByPredicate(child, predicate))
                {
                    yield return grandChild;
                }
            }
        }

        #endregion
    }
}
