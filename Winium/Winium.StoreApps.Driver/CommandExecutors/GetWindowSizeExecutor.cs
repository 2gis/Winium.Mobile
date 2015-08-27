namespace Winium.StoreApps.Driver.CommandExecutors
{
    #region

    using System.Collections.Generic;

    using Winium.StoreApps.Common;

    #endregion

    internal class GetWindowSizeExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var phoneScreenSize = this.Automator.EmulatorController.PhoneScreenSize;

            return this.JsonResponse(
                ResponseStatus.Success, 
                new Dictionary<string, int> { { "width", phoneScreenSize.Width }, { "height", phoneScreenSize.Height } });
        }

        #endregion
    }
}
