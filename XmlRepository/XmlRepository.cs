using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using cherryflavored.net;
using XmlRepository.Contracts;
using XmlRepository.DataMapper;
using XmlRepository.DataProviders;
using XmlRepository.DataSerializers;

namespace XmlRepository
{
    /// <summary>
    /// Represents the configuration class for xml repositories.
    /// </summary>
    public static class XmlRepository
    {
        /// <summary>
        /// Contains the lock object for type locking.
        /// </summary>
        private static readonly object LockObject = new object();

        /// <summary>
        /// Contains a list of all repositories created so far, for reuse.
        /// </summary>
        private static readonly IDictionary<Type, IXmlRepository> Repositories;

        /// <summary>
        /// Contains a list of all propertymappings for each available repositorytype.
        /// </summary>
        internal static readonly IDictionary<Type, List<PropertyMapping>> PropertyMappings;

        /// <summary>
        /// Gets the name of the root element.
        /// </summary>
        internal static readonly string RootElementName = "root";

        /// <summary>
        /// Gets the xml content of the empty root element.
        /// </summary>
        internal static readonly string RootElementXml = string.Format("<{0} />", RootElementName);

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

        ///<summary>
        /// Gets or sets the data mapper. If not set, the ReflectionDataMapper is being used.
        ///</summary>
        public static IDataMapper DataMapper
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data serializer. If not set, the XmlDataSerializer is being used.
        /// </summary>
        public static IDataSerializer DataSerializer
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data provider that is being used to access the data sources. If not
        /// set, the FileProvider is being used with the current folder as data folder.
        /// </summary>
        /// <value>The data provider.</value>
        public static IDataProvider DataProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes the <see cref="XmlRepository" /> type.
        /// </summary>
        static XmlRepository()
        {
            Repositories = new Dictionary<Type, IXmlRepository>();
            PropertyMappings = new Dictionary<Type, List<PropertyMapping>>();
            DataMapper = new ReflectionDataMapper();
            DataSerializer = new XmlDataSerializer();
            DataProvider = new FileDataProvider(Environment.CurrentDirectory, ".xml");
            DefaultQueryProperty = "Id";
        }

        /// <summary>
        /// Gets an xml repository for the specified entity type. If the repository has not been
        /// created yet, a new one is created; otherwise, the existing one is returned.
        /// e.g. RepositoryFor{Test}.WithIdentity(p => p.Name) for a entity named 'Test' and a identity property named 'Name' which is typed as string.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TIdentity">The identity type.</typeparam>
        /// <returns>An xml repository.</returns>
        public static IXmlRepository<TEntity, TIdentity> Get<TEntity, TIdentity>(IRepositorySelector<TEntity, TIdentity> repositorySelector) where TEntity : class, new()
        {
            lock (LockObject)
            {
                // If the repository does not yet exist, create it.
                if (!Repositories.ContainsKey(typeof(TEntity)))
                {
                    Repositories.Add(typeof(TEntity), new XmlRepository<TEntity, TIdentity>(repositorySelector.QueryProperty));

                    // Add default mappings.
                    AddDefaultMappingsFor(typeof (TEntity));
                }

                // Return the requested repository to the caller.
                return Repositories[typeof(TEntity)] as IXmlRepository<TEntity, TIdentity>;
            }
        }

        ///<summary>
        ///</summary>
        ///<param name="type"></param>
        public static void AddDefaultMappingsFor(Type type)
        {
            lock(LockObject)
            {
                // Add default mappings.
                foreach (var property in type.GetProperties())
                {
                    if(!PropertyMappings[type].Any(p => p.Name == property.Name))
                    {
                        AddMappingFor(property);
                    }
                }
            }
        }

        ///<summary>
        ///</summary>
        private static void AddMappingFor(PropertyInfo propertyInfo)
        {
            lock(LockObject)
            {
                AddMappingFor(propertyInfo.DeclaringType, new PropertyMapping(propertyInfo));
            }
        }

        ///<summary>
        ///</summary>
        public static void AddMappingFor(Type type, PropertyMapping mapping)
        {
            lock (LockObject)
            {
                // If the mapping list for this member does not yet exist, create it.
                if (!PropertyMappings.ContainsKey(type))
                {
                    PropertyMappings.Add(type, new List<PropertyMapping>());
                }

                if (!PropertyMappings[type].Any(p => p.Name == mapping.Name))
                {
                    PropertyMappings[type].Add(mapping);
                }
            }
        }
    }

