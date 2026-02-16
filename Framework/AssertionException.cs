//
// AssertionException.cs - NinjaTrader Testing Framework
// Exception thrown when a test assertion fails
//

#region Using declarations

using System;
using System.Runtime.Serialization;

#endregion

namespace NinjaTrader.NinjaScript.AddOns.Testing
{
    /// <summary>
    /// Exception thrown when a test assertion fails.
    /// This exception is caught by TestRunner to mark a test as failed.
    /// </summary>
    [Serializable]
    public class AssertionException : Exception
    {
        /// <summary>
        /// Creates a new AssertionException with no message
        /// </summary>
        public AssertionException()
            : base("Assertion failed")
        {
        }

        /// <summary>
        /// Creates a new AssertionException with the specified message
        /// </summary>
        /// <param name="message">The assertion failure message</param>
        public AssertionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates a new AssertionException with the specified message and inner exception
        /// </summary>
        /// <param name="message">The assertion failure message</param>
        /// <param name="innerException">The inner exception that caused this assertion failure</param>
        public AssertionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Serialization constructor for exception serialization support
        /// </summary>
        /// <param name="info">Serialization info</param>
        /// <param name="context">Streaming context</param>
        protected AssertionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}