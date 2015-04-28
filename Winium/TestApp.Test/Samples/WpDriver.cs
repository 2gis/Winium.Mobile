namespace Samples
{
    using System;

    using OpenQA.Selenium;
    using OpenQA.Selenium.Remote;

    public class WpDriver : RemoteWebDriver, IHasTouchScreen
    {
        public WpDriver(ICommandExecutor commandExecutor, ICapabilities desiredCapabilities)
            : base(commandExecutor, desiredCapabilities)
        {
            TouchScreen = new RemoteTouchScreen(this);
        }

        public WpDriver(ICapabilities desiredCapabilities)
            : base(desiredCapabilities)
        {
            TouchScreen = new RemoteTouchScreen(this);
        }

        public WpDriver(Uri remoteAddress, ICapabilities desiredCapabilities)
            : base(remoteAddress, desiredCapabilities)
        {
            TouchScreen = new RemoteTouchScreen(this);
        }

        public WpDriver(Uri remoteAddress, ICapabilities desiredCapabilities, TimeSpan commandTimeout)
            : base(remoteAddress, desiredCapabilities, commandTimeout)
        {
            TouchScreen = new RemoteTouchScreen(this);
        }

        public ITouchScreen TouchScreen { get; private set; }
    }
}
