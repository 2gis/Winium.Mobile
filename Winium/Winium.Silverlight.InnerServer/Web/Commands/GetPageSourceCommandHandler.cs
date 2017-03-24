namespace Winium.Silverlight.InnerServer.Web.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    class GetPageSourceCommandHandler : WebCommandHandler
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="environment">The <see cref="CommandEnvironment"/> to use in executing the command.</param>
        /// <param name="parameters">The <see cref="Dictionary{string, object}"/> containing the command parameters.</param>
        /// <returns>The JSON serialized string representing the command response.</returns>
        public override Response Execute(CommandEnvironment environment, Dictionary<string, object> parameters)
        {
            string pageSource = string.Empty;
            var executeScriptAtom = this.Atom;
            if (string.IsNullOrEmpty(environment.FocusedFrame))
            {
                //ManualResetEvent synchronizer = new ManualResetEvent(false);
                //Deployment.Current.Dispatcher.BeginInvoke(() =>
                //{
                //    pageSource = environment.Browser.SaveToString();
                //    synchronizer.Set();
                //});

                //synchronizer.WaitOne();

                pageSource = environment.Browser.SaveToString();
            }
            else
            {
                string script = "return document.documentElement.outerHTML;";
                string result = this.EvaluateAtom(environment, executeScriptAtom, script, new object[] { }, environment.CreateFrameObject());
                return Response.FromJson(result);
            }

            return Response.CreateSuccessResponse(pageSource);
        }
    }
}
