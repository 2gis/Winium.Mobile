namespace Winium.Mobile.Common.CommandSettings
{
    #region

    using Newtonsoft.Json;

    #endregion

    public class CommandSettings
    {
        #region Constants

        public const string ElementAttributeSettingsParameter = "ElementAttributeSettings";

        #endregion

        #region Constructors and Destructors

        public CommandSettings()
        {
            this.ElementAttributeSettings = new ElementAttributeCommandSettings();
        }

        #endregion

        #region Public Properties

        [JsonProperty("elementAttributeSettings")]
        public ElementAttributeCommandSettings ElementAttributeSettings { get; set; }

        #endregion
    }
}
