namespace WindowsPhoneDriver.OuterDriver.EmulatorHelpers
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Windows.Forms;

    using Microsoft.Xde.Common;
    using Microsoft.Xde.Wmi;

    using WindowsPhoneDriver.Common;
    using WindowsPhoneDriver.Common.Exceptions;

    internal class EmulatorController
    {
        #region Fields

        private readonly IXdeVirtualMachine emulatorVm;

        private Point cursor;

        private Size virtualScreenSize;

        #endregion

        #region Constructors and Destructors

        public EmulatorController(string emulatorName)
        {
            this.emulatorVm = GetEmulatorVm(emulatorName);
            this.cursor = new Point(0, 0);
            this.PhoneOrientationToUse = PhoneOrientation.Landscape;

            if (this.emulatorVm == null)
            {
                throw new NullReferenceException(
                    string.Format("Could not get running XDE virtual machine {0}", emulatorName));
            }

            this.PhoneScreenSize = this.emulatorVm.GetCurrentResolution();
        }

        #endregion

        #region Enums

        [Flags]
        public enum PhoneOrientation
        {
            None = 0, 

            Portrait = 1, 

            Landscape = 2, 

            PortraitUp = 5, 

            PortraitDown = 9, 

            LandscapeLeft = 18, 

            LandscapeRight = 34, 
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Current phone orientation to be used when translating phone screen coordinates into virtual machine screen coordinates.
        /// Should be set before using PhoneScreenSize or any mouse moving methods
        /// </summary>
        public PhoneOrientation PhoneOrientationToUse { get; set; }

        public Size PhoneScreenSize
        {
            get
            {
                return (this.PhoneOrientationToUse & PhoneOrientation.Landscape) == PhoneOrientation.Landscape
                           ? new Size(this.virtualScreenSize.Height, this.virtualScreenSize.Width)
                           : this.virtualScreenSize;
            }

            private set
            {
                this.virtualScreenSize = value;
            }
        }

        #endregion

        #region Public Methods and Operators

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

        public bool PhonePointVisibleOnScreen(Point phonePoint)
        {
            var phoneScreen = new Rectangle(new Point(0, 0), this.PhoneScreenSize);
            return phoneScreen.Contains(phonePoint);
        }

        public void TypeKey(Keys key)
        {
            this.emulatorVm.TypeKey(key);
        }

        public string TakeScreenshot()
        {
            var size = this.emulatorVm.GetCurrentResolution();
            var screen = this.emulatorVm.GetScreenShot(0, 0, size.Width, size.Height);

            var base64 = ImageToBase64String(screen, ImageFormat.Png);
            screen.Dispose();
            return base64;
        }

        #endregion

        #region Methods

        private static IXdeVirtualMachine GetEmulatorVm(string emulatorName)
        {
            var factory = new XdeWmiFactory();
            var vm = factory.GetVirtualMachine(emulatorName + "." + Environment.UserName);
            if (vm.EnabledState != VirtualMachineEnabledState.Enabled)
            {
                throw new XdeVirtualMachineException("Emulator is not running. ");
            }

            return vm;
        }

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
            switch (this.PhoneOrientationToUse)
            {
                case PhoneOrientation.PortraitDown:
                    translatedPoint.X = this.virtualScreenSize.Width - location.X;
                    translatedPoint.Y = this.virtualScreenSize.Height - location.Y;
                    break;

                case PhoneOrientation.LandscapeLeft:
                    translatedPoint.X = this.virtualScreenSize.Width - location.Y;
                    translatedPoint.Y = location.X;
                    break;

                case PhoneOrientation.LandscapeRight:
                    translatedPoint.X = location.Y;
                    translatedPoint.Y = this.virtualScreenSize.Height - location.X;
                    break;
            }

            return translatedPoint;
        }

        #endregion
    }
}
