namespace Winium.Mobile.Driver.CommandHelpers
{
    #region

    using System.Collections.Generic;
    using System.Windows.Forms;

    #endregion

    internal class SendKeysHelper
    {
        #region Static Fields

        public static readonly Dictionary<string, Keys> SpecialKeys = new Dictionary<string, Keys>
                                                               {
                                                                   { "\ue007", Keys.Enter }, 
                                                                   { "\ue031", Keys.F1 }, 
                                                                   { "\ue032", Keys.F2 }, 
                                                                   { "\ue033", Keys.F3 }
                                                               };

        #endregion
    }
}
