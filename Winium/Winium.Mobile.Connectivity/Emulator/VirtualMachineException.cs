namespace Winium.Mobile.Connectivity.Emulator
{
    #region

    using System;
    using System.Runtime.Serialization;

    #endregion

    [Serializable]
    public class VirtualMachineException : Exception
    {
        #region Constructors and Destructors

        public VirtualMachineException(string str)
            : base(str)
        {
        }

        public VirtualMachineException(string str, Exception e)
            : base(str, e)
        {
        }

        protected VirtualMachineException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
