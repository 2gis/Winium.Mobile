namespace Winium.Mobile.Driver.CommandExecutors
{
    #region

    using System.Collections.Generic;

    using Winium.Mobile.Common;
    using Winium.Mobile.Driver.CommandHelpers;

    #endregion

    internal class StatusExecutor : CommandExecutorBase
    {
        #region Methods

        protected override string DoImpl()
        {
            var response = new Dictionary<string, object> { { "build", new BuildInfo() }, };
            return this.JsonResponse(ResponseStatus.Success, response);
        }

        #endregion
    }
}
