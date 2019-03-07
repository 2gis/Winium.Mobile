

namespace Winium.Silverlight.InnerServer.Web.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class ClickElementCommandHandler : WebCommandHandler
    {
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            object element;
            var clickAtom = this.Atom;
            if (!parameters.TryGetValue("ID", out element))
            {
                return Response.CreateMissingParametersResponse("ID");
            }

            this.SetAtomExecutionTimeout(TimeSpan.FromMilliseconds(100));
            string result = this.EvaluateAtom(environment, clickAtom, element, environment.CreateFrameObject());
            if (string.IsNullOrEmpty(result))
            {
                return Response.CreateSuccessResponse();
            }

            return Response.FromJson(result);
        }
    }
}
