namespace Winium.Silverlight.InnerServer.Commands
{
    #region

    using Newtonsoft.Json.Linq;

    using Mobile.Common;
    using Mobile.Common.Exceptions;

    #endregion
    internal class SetContextCommand : CommandBase
    {
        public override string DoImpl()
        {
            JToken value;
            if (!this.Parameters.TryGetValue("name", out value))
            {
                throw new AutomationException("Bad argument: name", ResponseStatus.UnknownCommand);
            }

            var name = value.ToObject<string>();

            if (string.IsNullOrWhiteSpace(name))
            {
                name = DefaultContextNames.NativeAppContextName;
            }

            if (!name.Equals(DefaultContextNames.NativeAppContextName))
            {
                // check if context is still valid
                this.Automator.ContextsRegistry.GetContext(name);
            }

            this.Automator.CurrentContext = name;

            return this.JsonResponse(ResponseStatus.Success, this.Automator.CurrentContext);
        }
    }
}