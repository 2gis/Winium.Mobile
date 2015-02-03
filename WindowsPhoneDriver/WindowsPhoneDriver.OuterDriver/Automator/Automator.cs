namespace WindowsPhoneDriver.OuterDriver.Automator
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using OpenQA.Selenium.Remote;

    using WindowsPhoneDriver.Common;
    using WindowsPhoneDriver.OuterDriver.EmulatorHelpers;

    using DriverCommand = WindowsPhoneDriver.Common.DriverCommand;

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

        public static T GetValue<T>(IReadOnlyDictionary<string, object> parameters, string key) where T : class
        {
            object valueObject;
            parameters.TryGetValue(key, out valueObject);

            return valueObject as T;
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

        public Point? RequestElementLocation(string element)
        {
            var command = new Command(null, DriverCommand.GetElementLocationOnceScrolledIntoView, new Dictionary<string, object> { { "ID", element } });

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
            var command = new Command(null, DriverCommand.GetOrientation, null);
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
