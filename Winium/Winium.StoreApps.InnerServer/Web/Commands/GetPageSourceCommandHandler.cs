namespace Winium.StoreApps.InnerServer.Web.Commands
{
    using System.Collections.Generic;

    internal class GetPageSourceCommandHandler : WebCommandAdapterHandler
    {
        protected override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            var executeScriptAtom = this.Atom;
            string script = "return document.documentElement.outerHTML;";
            string result = this.EvaluateAtom(environment, executeScriptAtom, script, new object[] { }, environment.CreateFrameObject());
            return Response.FromJson(result);
        }
    }
}
