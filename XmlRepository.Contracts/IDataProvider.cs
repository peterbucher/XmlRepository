using System;

namespace XmlRepository.Contracts
{
    /// <summary>
    /// Provides access to a data sources.
    /// </summary>
    public interface IDataProvider
    {
        /// <summary>
        /// Loads the data source and returns its content.
        /// </summary>
        /// <typeparam name="TEntity">The entity type that shall be loaded.</typeparam>
        /// <returns>The root element.</returns>
        string Load<TEntity>();

        /// <summary>
        /// Saves the given content to the data source.
        /// </summary>
        /// <typeparam name="TEntity">The entity type that shall be loaded.</typeparam>
        /// <param name="content">The content.</param>
        void Save<TEntity>(string content);

        /// <summary>
        /// Raised when the underlying data source was changed.
        /// </summary>
        event EventHandler<DataSourceChangedEventArgs> DataSourceChanged;
    }
}