namespace Winium.Mobile.Common.CommandSettings
{
    #region

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    #endregion

    public enum ElementAttributeAccessModifier
    {
        None = 0, 

        AutomationProperties = 1, 

        DependencyProperties = 3, 

        ClrProperties = 7, 
    }

    public class ElementAttributeCommandSettings
    {
        #region Constructors and Destructors

        public ElementAttributeCommandSettings()
        {
            this.AccessModifier = ElementAttributeAccessModifier.ClrProperties;
            this.EnumAsString = false;
        }

        #endregion

        #region Public Properties

        [JsonProperty("accessModifier")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ElementAttributeAccessModifier AccessModifier { get; set; }

        [JsonProperty("enumAsString")]
        public bool EnumAsString { get; set; }

        #endregion
    }
}
