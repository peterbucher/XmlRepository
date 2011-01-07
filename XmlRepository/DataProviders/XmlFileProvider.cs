using System;
using System.IO;
using System.Xml.Linq;
using XmlRepository.Contracts;

namespace XmlRepository.DataProviders
{
    /// <summary>
    /// Provides access to an XML file data source.
    /// </summary>
    public class XmlFileProvider : IXmlDataProvider
    {
        private readonly object _lockObject = new object();

        private readonly string _dataPath;

        private readonly FileSystemWatcher _fileSystemWatcher;

        private readonly string _dataFileExtension;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlFileProvider" /> type.
        /// </summary>
        /// <param name="dataPath">The data path.</param>
        public XmlFileProvider(string dataPath)
        {
            this._dataPath = dataPath;
            this._dataFileExtension = ".xml";

            this._fileSystemWatcher = new FileSystemWatcher(this._dataPath, String.Concat("*", this._dataFileExtension));
            this._fileSystemWatcher.Changed += (sender, eventArgs) => OnDataSourceChanged(eventArgs.Name);
            this._fileSystemWatcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Loads the data source and returns the root element.
        /// </summary>
        /// <returns>The root element.</returns>
        public XElement Load<TEntity>()
        {
            lock (this._lockObject)
            {
                var dataFile = this.GetDataFile<TEntity>();
                if (!File.Exists(dataFile))
                {
                    this.Save<TEntity>(new XElement(XmlRepository.RootElementName));
                }

                return XElement.Load(dataFile);
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
                rootElement.Save(this.GetDataFile<TEntity>());
            }
        }

        /// <summary>
        /// Raised when the underlying data source was changed.
        /// </summary>
        public event EventHandler<DataSourceChangedEventArgs> DataSourceChanged;

        /// <summary>
        /// Raises the DataSourceChanged event.
        /// </summary>
        protected virtual void OnDataSourceChanged(string dataFile)
        {
            var handler = this.DataSourceChanged;
            if(handler != null)
            {
                handler(this, new DataSourceChangedEventArgs(dataFile.Substring(0, dataFile.Length - this._dataFileExtension.Length)));
            }
        }

        private string GetDataFile<TEntity>()
        {
            lock (this._lockObject)
            {
                return Path.Combine(this._dataPath, String.Concat(typeof (TEntity).Name, this._dataFileExtension));
            }
        }
    }
}