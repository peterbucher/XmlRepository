using System;
using System.Xml.Linq;

namespace XmlRepository.Contracts
{
    /// <summary>
    /// Provides access to XML data sources.
    /// </summary>
    public interface IXmlDataProvider
    {
        /// <summary>
        /// Loads the data source and returns the root element.
        /// </summary>
        /// <typeparam name="TEntity">The entity type that shall be loaded.</typeparam>
        /// <returns>The root element.</returns>
        XElement Load<TEntity>();

        /// <summary>
        /// Saves the root element and all descending elements to the data source.
        /// </summary>
        /// <typeparam name="TEntity">The entity type that shall be saved.</typeparam>
        /// <param name="rootElement">The root element.</param>
        void Save<TEntity>(XElement rootElement);

        /// <summary>
        /// Raised when the underlying data source was changed.
        /// </summary>
        event EventHandler<DataSourceChangedEventArgs> DataSourceChanged;
    }
}