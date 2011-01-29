using System;

namespace XmlRepository
{
    /// <summary>
    /// Thrown when an xml element was not found.
    /// </summary>
    public class ElementNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementNotFoundException" /> type.
        /// </summary>
        public ElementNotFoundException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementNotFoundException" /> type.
        /// </summary>
        /// <param name="message">The message.</param>
        public ElementNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementNotFoundException" /> type.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ElementNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}