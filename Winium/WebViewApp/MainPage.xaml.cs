// The WebView Application template is documented at http://go.microsoft.com/fwlink/?LinkID=391641
namespace WebViewApp
{
    #region

    using System;
    using System.Diagnostics;

    using Windows.Phone.UI.Input;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;

    using Winium.StoreApps.InnerServer;

    #endregion

    public sealed partial class MainPage : Page
    {
        private static readonly Uri HomeUri = new Uri("https://github.com/2gis/Winium.Mobile", UriKind.Absolute);

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= this.MainPageBackPressed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.WebViewControl.Navigate(HomeUri);

            HardwareButtons.BackPressed += this.MainPageBackPressed;
        }

        private void BrowserNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            if (!args.IsSuccess)
            {
                Debug.WriteLine("Navigation to this page failed, check your internet connection.");
            }
        }

        private void ForwardAppBarButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.WebViewControl.CanGoForward)
            {
                this.WebViewControl.GoForward();
            }
        }

        private void HomeAppBarButtonClick(object sender, RoutedEventArgs e)
        {
            this.WebViewControl.Navigate(HomeUri);
        }

        private void MainPageBackPressed(object sender, BackPressedEventArgs e)
        {
            if (!this.WebViewControl.CanGoBack)
            {
                return;
            }

            this.WebViewControl.GoBack();
            e.Handled = true;
        }

        private void MainPageOnLoaded(object sender, RoutedEventArgs e)
        {
            AutomationServer.Instance.InitializeAndStart();
        }
    }
}
