namespace Winium.Mobile.Driver.CommandExecutors
{
    #region

    using System;

    using Winium.Mobile.Connectivity.Emulator;

    #endregion

    internal class SetOrientationExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var value = this.ExecutedCommand.Parameters["orientation"].ToObject<string>();

            EmulatorController.PhoneOrientation orientation;
            Enum.TryParse(value, true, out orientation);

            this.Automator.EmulatorController.Orientation = orientation;

            return this.JsonResponse();
        }

        #endregion
    }
}
