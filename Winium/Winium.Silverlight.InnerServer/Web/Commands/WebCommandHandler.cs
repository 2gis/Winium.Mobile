namespace Winium.Silverlight.InnerServer.Web.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Phone.Controls;
    using Newtonsoft.Json;

    using Winium.Mobile.Common;
    using InnerServer.Commands;
    using Mobile.Common.Exceptions;

    internal abstract class WebCommandHandler : CommandBase
    {
        private TimeSpan atomExecutionTimeout = TimeSpan.FromMilliseconds(-1);

        public CommandEnvironment Context { get; set; }

        public string Atom { get; set; }

        public abstract Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters);

        public override string DoImpl()
        {
            var parameters = this.Parameters.ToDictionary(x => x.Key, x => x.Value as object);
            if (this.Parameters.ContainsKey("ID"))
            {
                parameters["ID"] = new Dictionary<string, string> { { "ELEMENT", this.Parameters["ID"].ToObject<string>() } };
            }
            var rv = this.Execute(this.Context, parameters);

            return this.JsonResponse(rv.Status, rv.Value);
        }

        protected static string CreateArgumentString(object[] args)
        {
            StringBuilder builder = new StringBuilder();
            foreach (object arg in args)
            {
                if (builder.Length > 0)
                {
                    builder.Append(",");
                }

                string argAsString = JsonConvert.SerializeObject(arg);
                builder.Append(argAsString);
            }

            return builder.ToString();
        }

        /// <summary>
        /// Evaluates a JavaScript atom in the <see cref="CommandEnvironment"/>.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> in which to evaluate the JavaScript atom.</param>
        /// <param name="atom">The JavaScript atom to evaluate.</param>
        /// <param name="args">An array of arguments to the JavaScript atom.</param>
        /// <returns>The string result of the atom execution.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Catching general exception type is expressly permitted here to allow proper reporting via JSON-serialized result.")]
        protected string EvaluateAtom(CommandEnvironment environment, string atom, params object[] args)
        {
            WebBrowser browser = environment.Browser;
            string argumentString = CreateArgumentString(args);
            string script = "window.top.__wd_fn_result = (" + atom + ")(" + argumentString + ");";
            string result = string.Empty;
            ManualResetEvent synchronizer = new ManualResetEvent(false);
            try
            {
                browser.InvokeScript("eval", script);
                result = browser.InvokeScript("eval", "window.top.__wd_fn_result").ToString();
            }

            catch (Exception ex)
            {
                throw new AutomationException(string.Format(
                    CultureInfo.InvariantCulture,
                    "Unexpected exception {0}: {1}",
                    ex.GetType(),
                    ex.Message), ResponseStatus.UnknownError);
            }

            finally
            {
                synchronizer.Set();
            }

            synchronizer.WaitOne(this.atomExecutionTimeout);

            return result;
        }

        /// <summary>
        /// Sets the timeout for execution of an atom.
        /// </summary>
        /// <param name="timeout">The <see cref="TimeSpan"/> representing the timeout for the atom execution.</param>
        protected void SetAtomExecutionTimeout(TimeSpan timeout)
        {
            this.atomExecutionTimeout = timeout;
        }
    }
}