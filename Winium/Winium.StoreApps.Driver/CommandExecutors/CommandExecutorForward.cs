﻿namespace Winium.StoreApps.Driver.CommandExecutors
{
    internal class CommandExecutorForward : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            return this.Automator.CommandForwarder.ForwardCommand(this.ExecutedCommand);
        }

        #endregion
    }
}
