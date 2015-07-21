namespace Winium.StoreApps.InnerServer.Element
{
    #region

    using Windows.Foundation;

    using Winium.StoreApps.InnerServer.Commands.Helpers;

    #endregion

    internal partial class WiniumElement
    {
        #region Public Methods and Operators

        public Point GetCoordinates()
        {
            var visualRoot = WiniumVirtualRoot.Current.VisualRoot.Element;
            var element = this.Element;

            var point = element.TransformToVisual(visualRoot).TransformPoint(new Point(0, 0));
            var center = new Point(point.X + (int)(element.ActualWidth / 2), point.Y + (int)(element.ActualHeight / 2));
            return ScreenCoordinatesHelper.LogicalPointToScreenPoint(center);
        }

        public Point GetCoordinatesInView()
        {
            // TODO reasearch posibility to replace this code to GetClickablePoint()
            var visualRoot = WiniumVirtualRoot.Current.VisualRoot.Element;
            var element = this.Element;

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

        #endregion

        #region Methods

        internal Rect GetRect()
        {
            var visualRoot = WiniumVirtualRoot.Current.VisualRoot.Element;
            var element = this.Element;

            var point1 = element.TransformToVisual(visualRoot).TransformPoint(new Point(0, 0));
            var point2 = new Point(point1.X + element.ActualWidth, point1.Y + element.ActualHeight);

            var scrPoint1 = ScreenCoordinatesHelper.LogicalPointToScreenPoint(point1);
            var scrPoint2 = ScreenCoordinatesHelper.LogicalPointToScreenPoint(point2);

            return new Rect(scrPoint1, scrPoint2);
        }

        #endregion
    }
}
