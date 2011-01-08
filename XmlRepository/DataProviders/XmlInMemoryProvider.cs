using System;
using System.Collections.Generic;
using System.Xml.Linq;
using XmlRepository.Contracts;

namespace XmlRepository.DataProviders
{
    /// <summary>
    /// Provides access to an XML file data source.
    /// </summary>
    public class XmlInMemoryProvider : IXmlDataProvider
    {
        /// <summary>
        /// Contains the lock object.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// Contains the dictionary that holds the in-memory elements.
        /// </summary>
        private readonly IDictionary<Type, XElement> _elements = new Dictionary<Type, XElement>();

        /// <summary>
        /// Loads the data source and returns the root element.
        /// </summary>
        /// <typeparam name="TEntity">The entity type that shall be loaded.</typeparam>
        /// <returns>The root element.</returns>
        public XElement Load<TEntity>()
        {
            lock(this._lock)
            {
                if(!this._elements.ContainsKey(typeof(TEntity)))
                {
                    this._elements.Add(typeof(TEntity), new XElement(XmlRepository.RootElementName));
                }

                return this._elements[typeof (TEntity)];
            }
        }

        /// <summary>
        /// Saves the root element and all descending elements to the data source.
        /// </summary>
        /// <typeparam name="TEntity">The entity type that shall be saved.</typeparam>
        /// <param name="rootElement">The root element.</param>
        public void Save<TEntity>(XElement rootElement)
        {
            lock(this._lock)
            {
                if (!this._elements.ContainsKey(typeof(TEntity)))
                {
                    this._elements.Add(typeof(TEntity), rootElement);
                    return;
                }

                this._elements[typeof(TEntity)] = rootElement;
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