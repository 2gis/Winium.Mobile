namespace Winium.StoreApps.Driver.Automator
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.Driver.EmulatorHelpers;

    #endregion

    internal class Automator
    {
        #region Static Fields

        private static readonly object LockObject = new object();

        private static volatile Automator instance;

        #endregion

        #region Fields

        private EmulatorController emulatorController;

        #endregion

        #region Constructors and Destructors

        public Automator(string session)
        {
            this.Session = session;
        }

        #endregion

        #region Public Properties

        public Capabilities ActualCapabilities { get; set; }

        public Requester CommandForwarder { get; set; }

        public IDeployer Deployer { get; set; }

        public EmulatorController EmulatorController
        {
            get
            {
                return this.emulatorController;
            }

            set
            {
                this.emulatorController = value;
            }
        }

        public string Session { get; private set; }

        #endregion

        #region Public Methods and Operators

        public static T GetValue<T>(IDictionary<string, JToken> parameters, string key) where T : class
        {
            JToken valueObject;
            if (!parameters.TryGetValue(key, out valueObject))
            {
                return null;
            }

            try
            {
                return valueObject.ToObject<T>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static Automator InstanceForSession(string sessionId)
        {
            if (instance == null)
            {
                lock (LockObject)
                {
                    if (instance == null)
                    {
                        if (sessionId == null)
                        {
                            sessionId = "AwesomeSession";
                        }

                        // TODO: Add actual support for sessions. Temporary return single Automator for any season
                        instance = new Automator(sessionId);
                    }
                }
            }

            return instance;
        }

        public Point? RequestElementLocation(JToken element)
        {
            var command = new Command(
                DriverCommand.GetElementLocationOnceScrolledIntoView, 
                new Dictionary<string, JToken> { { "ID", element } });

            var responseBody = this.CommandForwarder.ForwardCommand(command);

            var deserializeObject = JsonConvert.DeserializeObject<JsonResponse>(responseBody);

            if (deserializeObject.Status != ResponseStatus.Success)
            {
                return null;
            }

            var locationObject = deserializeObject.Value as JObject;
            if (locationObject == null)
            {
                return null;
            }

            var location = locationObject.ToObject<Dictionary<string, int>>();

            if (location == null)
            {
                return null;
            }

            var x = location["x"];
            var y = location["y"];
            return new Point(x, y);
        }

        /// <summary>
        /// This method should be called before any EmulatorController methods that move cursor or use screen size.
        /// Orientation value is used by EmulatorController to translate phone screen coordinates into virtual machine coordinates
        /// </summary>
        public void UpdatedOrientationForEmulatorController()
        {
            var command = new Command(DriverCommand.GetOrientation);
            var responseBody = this.CommandForwarder.ForwardCommand(command);
            var deserializeObject = JsonConvert.DeserializeObject<JsonResponse>(responseBody);
            if (deserializeObject.Status != ResponseStatus.Success)
            {
                return;
            }

            EmulatorController.PhoneOrientation orientation;
            if (Enum.TryParse(deserializeObject.Value.ToString(), true, out orientation))
            {
                this.emulatorController.PhoneOrientationToUse = orientation;
            }
        }

        #endregion
    }
}
