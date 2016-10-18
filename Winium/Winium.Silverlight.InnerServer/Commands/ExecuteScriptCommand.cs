namespace Winium.Silverlight.InnerServer.Commands
{
    using System;
    using System.Linq;

    using Winium.Mobile.Common;

    using Newtonsoft.Json.Linq;

    internal class ExecuteScriptCommand : CommandBase
    {
        public override string DoImpl()
        {
            JToken typeParam;
            if (!this.Parameters.TryGetValue("type", out typeParam))
            {
                return "specify fully qualified type name in 'type' parameter";
            }

            JToken methodParam;
            if (!this.Parameters.TryGetValue("method", out methodParam))
            {
                return "specify fully qualified type name in 'method' parameter";
            }

            JToken argsParam;
            var args = this.Parameters.TryGetValue("args", out argsParam)
                                ? ((JArray)argsParam).ToObject<object[]>()
                                : new object[] { };

            var typeName = typeParam.ToString();
            var type = FindType(typeName);
            var method = methodParam.ToString();
            var rv = type.GetMethod(method).Invoke(null, args);

            return this.JsonResponse(ResponseStatus.Success, rv);
        }

        private static Type FindType(string fullName)
        {
            return
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic)
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName.Equals(fullName));
        }
    }
}