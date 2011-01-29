using System;
using System.IO;
using XmlRepository.Contracts;

namespace XmlRepository.DataProviders
{
    /// <summary>
    /// Provides access to a file data source.
    /// </summary>
    public class FileDataProvider : IDataProvider
    {
        private readonly object _lockObject = new object();

        private readonly string _dataPath;

        private readonly FileSystemWatcher _fileSystemWatcher;

        private readonly string _dataFileExtension;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDataProvider" /> type.
        /// </summary>
        /// <param name="dataPath">The data path.</param>
        /// <param name="dataFileExtension">The data file extension (including the leading dot).</param>
        public FileDataProvider(string dataPath, string dataFileExtension)
        {
            this._dataPath = dataPath;
            this._dataFileExtension =
                dataFileExtension.StartsWith(".") ? dataFileExtension : string.Concat(".", dataFileExtension);

            this._fileSystemWatcher = new FileSystemWatcher(this._dataPath, String.Concat("*", this._dataFileExtension));
            this._fileSystemWatcher.Changed += (sender, eventArgs) => OnDataSourceChanged(eventArgs.Name);
            this._fileSystemWatcher.EnableRaisingEvents = true;
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
                string dataFilePath = this.GetDataFile<TEntity>();

                if (File.Exists(dataFilePath))
                {
                    return File.ReadAllText(this.GetDataFile<TEntity>());
                }

                return XmlRepository.RootElementXml;
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
                File.WriteAllText(this.GetDataFile<TEntity>(), content);
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