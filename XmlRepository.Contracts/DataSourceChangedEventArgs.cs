using System;

namespace XmlRepository.Contracts
{
    /// <summary>
    /// Represents the event argumemts when an XML data source was changed.
    /// </summary>
    public class DataSourceChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the empty event arguments.
        /// </summary>
        /// <value>The empty event arguments.</value>
        public static new DataSourceChangedEventArgs Empty
        {
            get
            {
                return new DataSourceChangedEventArgs(null);
            }
        }

        /// <summary>
        /// Gets the name of the entity type changed.
        /// </summary>
        /// <value>The name of the entity type.</value>
        public string EntityType
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceChangedEventArgs" /> type.
        /// </summary>
        /// <param name="entityType">The name of the entity type.</param>
        public DataSourceChangedEventArgs(string entityType)
        {
            this.EntityType = entityType;
        }
    }
}