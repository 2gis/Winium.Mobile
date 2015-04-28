namespace Samples
{
    #region

    using System;

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
    }
}
