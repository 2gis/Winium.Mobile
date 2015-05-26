namespace Samples
{
    #region

    using System;
    using System.Reflection;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Remote;

    #endregion

    public class WpDriver : RemoteWebDriver, IHasTouchScreen
    {
        #region Fields

        private ITouchScreen touchScreen;

        #endregion

        #region Constructors and Destructors

        public WpDriver(ICommandExecutor commandExecutor, ICapabilities desiredCapabilities)
            : base(commandExecutor, desiredCapabilities)
        {
        }

        public WpDriver(ICapabilities desiredCapabilities)
            : base(desiredCapabilities)
        {
        }

        public WpDriver(Uri remoteAddress, ICapabilities desiredCapabilities)
            : base(remoteAddress, desiredCapabilities)
        {
        }

        public WpDriver(Uri remoteAddress, ICapabilities desiredCapabilities, TimeSpan commandTimeout)
            : base(remoteAddress, desiredCapabilities, commandTimeout)
        {
        }

        #endregion

        #region Public Properties

        public ITouchScreen TouchScreen
        {
            get
            {
                return this.touchScreen ?? (this.touchScreen = new RemoteTouchScreen(this));
            }
        }

        #endregion

        private static object Invoke(object o, string methodName, params object[] args)
        {
            var mi = o.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            if (mi != null)
            {
                return mi.Invoke(o, args);
            }
            return null;
        }

        public IWebElement FindByXName(string locator)
        {
            return Invoke(this, "FindElement", "xname", locator) as IWebElement;
        }

        public static IWebElement FindByXName(IWebElement root, string locator)
        {
            var parentElement = root as RemoteWebElement;
            return Invoke(parentElement, "FindElement", "xname", locator) as IWebElement;
        }
    }
}
