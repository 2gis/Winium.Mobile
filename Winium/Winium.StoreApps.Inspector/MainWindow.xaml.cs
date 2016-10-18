namespace Winium.StoreApps.Inspector
{
    #region

    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Input;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using System.Xml;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Remote;

    #endregion

    public partial class MainWindow : INotifyPropertyChanged
    {
        #region Constants

        private const string WiniumStoreAppsDriver = "Winium.Mobile.Driver";

        #endregion

        #region Fields

        private string treeFilter;

        private string commandExecutor;

        private IWebDriver driver;

        private bool isBusy;

        private bool isConnected;

        private XmlDocument pageSourceData;

        private BitmapImage phoneScreenData;

        private XmlElement selectedNode;

        #endregion

        #region Constructors and Destructors

        public MainWindow()
        {
            this.InitializeComponent();
            this.DataContext = this;
            this.CommandExecutor = ConfigurationManager.AppSettings["RemoteDriverUri"];
            this.RefreshCommand = new RelayCommand(this.Refresh, this.CanExecuteRefreshCommand);
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public string TreeFilter
        {
            get
            {
                return this.treeFilter;
            }

            set
            {
                this.treeFilter = value;

                this.RaisePropertyChanged();
            }
        }

        public string CommandExecutor
        {
            get
            {
                return this.commandExecutor;
            }

            set
            {
                this.commandExecutor = value;

                this.RaisePropertyChanged();
            }
        }

        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                this.isBusy = value;

                this.RaisePropertyChanged();
            }
        }

        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }

            set
            {
                this.isConnected = value;

                this.RaisePropertyChanged();
            }
        }

        public XmlDocument PageSourceData
        {
            get
            {
                return this.pageSourceData;
            }

            set
            {
                this.pageSourceData = value;
                var dp = (XmlDataProvider)this.FindResource("XmlDp");
                dp.Document = this.pageSourceData;
                dp.XPath = "*";
            }
        }

        public BitmapImage PhoneScreenData
        {
            get
            {
                return this.phoneScreenData;
            }

            set
            {
                this.phoneScreenData = value;
                this.RaisePropertyChanged();
            }
        }

        public ICommand RefreshCommand { get; internal set; }

        public XmlElement SelectedNode
        {
            get
            {
                return this.selectedNode;
            }

            set
            {
                this.selectedNode = value;
                this.RaisePropertyChanged();
            }
        }

        #endregion

        #region Properties

        private string AppxPath { get; set; }

        #endregion

        #region Public Methods and Operators

        public static void ProcessUiTasks()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background, 
                new DispatcherOperationCallback(
                    delegate
                        {
                            frame.Continue = false;
                            return null;
                        }), 
                null);
            Dispatcher.PushFrame(frame);
        }

        #endregion

        #region Methods

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static bool IsDriverRunning()
        {
            var pname = Process.GetProcessesByName(WiniumStoreAppsDriver);
            return pname.Length != 0;
        }

        private bool CanExecuteRefreshCommand()
        {
            return !this.IsBusy && this.IsConnected;
        }

        private void CommandBindingCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBindingExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var selectedAttribute = this.PropertiesListView.SelectedItem as XmlAttribute;
            if (selectedAttribute != null)
            {
                Clipboard.SetText(selectedAttribute.Value);
            }
        }

        private void ConnectButtonClick(object sender, RoutedEventArgs e)
        {
            this.Deploy();
        }

        private bool ConnectToDriver(ICapabilities dc)
        {
            try
            {
                this.driver = new RemoteWebDriver(new Uri(this.CommandExecutor), dc);
                return true;
            }
            catch (Exception e)
            {
                var message = IsDriverRunning()
                                  ? e.Message
                                  : string.Format(
                                      CultureInfo.CurrentCulture, 
                                      "It seems, {0} is not running on {1}.\n\n{2}", 
                                      WiniumStoreAppsDriver, 
                                      this.CommandExecutor, 
                                      e.Message);
                this.Dispatcher.Invoke(
                    () => { MessageBox.Show(this, message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); });

                return false;
            }
        }

        private async void Deploy(string app = null)
        {
            var dc = new DesiredCapabilities();
            if (app == null)
            {
                dc.SetCapability("debugConnectToRunningApp", true);
            }
            else
            {
                dc.SetCapability("app", app);
            }

            this.IsBusy = true;
            this.TryKillDriver(app != null);

            this.IsConnected = await Task.Run(() => this.ConnectToDriver(dc));

            this.IsBusy = false;

            if (this.IsConnected)
            {
                this.Refresh();
            }
        }

        private void DeployButtonClick(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
                          {
                              DefaultExt = ".appx", 
                              Filter = "Appx package (.appx)|*.appx"
                          };

            var result = dlg.ShowDialog();

            if (result != true)
            {
                return;
            }

            this.AppxPath = dlg.FileName;
            this.Deploy(this.AppxPath);
        }

        private void PageSourceTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.SelectedNode = e.NewValue as XmlElement;
        }

        private void Refresh()
        {
            try
            {
                this.IsBusy = true;

                ProcessUiTasks();

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(this.driver.PageSource);
                this.PageSourceData = xmlDoc;

                var screenShot = ((ITakesScreenshot)this.driver).GetScreenshot();
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.StreamSource = new MemoryStream(screenShot.AsByteArray);
                bi.EndInit();

                this.PhoneScreenData = bi;

                this.IsBusy = false;
            }
            catch (Exception e)
            {
                this.IsBusy = false;

                this.Dispatcher.Invoke(
                    () => { MessageBox.Show(this, e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error); });
            }
        }

        private void RefreshButtonClick(object sender, RoutedEventArgs e)
        {
            this.Refresh();
        }

        private void ScopeToNodeClick(object sender, RoutedEventArgs e)
        {
            var dp = (XmlDataProvider)this.FindResource("XmlDp");
            dp.XPath = XPathHelpers.FindXPath(this.SelectedNode);
        }

        private void ScopeToRootClick(object sender, RoutedEventArgs e)
        {
            var dp = (XmlDataProvider)this.FindResource("XmlDp");
            dp.XPath = "*";
        }

        private void TryKillDriver(bool quit = true)
        {
            if (this.driver == null)
            {
                return;
            }

            try
            {
                if (quit)
                {
                    this.driver.Quit();
                }

                this.driver = null;
                this.IsConnected = false;
            }
            catch (WebDriverException)
            {
                // TODO add logging or something
            }
        }

        #endregion

        private void ApplyFilterClick(object sender, RoutedEventArgs e)
        {
            var dp = (XmlDataProvider)this.FindResource("XmlDp");
            dp.Document = this.pageSourceData;
            dp.XPath = this.treeFilter;
        }
    }
}
