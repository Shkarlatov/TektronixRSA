using System;

namespace Tektronix.TekRSA
{
    public class RSAException : Exception
    {
        #region default ctor
        public RSAException(string message) : base(message) { }

        public RSAException(string message, Exception innerException) : base(message, innerException) { }

        public RSAException() { }
        #endregion

        public RSAException(ReturnStatus status):base(status.ToString()){ }

    }
}
