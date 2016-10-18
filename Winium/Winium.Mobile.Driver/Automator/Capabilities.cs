namespace Winium.Mobile.Driver.Automator
{
    #region

    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Winium.Mobile.Connectivity;
    using Winium.Mobile.Common.CommandSettings;
    using Winium.Mobile.Common.Exceptions;

    #endregion

    public class Capabilities
    {
        #region Static Fields

        private static string boundDeviceName;

        #endregion

        #region Constructors and Destructors

        internal Capabilities()
        {
            this.AutoLaunch = true;
            this.App = string.Empty;
            this.Files = new Dictionary<string, string>();
            this.DeviceName = string.Empty;
            this.LaunchDelay = 0;
            this.LaunchTimeout = 10000;
            this.DebugConnectToRunningApp = false;
            this.TakesScreenshot = true;
            this.Dependencies = new List<string>();
            this.PingTimeout = DefaultPingTimeout;
            this.CommandSettings = new CommandSettings();
            this.IsJavascriptEnabled = true;
        }

        #endregion

        #region Public Properties

        public static string BoundDeviceName
        {
            get
            {
                return boundDeviceName;
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    return;
                }

                if (!Devices.Instance.IsValidDeviceName(value))
                {
                    const string MsgFormat =
                        "Could not find a device by strict name. You requested '{0}', but the available devices were:\n{1}";
                    throw new AutomationException(string.Format(MsgFormat, value, Devices.Instance));
                }

                boundDeviceName = value;
            }
        }

        public static int DefaultPingTimeout { get; set; }

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

        [JsonProperty("autoLaunch")]
        public bool AutoLaunch { get; set; }

        [JsonProperty("debugConnectToRunningApp")]
        public bool DebugConnectToRunningApp { get; set; }

        [JsonProperty("dependencies")]
        public List<string> Dependencies { get; set; }

        [JsonProperty("deviceName")]
        public string DeviceName { get; set; }

        [JsonProperty("files")]
        public Dictionary<string, string> Files { get; set; }

        [JsonProperty("launchDelay")]
        public int LaunchDelay { get; set; }

        [JsonProperty("launchTimeout")]
        public int LaunchTimeout { get; set; }

        [JsonProperty("pingTimeout")]
        public int PingTimeout { get; set; }

        [JsonProperty("takesScreenshot")]
        public bool TakesScreenshot { get; set; }

        [JsonProperty("commandSettings")]
        public CommandSettings CommandSettings { get; set; }

        [JsonProperty("javascriptEnabled")]
        public bool IsJavascriptEnabled { get; set; }

        #endregion

        #region Public Methods and Operators

        public static Capabilities CapabilitiesFromJsonString(string jsonString)
        {
            var capabilities = JsonConvert.DeserializeObject<Capabilities>(
                jsonString,
                new JsonSerializerSettings
                {
                    Error =
                            delegate (object sender, ErrorEventArgs args)
                                {
                                    args.ErrorContext.Handled = true;
                                }
                });
            return capabilities;
        }

        #endregion
    }
}
