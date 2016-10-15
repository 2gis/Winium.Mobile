namespace Winium.Silverlight.InnerServer.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Controls;
    using System.Windows.Media;

    internal static class FrameworkElementExtensions
    {
        #region Methods

        internal static string AutomationId(this FrameworkElement element)
        {
            return element == null ? null : element.GetValue(AutomationProperties.AutomationIdProperty) as string;
        }

        internal static string AutomationName(this FrameworkElement element)
        {
            return element == null ? null : element.GetValue(AutomationProperties.NameProperty) as string;
        }

        internal static Point GetCoordinates(this FrameworkElement element, UIElement visualRoot)
        {
            var point = element.TransformToVisual(visualRoot).Transform(new Point(0, 0));
            var center = new Point(point.X + (int)(element.ActualWidth / 2), point.Y + (int)(element.ActualHeight / 2));
            return center;
        }

        internal static Point GetCoordinatesInView(this FrameworkElement element, UIElement visualRoot)
        {
            var point = element.TransformToVisual(visualRoot).Transform(new Point(0, 0));
            var center = new Point(point.X + (int)(element.ActualWidth / 2), point.Y + (int)(element.ActualHeight / 2));
            var bounds = new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
            var boundsInView = new Rect(new Point(0, 0), visualRoot.RenderSize);
            boundsInView.Intersect(bounds);

            return boundsInView.IsEmpty ? center : new Point(boundsInView.X + (int)(boundsInView.Width / 2), boundsInView.Y + (int)(boundsInView.Height / 2));
        }

        internal static Rect GetRect(this FrameworkElement element, UIElement visualRoot)
        {
            var rect = new Rect(0, 0, element.ActualWidth, element.ActualHeight);
            return element.TransformToVisual(visualRoot).TransformBounds(rect);
        }

        internal static string GetText(this FrameworkElement element)
        {
            if (element is RichTextBox)
            {
                var rtb = element as RichTextBox;
                return rtb.Xaml;
            }

            var propertyNames = new List<string> { "Text", "Content" };

            foreach (var textProperty in from propertyName in propertyNames
                                         select element.GetType().GetProperty(propertyName)
                                         into textProperty
                                         where textProperty != null
                                         let value = textProperty.GetValue(element)
                                         where value is string
                                         select textProperty)
            {
                return textProperty.GetValue(element).ToString();
            }

            return string.Empty;
        }

        internal static bool IsUserVisible(this FrameworkElement element, UIElement visualRoot)
        {
            var zero = new Point(0, 0);
            var elementSize = new Size(element.ActualWidth, element.ActualHeight);

            // Check if element is of zero size
            if (!(elementSize.Width > 0 && elementSize.Height > 0))
            {
                return false;
            }

            var rect = new Rect(zero, elementSize);
            var bound = element.TransformToVisual(visualRoot).TransformBounds(rect);
            var rootRect = new Rect(zero, visualRoot.RenderSize);
            rootRect.Intersect(bound);

            // Check if element is offscreen
            if (rootRect.IsEmpty)
            {
                return false;
            }

            while (true)
            {
                if (element.Visibility != Visibility.Visible || !element.IsHitTestVisible || !(element.Opacity > 0))
                {
                    return false;
                }

                var container = VisualTreeHelper.GetParent(element) as FrameworkElement;
                if (container == null)
                {
                    return true;
                }

                if (container.RenderSize.IsEmpty)
                {
                    // There are some currentElements in UI tree that always return zero size, e.g. ContentControl, etc.
                    continue;
                }

                // FIXME we only check if currentElement is visible in parent and parent visible in his parent and so on
                // we do not actually check if any part of original currentElement is visible in grandparents
                elementSize = new Size(element.ActualWidth, element.ActualHeight);
                rect = new Rect(zero, elementSize);
                bound = element.TransformToVisual(container).TransformBounds(rect);
                var containerRect = new Rect(zero, container.RenderSize);

                containerRect.Intersect(bound);

                // Check if currentElement is offscreen
                if (containerRect.IsEmpty)
                {
                    return false;
                }

                element = container;
            }
        }

        #endregion
    }
}
