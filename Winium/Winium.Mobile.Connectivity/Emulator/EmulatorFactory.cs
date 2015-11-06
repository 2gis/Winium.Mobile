namespace Winium.Mobile.Connectivity.Emulator
{
    #region

    using System;
    using System.Collections.Generic;

    using Microsoft.Xde.Common;
    using Microsoft.Xde.Wmi;

    using Winium.StoreApps.Logging;

    #endregion

    public class EmulatorFactory : IDisposable
    {
        #region Static Fields

        private static readonly Lazy<EmulatorFactory> LazyInstance =
            new Lazy<EmulatorFactory>(() => new EmulatorFactory());

        #endregion

        #region Fields

        private readonly Dictionary<string, IXdeVirtualMachine> cache;

        private bool isDisposed;

        #endregion

        #region Constructors and Destructors

        private EmulatorFactory()
        {
            this.cache = new Dictionary<string, IXdeVirtualMachine>();
        }

        #endregion

        #region Public Properties

        public static EmulatorFactory Instance
        {
            get
            {
                return LazyInstance.Value;
            }
        }

        #endregion

        #region Public Methods and Operators

        public void Dispose()
        {
            if (!this.isDisposed)
            {
                foreach (var chachedVm in this.cache)
                {
                    chachedVm.Value.Dispose();
                }

                this.cache.Clear();
                this.isDisposed = true;
            }

            GC.SuppressFinalize(this);
        }

        public VirtualMachine GetVm(string emulatorName)
        {
            var wmiFactory = new XdeWmiFactory();
            var fullName = emulatorName + "." + Environment.UserName.ToLowerInvariant();
            IXdeVirtualMachine vm;
            var cacheHit = this.cache.TryGetValue(fullName, out vm);
            if (!cacheHit || vm.EnabledState == VirtualMachineEnabledState.Unknown)
            {
                Logger.Info("Emulator '{0}' not found in cache. Requesting WMI.", fullName);
                vm = wmiFactory.GetVirtualMachine(fullName);
                this.cache[fullName] = vm;
            }
            else
            {
                Logger.Info("Emulator '{0}' found in cache. Using cached VM.", fullName);
            }

            if (vm == null)
            {
                throw new XdeVirtualMachineException(string.Format("Emulator '{0}' not found.", fullName));
            }

            if (vm.EnabledState != VirtualMachineEnabledState.Enabled)
            {
                throw new XdeVirtualMachineException(string.Format("Emulator '{0}' is not running.", fullName));
            }

            return new VirtualMachine(vm);
        }

        #endregion
    }
}
