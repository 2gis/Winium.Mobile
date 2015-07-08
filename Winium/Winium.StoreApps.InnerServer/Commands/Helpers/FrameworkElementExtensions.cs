namespace Winium.StoreApps.InnerServer.Commands.Helpers
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Newtonsoft.Json.Linq;

    using Windows.Foundation;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Automation;
    using Windows.UI.Xaml.Automation.Peers;
    using Windows.UI.Xaml.Media;

    using Winium.StoreApps.Common.Exceptions;

    #endregion

    internal static class FrameworkElementExtensions
    {
        #region Constants

        internal const string HelpNotSupportInterfaceMsg = "Element does not support {0} control pattern interface.";

        #endregion

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

        internal static AutomationPeer GetAutomationPeer(this FrameworkElement element)
        {
            var peer = FrameworkElementAutomationPeer.FromElement(element);
            if (peer == null)
            {
                throw new AutomationException("Element does not support AutomationPeer.");
            }

            return peer;
        }

        internal static Rect GetRect(this FrameworkElement element, UIElement visualRoot)
        {
            var point1 = element.TransformToVisual(visualRoot).TransformPoint(new Point(0, 0));
            var point2 = new Point(point1.X + element.ActualWidth, point1.Y + element.ActualHeight);

            var scrPoint1 = ScreenCoordinatesHelper.LogicalPointToScreenPoint(point1);
            var scrPoint2 = ScreenCoordinatesHelper.LogicalPointToScreenPoint(point2);

            return new Rect(scrPoint1, scrPoint2);
        }

        internal static Point GetCoordinates(this FrameworkElement element, UIElement visualRoot)
        {
            var point = element.TransformToVisual(visualRoot).TransformPoint(new Point(0, 0));
            var center = new Point(point.X + (int)(element.ActualWidth / 2), point.Y + (int)(element.ActualHeight / 2));
            return ScreenCoordinatesHelper.LogicalPointToScreenPoint(center);
        }

        internal static Point GetCoordinatesInView(this FrameworkElement element, UIElement visualRoot)
        {
            // TODO reasearch posibility to replace this code to GetClickablePoint()
            // FIXME Location returns wrong coordinates when apps screen slides up due to some input element getting focus #34
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

        internal static T GetProviderOrDefault<T>(this FrameworkElement element, PatternInterface patternInterface)
            where T : class
        {
            var peer = FrameworkElementAutomationPeer.FromElement(element);

            return peer == null ? null : peer.GetPattern(patternInterface) as T;
        }

        internal static T GetProvider<T>(this FrameworkElement element, PatternInterface patternInterface)
            where T : class
        {
            var provider = element.GetProviderOrDefault<T>(patternInterface);
            if (provider != null)
            {
                return provider;
            }

            throw new AutomationException(string.Format(HelpNotSupportInterfaceMsg, typeof(T).Name));
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
            if (elementSize.Width <= 0 || elementSize.Height <= 0)
            {
                return false;
            }

            var rect = new Rect(zero, elementSize);
            var bound = element.TransformToVisual(visualRoot).TransformBounds(rect);
            var rootRect = new Rect(zero, visualRoot.RenderSize);
            rootRect.Intersect(bound);

            // Check if element is offscreen. Save time on traversing tree if element is not in root rect
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
                    // There are some elements in UI tree that always return zero size, e.g. ContentControl, etc.
                    continue;
                }

                // FIXME we only check if element is visible in parent and parent visible in his parent and so on
                // we do not actully check if any part of original element is visible in grandparents
                elementSize = new Size(element.ActualWidth, element.ActualHeight);
                rect = new Rect(zero, elementSize);
                bound = element.TransformToVisual(container).TransformBounds(rect);
                var containerRect = new Rect(zero, container.RenderSize);
                
                containerRect.Intersect(bound);

                // Check if element is offscreen
                if (containerRect.IsEmpty)
                {
                    return false;
                }

                element = container;
            }
        }

        internal static void SetAttribute(this FrameworkElement element, string attributeName, JToken value)
        {
            object targetObject;
            PropertyInfo targetPropertyInfo;

            if (GetAttributeTarget(element, attributeName, out targetObject, out targetPropertyInfo))
            {
                targetPropertyInfo.SetValue(targetObject, value.ToObject(targetPropertyInfo.PropertyType));
            }
            else
            {
                throw new AutomationException("Could not access attribute {0}.", attributeName);
            }
        }

        #endregion
    }
}
