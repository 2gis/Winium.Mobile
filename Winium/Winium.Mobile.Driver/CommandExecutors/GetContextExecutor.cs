namespace Winium.Mobile.Driver.CommandExecutors
{
    using Winium.Mobile.Common;

    #region

    

    #endregion

    internal class GetContextExecutor : CommandExecutorBase
    {
        protected override string DoImpl()
        {
            return this.JsonResponse(ResponseStatus.Success, this.Automator.CurrentContext);
        }
    }
}
