using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using XmlRepository.Contracts;
using XmlRepository.DataProviders;

namespace XmlRepository
{
    /// <summary>
    /// Represents the configuration class for xml repositories.
    /// </summary>
    public static class XmlRepository
    {
        /// <summary>
        /// Gets the name of the root element.
        /// </summary>
        internal static readonly string RootElementName = "root";

        /// <summary>
        /// Gets or sets the name of the entity's property that is used as default key within
        /// queries.
        /// </summary>
        /// <value>
        /// The name of the entity's property that is used as default key within queries.
        /// </value>
        public static string DefaultQueryProperty
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data provider that is being used to access XML data sources. If not
        /// set, the XmlFileProvider is being used with the current folder as data folder.
        /// </summary>
        /// <value>The data provider.</value>
        public static IXmlDataProvider DataProvider
        {
            get;
            set; 
        }

        /// <summary>
        /// Initializes the <see cref="XmlRepository" /> type.
        /// </summary>
        static XmlRepository()
        {
            DataProvider = new XmlFileProvider(Environment.CurrentDirectory);
            DefaultQueryProperty = "Id";
        }
    }

    /// <summary>
    /// Represents an xml repository.
    /// </summary>
    public class XmlRepository<TEntity> : IXmlRepository<TEntity>
    {
        /// <summary>
        /// Contains the lock object for instance locking.
        /// </summary>
        private readonly object _lockObject = new object();

        /// <summary>
        /// Contains the lock object for type locking.
        /// </summary>
        private static readonly object LockObject = new object();

        /// <summary>
        /// Contains the conversion method which converts the entity to an XElement.
        /// </summary>
        private readonly Func<TEntity, XElement> _toXElementFunction;

        /// <summary>
        /// Contains the conversion method which converts an XElement to the entity.
        /// </summary>
        private readonly Func<XElement, TEntity> _toObjectFunction;

        /// <summary>
        /// Contains the name of the entity's property that is used as default key within queries.
        /// </summary>
        private readonly string _defaultQueryProperty;

        /// <summary>
        /// Contains the property info of the entity's property that is used as default key within
        /// queries.
        /// </summary>
        private readonly PropertyInfo _defaultQueryPropertyInfo;

        /// <summary>
        /// Contains a reference to the root element of the data source.
        /// </summary>
        private XElement _rootElement;

        /// <summary>
        /// Contains <c>true</c> if the in-memory representation of the data source contains
        /// changes that have not yet been submitted to the data source; <c>false</c> otherwise.
        /// </summary>
        private bool _isDirty;

        /// <summary>
        /// Contains a list of all repositories created so far, for reuse.
        /// </summary>
        private static readonly IDictionary<Type, IXmlRepository> Repositories;

        /// <summary>
        /// Gets the repository for the specified type. If the repository has not been created yet,
        /// a new one is created; otherwise, the existing one is returned.
        /// </summary>
        /// <value>The repository for the requested type.</value>
        public static IXmlRepository<TEntity> Instance
        {
            get
            {
                lock (LockObject)
                {
                    // If the repository does not yet exist, create it.
                    if (!Repositories.ContainsKey(typeof(TEntity)))
                    {
                        Repositories.Add(typeof(TEntity), new XmlRepository<TEntity>());
                    }

                    // Return the requested repository to the caller.
                    return Repositories[typeof(TEntity)] as IXmlRepository<TEntity>;
                }
            }
        }

