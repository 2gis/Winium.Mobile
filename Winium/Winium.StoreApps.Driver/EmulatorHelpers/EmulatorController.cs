namespace Winium.StoreApps.Driver.EmulatorHelpers
{
    #region

    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    using Microsoft.Xde.Common;
    using Microsoft.Xde.Interface;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.Common.Exceptions;

    #endregion

    internal class EmulatorController
    {
        #region Fields

        private readonly IXdeAutomation client;

        private readonly VirtualMachine emulatorVm;

        private Point cursor;

        private Size virtualScreenSize;

        #endregion

        #region Constructors and Destructors

        public EmulatorController(string emulatorName)
        {
            this.emulatorVm = EmulatorFactory.Instance.GetVm(emulatorName);

            if (this.emulatorVm == null)
            {
                throw new NullReferenceException(
                    string.Format("Could not get running XDE virtual machine {0}", emulatorName));
            }

            this.client = AutomationClient.CreateAutomationClient(this.emulatorVm.Name);

            this.cursor = new Point(0, 0);
            this.PhoneScreenSize = this.emulatorVm.GetCurrentResolution();
        }

        #endregion

        #region Enums

        public enum PhoneOrientation
        {
            Portrait = 1, 

            Landscape = 2, 
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Current phone orientation to be used when translating phone screen coordinates into virtual machine screen coordinates.
        /// Should be set before using PhoneScreenSize or any mouse moving methods
        /// </summary>
        public PhoneOrientation Orientation
        {
            get
            {
                return this.client.DisplayOrientation == DisplayOrientation.Portrait
                           ? PhoneOrientation.Portrait
                           : PhoneOrientation.Landscape;
            }

            set
            {
                this.client.DisplayOrientation = (value == PhoneOrientation.Portrait)
                                                     ? DisplayOrientation.Portrait
                                                     : DisplayOrientation.LandscapeLeft;
                Thread.Sleep(1500);
            }
        }

        public Size PhoneScreenSize
        {
            get
            {
                var landscape = this.Orientation.HasFlag(PhoneOrientation.Landscape);
                return landscape
                           ? new Size(this.virtualScreenSize.Height, this.virtualScreenSize.Width)
                           : this.virtualScreenSize;
            }

            private set
            {
                this.virtualScreenSize = value;
            }
        }

        public string VmName
        {
            get
            {
                return this.emulatorVm.Name;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Disconnect()
        {
            this.client.Dispose();
            this.emulatorVm.Dispose();
        }

        public string GetIpAddress()
        {
            var nic = this.emulatorVm.InternalNic;
            if (nic == null)
            {
                throw new NullReferenceException("Windows Phone Emulator Internal Switch not found");
            }

            if (nic.SwitchInformation.HostIpAddress == null)
            {
                throw new NullReferenceException("Unable to determine Windows Phone Emulator IP address.");
            }

            return nic.SwitchInformation.HostIpAddress;
        }

        public void LeftButtonClick(Point? point = null)
        {
            if (point.HasValue)
            {
                this.MoveCursorTo(point.Value);
            }

            this.LeftButtonDown();
            this.LeftButtonUp();
        }

        public void LeftButtonDown()
        {
            var hold = new MouseEventArgs(MouseButtons.Left, 0, this.cursor.X, this.cursor.Y, 0);
            this.emulatorVm.SendMouseEvent(hold);
        }

        public void LeftButtonUp()
        {
            var release = new MouseEventArgs(MouseButtons.None, 0, this.cursor.X, this.cursor.Y, 0);
            this.emulatorVm.SendMouseEvent(release);
        }

        public void MoveCursorTo(Point phonePoint)
        {
            var xdePoint = this.TranslatePhoneToVirtualmachinePoint(phonePoint);
            this.cursor = xdePoint;
        }

        public void PerformGesture(IGesture gesture)
        {
            // TODO Works only for default portrait orientation, need to take orientation into account
            var array = gesture.GetScreenPoints().ToArray();

            foreach (var point in array.Take(array.Length - 1))
            {
                this.MoveCursorTo(point);
                this.LeftButtonDown();
                Thread.Sleep(gesture.PeriodBetweenPoints);
            }

            this.MoveCursorTo(array.Last());
            this.LeftButtonUp();
        }

        public string TakeScreenshot()
        {
            var size = this.virtualScreenSize;
            var screen = this.emulatorVm.GetScreenShot(0, 0, size.Width, size.Height);

            var base64 = ImageToBase64String(screen, ImageFormat.Png);
            screen.Dispose();
            return base64;
        }

        public void TypeKey(Keys key)
        {
            this.emulatorVm.TypeKey(key);
        }

        #endregion

        #region Methods

        private static string ImageToBase64String(Image image, ImageFormat imageFormat)
        {
            byte[] byteBuffer;
            using (var memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, imageFormat);

                memoryStream.Position = 0;
                byteBuffer = memoryStream.ToArray();
            }

            if (byteBuffer == null)
            {
                throw new AutomationException("Could not take screenshot.", ResponseStatus.UnknownError);
            }

            return Convert.ToBase64String(byteBuffer);
        }

        private Point TranslatePhoneToVirtualmachinePoint(Point location)
        {
            var translatedPoint = location;
            switch (this.client.DisplayOrientation)
            {
                case DisplayOrientation.LandscapeLeft:
                    translatedPoint.X = this.virtualScreenSize.Width - location.Y;
                    translatedPoint.Y = location.X;
                    break;

                case DisplayOrientation.LandscapeRight:
                    translatedPoint.X = location.Y;
                    translatedPoint.Y = this.virtualScreenSize.Height - location.X;
                    break;
            }

            return translatedPoint;
        }

        #endregion
    }
}
