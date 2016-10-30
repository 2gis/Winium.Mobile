namespace Winium.StoreApps.InnerServer.Commands
{
    using System.Collections.Generic;
    using System.Linq;

    using Winium.Mobile.Common;

    class GetWebContextsCommand : CommandBase
    {
        private static readonly IEnumerable<string> UnconditionalContexts = new[] { DefaultContextNames.NativeAppContextName };
        protected override string DoImpl()

        {
            var contexts = this.Automator.ContextsRegistry.GetAllContexts();

            return this.JsonResponse(ResponseStatus.Success, UnconditionalContexts.Union(contexts));
        }
    }
}
