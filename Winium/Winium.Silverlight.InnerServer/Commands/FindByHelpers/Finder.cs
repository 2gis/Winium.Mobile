namespace Winium.Silverlight.InnerServer.Commands.FindByHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    
    internal class Finder
    {
        #region Methods

        public static IEnumerable<DependencyObject> GetDescendantsBy(DependencyObject rootItem, By searchStrategy)
        {
            var predicate = searchStrategy.Predicate;

            return GetDescendantsByPredicate(rootItem, predicate, false);
        }

        public static IEnumerable<DependencyObject> GetPopupsDescendantsBy(By searchStrategy)
        {
            var predicate = searchStrategy.Predicate;

            return GetPopupsDescendantsByPredicate(predicate, false);
        }

        public static IEnumerable<DependencyObject> GetPopupsChildren()
        {
            return GetPopupsDescendantsByPredicate(x => true, true);
        }

        public static IEnumerable<DependencyObject> GetChildren(DependencyObject rootItem)
        {
            return GetDescendantsByPredicate(rootItem, x => true, true);
        }

        private static IEnumerable<DependencyObject> GetPopupsDescendantsByPredicate(
            Predicate<DependencyObject> predicate,
            bool childrenOnly)
        {
            var popups = VisualTreeHelper.GetOpenPopups();
            return popups.SelectMany(popup => GetDescendantsByPredicate(popup.Child, predicate, childrenOnly));
        }

        private static IEnumerable<DependencyObject> GetDescendantsByPredicate(
            DependencyObject rootItem, 
            Predicate<DependencyObject> predicate,
            bool childrenOnly)
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(rootItem);
            for (var i = 0; i < childrenCount; ++i)
            {
                var child = VisualTreeHelper.GetChild(rootItem, i);
                if (predicate(child))
                {
                    yield return child;
                }

                if (childrenOnly)
                {
                    continue;
                }

                foreach (var grandChild in GetDescendantsByPredicate(child, predicate, false))
                {
                    yield return grandChild;
                }
            }
        }

        #endregion
    }
}
