using System;
using XmlRepository.Contracts;

namespace XmlRepository.DataProviders
{
    /// <summary>
    /// Provides access to an in memory data source.
    /// </summary>
    public class InMemoryDataProvider : IDataProvider
    {
        /// <summary>
        /// Contains the lock object.
        /// </summary>
        private readonly object _lockObject = new object();

        /// <summary>
        /// Contains the data content.
        /// </summary>
        private string _dataContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryDataProvider" /> type.
        /// </summary>
        public InMemoryDataProvider()
        {
            this._dataContent = XmlRepository.RootElementXml;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryDataProvider" /> type with initial data passed in.
        /// <param name="initialDataContent">Initial data for the data provider.</param>
        /// </summary>
        public InMemoryDataProvider(string initialDataContent)
        {
            this._dataContent = initialDataContent;
        }

        /// <summary>
        /// Loads the data source and returns its content.
        /// </summary>
        /// <typeparam name="TEntity">The entity type that shall be loaded.</typeparam>
        /// <returns>The root element.</returns>
        public string Load<TEntity>()
        {
            lock (this._lockObject)
            {
                return this._dataContent;
            }
        }

        /// <summary>
        /// Saves the given content to the data source.
        /// </summary>
        /// <typeparam name="TEntity">The entity type that shall be loaded.</typeparam>
        /// <param name="content">The content.</param>
        public void Save<TEntity>(string content)
        {
            lock (this._lockObject)
            {
                this._dataContent = content;
            }
        }

        /// <summary>
        /// Raised when the underlying data source was changed.
        /// </summary>
        public event EventHandler<DataSourceChangedEventArgs> DataSourceChanged;

        /// <summary>
        /// Raises the DataSourceChanged event.
        /// </summary>
        protected virtual void OnDataSourceChanged()
        {
            var handler = this.DataSourceChanged;
            if (handler != null)
            {
                handler(this, DataSourceChangedEventArgs.Empty);
            }
        }
    }
}