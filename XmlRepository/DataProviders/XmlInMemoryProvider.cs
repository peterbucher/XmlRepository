using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly object _lockObject = new object();

        /// <summary>
        /// Contains the dictionary that holds the in-memory elements.
        /// </summary>
        private readonly IDictionary<Type, XElement> _elements;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlInMemoryProvider" /> type.
        /// </summary>
        public XmlInMemoryProvider()
        {
            this._elements = new Dictionary<Type, XElement>();
        }

        /// <summary>
        /// Loads the data source and returns the root element.
        /// </summary>
        /// <typeparam name="TEntity">The entity type that shall be loaded.</typeparam>
        /// <returns>The root element.</returns>
        public XElement Load<TEntity>()
        {
            lock (this._lockObject)
            {
                if (!this._elements.ContainsKey(typeof(TEntity)))
                {
                    this._elements.Add(typeof(TEntity), new XElement(XmlRepository.RootElementName));
                }

                return this.Clone(this._elements[typeof(TEntity)]);
            }
        }

        /// <summary>
        /// Saves the root element and all descending elements to the data source.
        /// </summary>
        /// <typeparam name="TEntity">The entity type that shall be saved.</typeparam>
        /// <param name="rootElement">The root element.</param>
        public void Save<TEntity>(XElement rootElement)
        {
            lock (this._lockObject)
            {
                if (!this._elements.ContainsKey(typeof(TEntity)))
                {
                    this._elements.Add(typeof(TEntity), rootElement);
                    return;
                }

                this._elements[typeof(TEntity)] = this.Clone(rootElement);
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

        /// <summary>
        /// Clones the given XElement.
        /// </summary>
        /// <param name="source">The source XElement.</param>
        /// <returns>The cloned XElement.</returns>
        private XElement Clone(XElement source)
        {
            lock (this._lockObject)
            {
                return XElement.Parse(source.ToString());
            }
        }
    }
}