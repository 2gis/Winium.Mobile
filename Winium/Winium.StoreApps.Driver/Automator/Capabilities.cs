namespace Winium.StoreApps.Driver.Automator
{
    #region

    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    #endregion

    internal class Capabilities
    {
        #region Constructors and Destructors

        internal Capabilities()
        {
            this.App = string.Empty;
            this.Files = new Dictionary<string, string>();
            this.DeviceName = string.Empty;
            this.LaunchDelay = 0;
            this.LaunchTimeout = 10000;
            this.DebugConnectToRunningApp = false;
            this.InnerPort = 9998;
            this.TakesScreenshot = true;
        }

        #endregion

        #region Public Properties

        [JsonProperty("platformName")]
        public static string PlatformName
        {
            get
            {
                return "WindowsPhone";
            }
        }

        [JsonProperty("app")]
        public string App { get; set; }

        [JsonProperty("debugConnectToRunningApp")]
        public bool DebugConnectToRunningApp { get; set; }

        [JsonProperty("deviceName")]
        public string DeviceName { get; set; }

        [JsonProperty("files")]
        public Dictionary<string, string> Files { get; set; }

        [JsonProperty("innerPort")]
        public int InnerPort { get; set; }

        [JsonProperty("launchDelay")]
        public int LaunchDelay { get; set; }

        [JsonProperty("launchTimeout")]
        public int LaunchTimeout { get; set; }

        [JsonProperty("takesScreenshot")]
        public bool TakesScreenshot { get; set; }

        #endregion

        #region Public Methods and Operators

        public static Capabilities CapabilitiesFromJsonString(string jsonString)
        {
            var capabilities = JsonConvert.DeserializeObject<Capabilities>(
                jsonString, 
                new JsonSerializerSettings
                    {
                        Error =
                            delegate(object sender, ErrorEventArgs args)
                                {
                                    args.ErrorContext.Handled = true;
                                }
                    });

            return capabilities;
        }

        public string CapabilitiesToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }

        #endregion
    }
}