        /// <summary>
        /// Initializes the <see cref="XmlRepository" /> type.
        /// </summary>
        static XmlRepository()
        {
            Repositories = new Dictionary<Type, IXmlRepository>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRepository" /> type.
        /// </summary>
        private XmlRepository()
        {
            lock (this._lockObject)
            {
                // Set up the default query property.
                this._defaultQueryProperty = XmlRepository.DefaultQueryProperty;
                this._defaultQueryPropertyInfo =
                    typeof(TEntity).GetProperty(this._defaultQueryProperty);

                // Set up the conversion methods.
                this._toXElementFunction = XmlMappingBuilder<TEntity>.ToXElement();
                this._toObjectFunction = XmlMappingBuilder<TEntity>.ToObject();

                // If the data source has been changed, reload the in-memory representation of the
                // data source.
                XmlRepository.DataProvider.DataSourceChanged +=
                    (sender, eventArgs) =>
                        {
                            if (eventArgs.EntityType == typeof(TEntity).Name)
                            {
                                this.DiscardChanges();
                            }
                        };

                // Discard the in-memory representation of the data source, as it has not yet been
                // loaded. This results in an initial load.
                this.DiscardChanges();
            }
        }

        /// <summary>
        /// Gets all elements with the specified key value pair.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A list of elements. If no elements were found, an empty list is returned.
        /// </returns>
        private IEnumerable<XElement> GetElementsByKeyValuePair(string key, object value)
        {
            lock (this._lockObject)
            {
                return
                    from e in this._rootElement.Elements()
                    where (e.Element(key).Value.Equals(value.ToString()))
                    select e;
            }
        }

        /// <summary>
        /// Loads the entity with the specified value for the default query property.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The entity, if an entity was found. Otherwise, an exception is thrown.
        /// </returns>
        public TEntity Load(object value)
        {
            lock (this._lockObject)
            {
                return this.LoadBy(this._defaultQueryProperty, value);
            }
        }

        /// <summary>
        /// Loads the entity with the specified value for the given query property.
        /// </summary>
        /// <param name="queryProperty">The name of the query property.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The entity, if an entity was found. Otherwise, an exception is thrown.
        /// </returns>
        public TEntity LoadBy(string queryProperty, object value)
        {
            lock (this._lockObject)
            {
                return this.LoadAllBy(queryProperty, value).Single();
            }
        }

        /// <summary>
        /// Loads the entity that matches the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// The entity, if an entity was found. Otherwise, an exception is thrown.
        /// </returns>
        public TEntity LoadBy(Func<TEntity, bool> predicate)
        {
            lock (this._lockObject)
            {
                return LoadAll().Where(predicate).Single();
            }
        }

        /// <summary>
        /// Loads all entities.
        /// </summary>
        /// <returns>
        /// A list of all entities. If no entities were found, an empty list is returned.
        /// </returns>
        public IEnumerable<TEntity> LoadAll()
        {
            lock (this._lockObject)
            {
                return
                    (from e in this._rootElement.Elements()
                     select this._toObjectFunction(e)).ToList();
            }
        }

        /// <summary>
        /// Loads all entities with the specified value for the given query property.
        /// </summary>
        /// <param name="queryProperty">The name of the query property.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A list of all entities. If no entities were found, an empty list is returned.
        /// </returns>
        public IEnumerable<TEntity> LoadAllBy(string queryProperty, object value)
        {
            lock (this._lockObject)
            {
                return
                    (from e in this.GetElementsByKeyValuePair(this._defaultQueryProperty, value)
                     select this._toObjectFunction(e)).ToList();
            }
        }

        /// <summary>
        /// Loads all entities that match the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// A list of all entities. If no entities were found, an empty list is returned.
        /// </returns>
        public IEnumerable<TEntity> LoadAllBy(Func<TEntity, bool> predicate)
        {
            lock (this._lockObject)
            {
                return LoadAll().Where(predicate).ToList();
            }
        }

        /// <summary>
        /// Saves or updates the given entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public void SaveOnSubmit(TEntity entity)
        {
            lock (this._lockObject)
            {
                // Check whether the entity already exists. If so, remove it (and simulate an
                // update this way).
                var defaultQueryPropertyValue = _defaultQueryPropertyInfo.GetValue(entity, null);
                var element =
                    (this.GetElementsByKeyValuePair(
                        this._defaultQueryProperty, defaultQueryPropertyValue))
                    .SingleOrDefault();
                if (element != null)
                {
                    element.Remove();
                }

                // Add the entity.
                this._rootElement.Add(this._toXElementFunction(entity));
                this._isDirty = true;
            }
        }

        /// <summary>
        /// Deletes the entity with the given identity value.
        /// </summary>
        /// <typeparam name="TIdentity"></typeparam>
        /// <param name="value">The identity value.</param>
        public void DeleteOnSubmit<TIdentity>(TIdentity value)
        {
            lock (this._lockObject)
            {
                this.GetElementsByKeyValuePair(
                    this._defaultQueryProperty, value).Single().Remove();
                this._isDirty = true;
            }
        }

        /// <summary>
        /// Deletes all entities.
        /// </summary>
        public void DeleteAllOnSubmit()
        {
            lock (this._lockObject)
            {
                this._rootElement.RemoveAll();
                this._isDirty = true;
            }
        }

        /// <summary>
        /// Submits all changes to the repository so that the changes are persistent.
        /// </summary>
        public void SubmitChanges()
        {
            lock (this._lockObject)
            {
                if (!this._isDirty)
                {
                    return;
                }

                XmlRepository.DataProvider.Save<TEntity>(this._rootElement);
                this._isDirty = false;
            }
        }

        /// <summary>
        /// Discards all changes that have not yet been submitted.
        /// </summary>
        public void DiscardChanges()
        {
            lock (this._lockObject)
            {
                this._rootElement = XmlRepository.DataProvider.Load<TEntity>();
                this._isDirty = false;
            }
        }

        /// <summary>
        /// Disposes the current instance and submits the changes.
        /// </summary>
        public void Dispose()
        {
            lock (this._lockObject)
            {
                this.SubmitChanges();
            }
        }
    }
}