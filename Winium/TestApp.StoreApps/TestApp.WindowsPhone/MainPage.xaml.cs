// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace TestApp
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Automation;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Navigation;

    using Winium.StoreApps.InnerServer;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly List<string> _monthsList = new List<string>
        {
            "January",
            "February",
            "March",
            "April",
            "May",
            "June",
            "July",
            "August",
            "September",
            "October",
            "November",
            "December",
            "Moses",
            "Homer",
            "Aristotle",
            "Archimedes",
            "Caesar",
            "Saint Paul",
            "Charlemagnev",
            "Dante",
            "Gutenberg",
            "Shakespeare",
            "Descartes",
            "Frederick",
            "Bichat"
        };

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            foreach (var month in _monthsList)
            {
                var textBlock = new TextBlock { Text = month };
                textBlock.SetValue(AutomationProperties.NameProperty, month);

                // ReSharper disable once PossibleNullReferenceException
                this.ListBox.Items.Add(textBlock);
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void SetButtonOnClick(object sender, RoutedEventArgs e)
        {
            TextBox.Text = "CARAMBA";
        }

        private void MainPageOnLoaded(object sender, RoutedEventArgs e)
        {
            AutomationServer.Instance.InitializeAndStart();
            Pivot.Title = string.Format("{0} StoreApps", AutomationServer.Instance.Port);
        }

        private void GoAppBarButtonOnClick(object sender, RoutedEventArgs e)
        {
            TextBox.Text = "Clicked GoAppBarButton";
        }

        private void FirstAppBarButtonOnClick(object sender, RoutedEventArgs e)
        {
            TextBox.Text = "Clicked FirstAppBarButton";
        }

        private void SecondAppBarButtonOnClick(object sender, RoutedEventArgs e)
        {
            TextBox.Text = "Clicked SecondAppBarButton";
        }

        private void AppBarOnOpened(object sender, object e)
        {
            TextBox.Text = "AppBar Opened";
        }

        private void AppBarOnClosed(object sender, object e)
        {
            TextBox.Text = "AppBar Closed";
        }

        private void SuggestionsSuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            Suggestions.Text = args.SelectedItem.ToString();
        }

        private void SuggestionsTextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suggestions = new List<string> { "A1", "A2", "A3" };
                sender.ItemsSource = suggestions;
            }
        }

        private async void ButtonClick(object sender, RoutedEventArgs e)
        {
            var noWifiDialog = new ContentDialog
            {
                Title = "No wifi connection",
                Content = "Check connection and try again",
                PrimaryButtonText = "Ok",
                SecondaryButtonText = "Cancel"
            };
            var result = await noWifiDialog.ShowAsync();

            this.SecondTabTextBox.Text = result == ContentDialogResult.Primary ? "Accepted" : "Dismissed";
        }
    }
}
