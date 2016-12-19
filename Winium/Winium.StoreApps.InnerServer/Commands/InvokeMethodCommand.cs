namespace Winium.StoreApps.InnerServer.Commands
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Newtonsoft.Json.Linq;

    using Winium.Mobile.Common;

    internal class InvokeMethodCommand : CommandBase
    {
        protected override string DoImpl()
        {
            JToken typeParam;
            if (!this.Parameters.TryGetValue("type", out typeParam))
            {
                return this.JsonResponse(ResponseStatus.JavaScriptError, "specify fully qualified type name in 'type' parameter");
            }

            JToken methodParam;
            if (!this.Parameters.TryGetValue("method", out methodParam))
            {
                return this.JsonResponse(ResponseStatus.JavaScriptError, "specify fully qualified type name in 'method' parameter");
            }

            JToken argsParam;
            var args = this.Parameters.TryGetValue("args", out argsParam)
                           ? ((JArray)argsParam).ToObject<object[]>()
                           : new object[] { };

            var typeName = typeParam.ToString();
            var type = Type.GetType(typeName);

            if (type == null)
            {
                return this.JsonResponse(ResponseStatus.JavaScriptError, string.Format("type '{0}' not found", type));
            }

            var method = methodParam.ToString();
            var rv = type.GetRuntimeMethods().Single(x => x.Name == method).Invoke(null, args);

            return this.JsonResponse(ResponseStatus.Success, rv);
        }
    }
}
