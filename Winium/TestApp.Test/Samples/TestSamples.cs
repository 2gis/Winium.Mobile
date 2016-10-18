namespace Samples
{
    #region

    using System;
    using System.Diagnostics;
    using System.IO;

    using NUnit.Framework;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Interactions;
    using OpenQA.Selenium.Remote;
    using OpenQA.Selenium.Support.UI;

    #endregion

    [TestFixture]
    public class TestSamples
    {
        #region Fields

        private WpDriver driver;

        #endregion

        #region Public Methods and Operators

        [TearDown]
        public void CleanUp()
        {
            this.driver.Quit();
        }

        [SetUp]
        public void Initialize()
        {
            var capabillities = new DesiredCapabilities();

            var basePath = Directory.GetCurrentDirectory();
            const string FilePath =
                "..\\..\\..\\..\\TestApp.StoreApps\\TestApp.WindowsPhone\\AppPackages\\TestApp.WindowsPhone_1.0.0.0_AnyCPU_Test\\TestApp.WindowsPhone_1.0.0.0_AnyCPU.appx";
            var autPath = Path.GetFullPath(Path.Combine(basePath, FilePath));

            capabillities.SetCapability("app", autPath);

            this.driver = new WpDriver(new Uri("http://localhost:9999"), capabillities);
        }

        [Test]
        public void TestFindChilds()
        {
            var parent = this.driver.FindElementById("MyListBox");
            var elements = parent.FindElements(By.ClassName("Windows.UI.Xaml.Controls.TextBlock"));

            Assert.AreEqual(21, elements.Count);
        }

        [Test]
        public void TestScreenshot()
        {
            var scr = this.driver.GetScreenshot().AsBase64EncodedString;

            Assert.AreNotEqual(string.Empty, scr);
        }

        [Test]
        public void TestTouchActions()
        {
            var touches = new TouchActions(this.driver);

            var pivot = this.driver.FindElementById("MainPivot");

            touches.Flick(pivot, -300, 0, 500).Perform();

            var wait = new WebDriverWait(this.driver, TimeSpan.FromSeconds(5));
            var element = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("SecondTabTextBox")));
            Assert.AreEqual("Nice swipe!", element.Text);
        }

        #endregion
    }
}
