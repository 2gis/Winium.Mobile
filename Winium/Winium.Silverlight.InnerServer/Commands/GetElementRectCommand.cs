namespace Winium.Silverlight.InnerServer.Commands
{
    using Mobile.Common;
    using System.Collections.Generic;

    internal class GetElementRectCommand : CommandBase
    {
        #region Public Properties

        public string ElementId { get; set; }

        #endregion

        #region Public Methods and Operators

        public override string DoImpl()
        {
            // FIXME Replace GetElementSizeCommand and LocationCommand with calls to this command as it is done in FireFox driver
            var element = this.Automator.WebElements.GetRegisteredElement(this.ElementId);

            var rect = element.GetRect(this.Automator.VisualRoot);
            var response = new Dictionary<string, int>
                                      {
                                          { "x", (int)rect.X },
                                          { "y", (int)rect.Y },
                                          { "width", (int)rect.Width }, 
                                          { "height", (int)rect.Height }
                                      };

            return this.JsonResponse(ResponseStatus.Success, response);
        }

        #endregion
    }
}
