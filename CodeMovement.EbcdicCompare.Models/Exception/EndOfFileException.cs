using System;
using System.Diagnostics.CodeAnalysis;

namespace CodeMovement.EbcdicCompare.Models.Exception
{
    /// <summary>
    /// Exception thrown by EbcdicReader to signal that the end of file has been prematurely encountered.
    /// </summary>
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class EndOfFileException : EbcdicException
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public EndOfFileException()
            : base(String.Empty)
        {
        }

        /// <summary>
        /// Custom constructor using a message
        /// </summary>
        /// <param name="message"></param>
        public EndOfFileException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Custom constructor using a message and an inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cause"></param>
        public EndOfFileException(string message, System.Exception cause)
            : base(message, cause)
        {
        }
    }
}
