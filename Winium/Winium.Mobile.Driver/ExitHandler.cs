namespace Winium.Mobile.Driver
{
    #region

    using System.Runtime.InteropServices;

    #endregion

    public static class ExitHandler
    {
        #region Static Fields

        private static EventHandler handler;

        #endregion

        #region Delegates

        public delegate bool EventHandler(CtrlType signal);

        #endregion

        #region Enums

        public enum CtrlType
        {
            CtrlCEvent = 0, 

            CtrlBreakEvent = 1, 

            CtrlCloseEvent = 2, 

            CtrlLogoffEvent = 5, 

            CtrlShutdownEvent = 6
        }

        #endregion

        #region Public Methods and Operators

        public static bool SetHandler(EventHandler exitHandler)
        {
            handler = exitHandler;
            return SetConsoleCtrlHandler(handler, true);
        }

        #endregion

        #region Methods

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        #endregion
    }
}
