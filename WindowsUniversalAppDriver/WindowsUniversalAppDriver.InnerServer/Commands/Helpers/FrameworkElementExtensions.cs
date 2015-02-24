namespace WindowsUniversalAppDriver.InnerServer.Commands.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Newtonsoft.Json.Linq;

    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Automation;
    using Windows.UI.Xaml.Media;

    using WindowsUniversalAppDriver.Common.Exceptions;

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

        internal static object GetAttribute(this FrameworkElement element, string attributeName)
        {
            object targetObject;
            PropertyInfo targetPropertyInfo;

            if (GetAttributeTarget(element, attributeName, out targetObject, out targetPropertyInfo))
            {
                return targetPropertyInfo.GetValue(targetObject, null);
            }

            throw new AutomationException("Could not access attribute {0}.", attributeName);
        }

        internal static bool GetAttributeTarget(
            this object element, 
            string attributeName, 
            out object targetObject, 
            out PropertyInfo targetPropertyInfo)
        {
            targetObject = null;
            targetPropertyInfo = null;

            object parent = null;
            var curObject = element;
            PropertyInfo propertyInfo = null;

            var properties = attributeName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (!properties.Any())
            {
                return false;
            }

            foreach (var property in properties)
            {
                parent = curObject;
                propertyInfo = curObject.GetType().GetRuntimeProperty(property);
                if (propertyInfo == null)
                {
                    return false;
                }

                curObject = propertyInfo.GetValue(curObject, null);
            }

            targetObject = parent;
            targetPropertyInfo = propertyInfo;

            return true;
        }

        internal static Point GetCoordinates(this FrameworkElement element, UIElement visualRoot)
        {
            var point = element.TransformToVisual(visualRoot).TransformPoint(new Point(0, 0));
            var center = new Point(point.X + (int)(element.ActualWidth / 2), point.Y + (int)(element.ActualHeight / 2));
            return ScreenCoordinatesHelper.LogicalPointToScreenPoint(center);
        }

        internal static Point GetCoordinatesInView(this FrameworkElement element, UIElement visualRoot)
        {
            var point = element.TransformToVisual(visualRoot).TransformPoint(new Point(0, 0));
            var center = new Point(point.X + (int)(element.ActualWidth / 2), point.Y + (int)(element.ActualHeight / 2));
            var bounds = new Rect(point, new Size(element.ActualWidth, element.ActualHeight));
            var boundsInView = new Rect(new Point(0, 0), visualRoot.RenderSize);
            boundsInView.Intersect(bounds);

            var result = boundsInView.IsEmpty
                             ? center
                             : new Point(
                                   boundsInView.X + (int)(boundsInView.Width / 2), 
                                   boundsInView.Y + (int)(boundsInView.Height / 2));
            return ScreenCoordinatesHelper.LogicalPointToScreenPoint(result);
        }

        internal static string GetText(this FrameworkElement element)
        {
            var propertyNames = new List<string> { "Text", "Content" };

            foreach (var textProperty in from propertyName in propertyNames
                                         select element.GetType().GetRuntimeProperty(propertyName)
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

                element = container;
            }
        }

        internal static bool SetAttribute(this FrameworkElement element, string attributeName, JToken value)
        {
            object targetObject;
            PropertyInfo targetPropertyInfo;

            if (GetAttributeTarget(element, attributeName, out targetObject, out targetPropertyInfo))
            {
                targetPropertyInfo.SetValue(targetObject, value.ToObject(targetPropertyInfo.PropertyType));
            }

            throw new AutomationException("Could not access attribute {0}.", attributeName);
        }

        #endregion
    }
}
