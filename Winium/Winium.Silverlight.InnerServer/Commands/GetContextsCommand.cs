namespace Winium.Silverlight.InnerServer.Commands
{
    using System.Collections.Generic;
    using System.Linq;
    using Winium.Mobile.Common;

    internal class GetWebContextsCommand : CommandBase
    {
        private static readonly IEnumerable<string> UnconditionalContexts = new[] { DefaultContextNames.NativeAppContextName };
        public override string DoImpl()

        {
            var contexts = this.Automator.ContextsRegistry.GetAllContexts(this.Automator.browser);

            return this.JsonResponse(ResponseStatus.Success, UnconditionalContexts.Union(contexts));
        }
    }
}