    /// <summary>
    /// Represents an xml repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    internal class XmlRepository<TEntity, TIdentity> : IXmlRepository<TEntity, TIdentity>
        where TEntity : class, new()
    {
        /// <summary>
        /// Contains the lock object for instance locking.
        /// </summary>
        private readonly
            object _lockObject = new object();

        /// <summary>
        /// Contains the name of the entity's property that is used as default key within queries.
        /// </summary>
        private readonly
        string _defaultQueryProperty;

        /// <summary>
        /// Contains the property info of the entity's property that is used as default key within
        /// queries.
        /// </summary>
        private readonly
        PropertyInfo _defaultQueryPropertyInfo;

        /// <summary>
        /// Contains a reference to the root element of the data source.
        /// </summary>
        private
        XElement _rootElement;

        /// <summary>
        /// Contains <c>true</c> if the in-memory representation of the data source contains
        /// changes that have not yet been submitted to the data source; <c>false</c> otherwise.
        /// </summary>
        private
        bool _isDirty;

        /// <summary>
        /// Creates a new instance of the <see cref="XmlRepository" /> type with a given query property.
        /// </summary>
        /// <param name="queryProperty"></param>
        internal
        XmlRepository(string
        queryProperty)
        {
            lock (this._lockObject)
            {
                // Set up the default query property.
                this._defaultQueryProperty = queryProperty ?? XmlRepository.DefaultQueryProperty;
                this._defaultQueryPropertyInfo = typeof(TEntity).GetProperty(this._defaultQueryProperty);

                // Check whether the default query property was found.
                if (this._defaultQueryPropertyInfo == null)
                {
                    throw new Exception(
                        string.Format(
                            "The property '{0}' has not been found on type '{1}' (is it missing, private, static or misspelled?)",
                            this._defaultQueryProperty, typeof(TEntity).AssemblyQualifiedName));
                }

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
        private
            IEnumerable<XElement> GetElementsByKeyValuePair
            (string key, object value)
        {
            lock (this._lockObject)
            {
                try
                {
                    var elementsByKeyAndValue = this._rootElement.Elements()
                        .Where(e => e.Element(key).Value.Equals(value.ToString()));

                    return elementsByKeyAndValue;

                }
                catch (NullReferenceException e)
                {
                    throw new ElementNotFoundException(string.Format("'element {0}' not found.", key), e);
                }
            }
        }

        /// <summary>
        /// Loads the entity with the given identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>
        /// The entity, if an entity was found. Otherwise, an exception is thrown.
        /// </returns>
        public
            TEntity LoadBy
            (TIdentity
            identity)
        {
            lock (this._lockObject)
            {
                try
                {
                    return XmlRepository.DataMapper.ToObject<TEntity>(
                        this.GetElementsByKeyValuePair(this._defaultQueryProperty, identity).Single());
                }
                catch (InvalidOperationException e)
                {
                    throw new EntityNotFoundException(null, e);
                }
            }
        }

        /// <summary>
        /// Loads the entity that matches the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// The entity, if an entity was found. Otherwise, an exception is thrown.
        /// </returns>
        public
            TEntity LoadBy
            (Func<TEntity, bool>
            predicate)
        {
            lock (this._lockObject)
            {
                var entities = this.LoadAll().Where(predicate);

                try
                {
                    return entities.Single();
                }
                catch (InvalidOperationException e)
                {
                    throw new EntityNotFoundException(
                        string.Format(
                            "Entity of type '{0}' that matches to 'TODO: Wie predicate als string darstellen', was not fount (no elements, no matching or more than one matching element)",
                            typeof(TEntity).Name), e);
                }
            }
        }

        /// <summary>
        /// Loads all entities.
        /// </summary>
        /// <returns>
        /// A list of all entities. If no entities were found, an empty list is returned.
        /// </returns>
        public
            IEnumerable<TEntity> LoadAll
            ()
        {
            lock (this._lockObject)
            {
                return
                    (from e in this._rootElement.Elements()
                     select XmlRepository.DataMapper.ToObject<TEntity>(e)).ToList();
            }
        }

        /// <summary>
        /// Loads all entities that match the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// A list of all entities. If no entities were found, an empty list is returned.
        /// </returns>
        public
            IEnumerable<TEntity> LoadAllBy
            (Func<TEntity, bool>
            predicate)
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
        public
            void SaveOnSubmit
            (TEntity
            entity)
        {
            lock (this._lockObject)
            {
                Enforce.NotNull(entity, () => entity);

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
                this._rootElement.Add(XmlRepository.DataMapper.ToXElement(entity));
                this._isDirty = true;
            }
        }

        /// <summary>
        /// Saves or updates the given entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public
            void SaveOnSubmit
            (IEnumerable<TEntity> entities)
        {
            lock (this._lockObject)
            {
                Enforce.NotNull(entities, () => entities);

                foreach (var entity in entities)
                {
                    this.SaveOnSubmit(entity);
                }
            }
        }

        /// <summary>
        /// Deletes the entity with the given identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        public
            void DeleteOnSubmit
            (TIdentity
            identity)
        {
            try
            {
                (this.GetElementsByKeyValuePair(this._defaultQueryProperty, identity)).Remove();
            }
            catch (InvalidOperationException e)
            {
                throw new EntityNotFoundException(null, e);
            }
            this._isDirty = true;
        }

        /// <summary>
        /// Deletes the entities that match given predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        public
            void DeleteOnSubmit
            (Func<TEntity, bool>
            predicate)
        {
            lock (this._lockObject)
            {
                Enforce.NotNull(predicate, () => predicate);

                (from e in this._rootElement.Elements()
                 where predicate(XmlRepository.DataMapper.ToObject<TEntity>(e))
                 select e).Remove();
                this._isDirty = true;
            }
        }

        /// <summary>
        /// Deletes all entities.
        /// </summary>
        public
            void DeleteAllOnSubmit
            ()
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
        public
            void SubmitChanges
            ()
        {
            lock (this._lockObject)
            {
                if (!this._isDirty)
                {
                    return;
                }

                XmlRepository.DataProvider.Save<TEntity>(
                    XmlRepository.DataSerializer.Serialize(
                        this._rootElement));
                this._isDirty = false;
            }
        }

        /// <summary>
        /// Discards all changes that have not yet been submitted.
        /// </summary>
        public
            void DiscardChanges
            ()
        {
            lock (this._lockObject)
            {
                this._rootElement =
                    XmlRepository.DataSerializer.Deserialize(
                        XmlRepository.DataProvider.Load<TEntity>());
                this._isDirty = false;
            }
        }

        /// <summary>
        /// Disposes the current instance and submits the changes.
        /// </summary>
        public
            void Dispose
            ()
        {
            lock (this._lockObject)
            {
                this.SubmitChanges();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public
            IEnumerator<TEntity> GetEnumerator
            ()
        {
            lock (this._lockObject)
            {
                return
                    (from e in this._rootElement.Elements()
                     select XmlRepository.DataMapper.ToObject<TEntity>(e)).GetEnumerator();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}