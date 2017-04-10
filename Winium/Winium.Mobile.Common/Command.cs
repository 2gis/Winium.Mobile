namespace Winium.Mobile.Common
{
    #region

    using System.Collections.Generic;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    #endregion

    public class Command
    {
        #region Constructors and Destructors

        public Command(string name, IDictionary<string, JToken> parameters)
            : this(name)
        {
            if (parameters != null)
            {
                this.Parameters = parameters;
            }
        }

        public Command(string name, string jsonParameters)
            : this(name, string.IsNullOrEmpty(jsonParameters) ? null : JObject.Parse(jsonParameters))
        {
        }

        public Command(string name)
        {
            this.Parameters = new JObject();
            this.Name = name;
            this.Context = DefaultContextNames.NativeAppContextName;
        }


        // ReSharper disable once UnusedMember.Global
        public Command()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the command name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets the parameters of the command
        /// </summary>
        [JsonProperty("parameters")]
        public IDictionary<string, JToken> Parameters { get; set; }

        /// <summary>
        /// Gets the SessionID of the command
        /// </summary>
        [JsonProperty("sessionId")]
        public string SessionId { get; set; }

        [JsonProperty("context")]
        public string Context { get; set; }

        [JsonProperty("atom")]
        public string Atom { get; set; }

        #endregion
    }
}
