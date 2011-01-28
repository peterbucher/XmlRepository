using System;

namespace XmlRepository
{
    /// <summary>
    /// Thrown when an datafile was not found.
    /// </summary>
    public class DataFileNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataFileNotFoundException" /> type.
        /// </summary>
        public DataFileNotFoundException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFileNotFoundException" /> type.
        /// </summary>
        /// <param name="message">The message.</param>
        public DataFileNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataFileNotFoundException" /> type.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public DataFileNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}