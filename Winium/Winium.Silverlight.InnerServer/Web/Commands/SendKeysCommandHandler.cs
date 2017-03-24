namespace Winium.Silverlight.InnerServer.Web.Commands
{
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class SendKeysCommandHandler : WebCommandHandler
    {
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            object element;
            if (!parameters.TryGetValue("ID", out element))
            {
                return Response.CreateMissingParametersResponse("ID");
            }

            object keys;
            if (!parameters.TryGetValue("value", out keys))
            {
                return Response.CreateMissingParametersResponse("value");
            }

            var keysAsString = string.Empty;
            //var keysAsArray = keys as object[];
            var keysAsArray = keys as JArray;
            if (keysAsArray != null)
            {
                keysAsString = keysAsArray.Cast<object>().Aggregate(keysAsString, (current, key) => current + key.ToString());
            }

            var typeAtom = this.Atom;

            // Normalize line endings to single line feed, as that's what the atom expects.
            keysAsString = keysAsString.Replace("\r\n", "\n");
            string result = this.EvaluateAtom(environment, typeAtom, element, keysAsString, environment.CreateFrameObject());
            return Response.FromJson(result);
        }
    }
}
