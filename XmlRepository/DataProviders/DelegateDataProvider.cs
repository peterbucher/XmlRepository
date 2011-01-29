using System;
using XmlRepository.Contracts;

namespace XmlRepository.DataProviders
{
    /// <summary>
    /// Provides access to a given load, and save delegate.
    /// </summary>
    public class DelegateDataProvider : IDataProvider
    {
        /// <summary>
        /// Contains the lock object.
        /// </summary>
        private readonly object _lockObject = new object();

        /// <summary>
        /// Contains the load delegate.
        /// </summary>
        private readonly Func<string> _loadDelegate;

        /// <summary>
        /// Contains the save delegate.
        /// </summary>
        private readonly Action<string> _saveDelegate;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryDataProvider" /> type.
        /// </summary>
        public DelegateDataProvider(Func<string> loadDelegate, Action<string> saveDelegate)
        {
            this._loadDelegate = loadDelegate;
            this._saveDelegate = saveDelegate;
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
                return this._loadDelegate();
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
                this._saveDelegate(content);
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