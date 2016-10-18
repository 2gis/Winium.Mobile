﻿namespace Winium.Mobile.Driver.CommandExecutors
{
    using System;
    using System.Threading;

    using Winium.Mobile.Common;
    using Winium.Mobile.Driver.Automator;

    internal class CloseAppExecutor : CommandExecutorBase
    {
        #region Public Methods and Operators

        public static void CloseApp(Automator automator)
        {
            var remoteCommand = new Command(DriverCommand.CloseApp);
            automator.CommandForwarder.ForwardCommand(remoteCommand);
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
        }

        #endregion

        #region Methods

        protected override string DoImpl()
        {
            CloseApp(this.Automator);

            return this.JsonResponse();
        }

        #endregion
    }
}
