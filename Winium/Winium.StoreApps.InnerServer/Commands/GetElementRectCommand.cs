﻿namespace Winium.StoreApps.InnerServer.Commands
 {
     #region

     using System.Collections.Generic;

     using Winium.Mobile.Common;

     #endregion

     internal class GetElementRectCommand : CommandBase
     {
         #region Public Properties

         public string ElementId { private get; set; }

         #endregion

         #region Public Methods and Operators

         protected override string DoImpl()
         {
             // FIXME Replace GetElementSizeCommand and LocationCommand with calls to this command as it is done in FireFox driver
             var element = this.Automator.ElementsRegistry.GetRegisteredElement(this.ElementId);
             var rect = element.GetRect();
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
