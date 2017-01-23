using System;
using System.Runtime.Serialization;

namespace CodeMovement.EbcdicCompare.Models.Exception
{
    /// <summary>
    ///  Exception thrown by EbcdicReader and EbcdicWriter when the format of a field is missing or unknown.
    /// </summary>
    [Serializable]
    public class UnexpectedFieldTypeException : EbcdicException
    {
        private readonly char _fieldType;

        /// <summary>
        /// Custom constructor with a message
        /// </summary>
        /// <param name="message"></param>
        public UnexpectedFieldTypeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Custom constructor with a message and an inner exception
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cause"></param>
        public UnexpectedFieldTypeException(string message, System.Exception cause)
            : base(message, cause)
        {
        }

        /// <summary>
        /// Custom constructor with a filedType
        /// </summary>
        /// <param name="fieldType"></param>
        public UnexpectedFieldTypeException(char fieldType)
            : base("Unexpected field encountered: " + fieldType)
        {
            _fieldType = fieldType;
        }

        /// <summary>
        /// Constructor for deserialization.
        /// </summary>
        /// <param name="info">the info holding the serialization data</param>
        /// <param name="context">the serialization context</param>
        public UnexpectedFieldTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _fieldType = info.GetChar("FieldType");
        }

        /// <summary>
        /// override on getMessage
        /// </summary>
        public override string Message
        {
            get { return "Unexpected field encountered: " + _fieldType; }
        }

        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("FieldType", _fieldType);
        }
    }
}
