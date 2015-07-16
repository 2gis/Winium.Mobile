namespace Winium.StoreApps.InnerServer.Element
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;

    #endregion

    internal class WiniumVirtualRoot : ISearchable
    {
        #region Static Fields

        private static readonly Lazy<WiniumVirtualRoot> CurrentValue =
            new Lazy<WiniumVirtualRoot>(() => new WiniumVirtualRoot());

        #endregion

        #region Constructors and Destructors

        private WiniumVirtualRoot()
        {
            this.VisulaRoot = new WiniumElement(GetTrueVisualRoot());
        }

        #endregion

        #region Public Properties

        public static WiniumVirtualRoot Current
        {
            get
            {
                return CurrentValue.Value;
            }
        }

        public IEnumerable<WiniumElement> OpenPopups
        {
            get
            {
                var popups = VisualTreeHelper.GetOpenPopups(Window.Current).ToList();
                return popups.Select(x => new WiniumElement(x.Child as FrameworkElement));
            }
        }

        public WiniumElement VisulaRoot { get; private set; }

        #endregion

        #region Public Methods and Operators

        public IEnumerable<WiniumElement> Find(TreeScope scope, Predicate<FrameworkElement> predicate)
        {
            if (!Enum.IsDefined(typeof(TreeScope), scope))
            {
                throw new ArgumentException("One of TreeScope.Children or TreeScope.Descendants should be set");
            }

            // yield main visual tree
            if (predicate(this.VisulaRoot.Element))
            {
                yield return this.VisulaRoot;
            }

            if (scope.HasFlag(TreeScope.Descendants))
            {
                foreach (var element in this.VisulaRoot.Find(scope, predicate))
                {
                    yield return element;
                }
            }

            // yield popups (AppBar, etc.)
            var popups = VisualTreeHelper.GetOpenPopups(Window.Current);
            foreach (var popupChild in popups.Select(popup => new WiniumElement(popup.Child as FrameworkElement)))
            {
                if (predicate(this.VisulaRoot.Element))
                {
                    yield return popupChild;
                }

                if (scope.HasFlag(TreeScope.Descendants))
                {
                    foreach (var popupElement in popupChild.Find(scope, predicate))
                    {
                        yield return popupElement;
                    }
                }
            }
        }

        #endregion

        #region Methods

        private static FrameworkElement GetTrueVisualRoot()
        {
            var root = Window.Current.Content;
            while (true)
            {
                var parent = VisualTreeHelper.GetParent(root) as UIElement;
                if (parent == null)
                {
                    return root as FrameworkElement;
                }

                root = parent;
            }
        }

        #endregion
    }
}
