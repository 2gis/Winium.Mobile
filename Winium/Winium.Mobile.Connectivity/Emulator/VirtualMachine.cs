namespace Winium.Mobile.Connectivity.Emulator
{
    #region

    using System;
    using System.Drawing;
    using System.Windows.Forms;

    using Microsoft.Xde.Common;

    #endregion

    public class VirtualMachine : IDisposable
    {
        #region Fields

        private IXdeVirtualMachine vm;

        #endregion

        #region Constructors and Destructors

        public VirtualMachine(IXdeVirtualMachine virtualMachine)
        {
            this.vm = virtualMachine;
        }

        #endregion

        #region Public Properties

        public IXdeVirtualMachineNicInformation InternalNic
        {
            get
            {
                return this.vm.InternalNic;
            }
        }

        public string Name
        {
            get
            {
                return this.vm.Name;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            this.vm = null;
        }

        public Size GetCurrentResolution()
        {
            return this.vm.GetCurrentResolution();
        }

        public Image GetScreenShot(int startX, int startY, int width, int height)
        {
            return this.vm.GetScreenShot(startX, startY, width, height);
        }

        public void PressKey(Keys key)
        {
            this.vm.PressKey(key);
        }

        public void ReleaseKey(Keys key)
        {
            this.vm.ReleaseKey(key);
        }

        public void SendMouseEvent(MouseEventArgs args)
        {
            this.vm.SendMouseEvent(args);
        }

        public void TypeKey(Keys key)
        {
            this.vm.TypeKey(key);
        }

        #endregion
    }
}
