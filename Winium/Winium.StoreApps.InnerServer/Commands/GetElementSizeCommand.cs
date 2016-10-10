﻿namespace Winium.StoreApps.InnerServer.Commands
 {
     #region

     using System.Collections.Generic;

     using Winium.Mobile.Common;

     #endregion

     internal class GetElementSizeCommand : CommandBase
     {
         #region Public Properties

         public string ElementId { private get; set; }

         #endregion

         #region Public Methods and Operators

         protected override string DoImpl()
         {
             var element = this.Automator.ElementsRegistry.GetRegisteredElement(this.ElementId);
             var rect = element.GetRect();
             var response = new Dictionary<string, int>
                                      {
                                          { "width", (int)rect.Width }, 
                                          { "height", (int)rect.Height }
                                      };

             return this.JsonResponse(ResponseStatus.Success, response);
         }

         #endregion
     }
 }
