using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Coveralls
{
    [Serializable, ExcludeFromCodeCoverage]
    public class CoverallsException : Exception
    {
        public CoverallsException()
            : base()
        {
        }

        public CoverallsException(string message) : base(message)
        {
        }

        public CoverallsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CoverallsException(SerializationInfo info, StreamingContext context):
            base(info, context)
        { }
    }
}