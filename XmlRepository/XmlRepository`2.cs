﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using cherryflavored.net;
using XmlRepository.Contracts;
using XmlRepository.Contracts.Mapping;
using XmlRepository.Contracts.DataProviders;
using XmlRepository.Mapping;

namespace XmlRepository
{
    /// <summary>
    /// Represents an xml repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    internal class XmlRepository<TEntity, TIdentity> : IXmlRepository<TEntity, TIdentity> where TEntity : class, new()
    {
        /// <summary>
        /// Contains the lock object for instance locking.
        /// </summary>
        private readonly object _lockObject = new object();

        /// <summary>
        /// Contains the name of the entity's property that is used as default key within queries.
        /// </summary>
        private string _defaultQueryProperty;

        /// <summary>
        /// Contains the property mappings that belongs to this instance.
        /// </summary>
        private IDictionary<Type, IList<IPropertyMapping>> _propertyMappings;

        /// <summary>
        /// Contains the data provider that belongs to this instance.
        /// </summary>
        private IDataProvider _dataProvider;

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
        /// Creates a new instance of the <see cref="XmlRepository" /> type with a given query property.
        /// </summary>
        /// <param name="repositoryModifier">The repository modifier.</param>
        internal XmlRepository(IRepositoryModifier<TEntity, TIdentity> repositoryModifier)
        {
            lock (this._lockObject)
            {
                // Set up the default query property.
                this._defaultQueryProperty = repositoryModifier.QueryProperty ?? XmlRepository.DefaultQueryProperty;
                this._defaultQueryPropertyInfo = typeof(TEntity).GetProperty(this._defaultQueryProperty);

                // Check whether the default query property was found.
                if (this._defaultQueryPropertyInfo == null)
                {
                    throw new Exception(
                        string.Format(
                            "The identifier property '{0}' has not been found on type '{1}' (is it missing, private, static or misspelled?)",
                            this._defaultQueryProperty, typeof(TEntity).AssemblyQualifiedName));
                }

                this.DataMapper = new ReflectionDataMapper();

                if (repositoryModifier.PropertyMappings != null)
                {
                    this.DataMapper.PropertyMappings = repositoryModifier.PropertyMappings;
                }
                else
                {
                    this.DataMapper.PropertyMappings = XmlRepository.PropertyMappings;
                }

                this._propertyMappings = repositoryModifier.PropertyMappings ?? XmlRepository.PropertyMappings;
                this._dataProvider = repositoryModifier.DataProvider ?? XmlRepository.DataProvider;


                // If the data source has been changed, reload the in-memory representation of the
                // data source.
                EventHandler<DataSourceChangedEventArgs> handler = (sender, eventArgs) =>
                {
                    if (eventArgs.EntityType ==
                        typeof(TEntity).Name)
                    {
                        this.DiscardChanges();
                    }
                };

                // Ensure we don't end up being triggered multiple times by the event.
                this._dataProvider.DataSourceChanged -= handler;
                this._dataProvider.DataSourceChanged += handler;

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
                try
                {
                    var idMapping = this._propertyMappings[typeof(TEntity)].Single(m => m.Name == this._defaultQueryProperty);

                    Func<XElement, bool> predicate;

                    switch (idMapping.XmlMappingType)
                    {
                        case XmlMappingType.Element:
                            predicate = e => e.Element(key).Value.Equals(value.ToString());
                            break;
                        case XmlMappingType.Attribute:
                            predicate = e => e.Attribute(key).Value.Equals(value.ToString());
                            break;
                        default:
                            predicate = e => e.Name.Equals(key) && e.Value.Equals(value.ToString());
                            break;
                    }

                    return this._rootElement.Elements().Where(predicate);
                }
                catch (NullReferenceException e)
                {
                    throw new ElementNotFoundException(string.Format("'element {0}' not found.", key), e);
                }
            }

        }

        ///<summary>
        /// The repository key.
        ///</summary>
        public RepositoryKey Key
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data provider that is being used to access the data sources. If not
        /// set, the FileDataProvider is being used with the current folder as data folder.
        /// </summary>
        /// <value>The data provider.</value>
        public IDataProvider DataProvider
        {
            get
            {
                return this._dataProvider;
            }

            set
            {
                XmlRepository.RemoveRepositoryKey(this);

                this._dataProvider = value;

                XmlRepository.AddRepositoryKey(this);
            }
        }

        public IDataMapper DataMapper
        {
            get;
            set;
        }

        /// <summary>
        /// Contains a list of all propertymappings for each available repositorytype.
        /// </summary>
        public IDictionary<Type, IList<IPropertyMapping>> PropertyMappings
        {
            get
            {
                return this._propertyMappings;
            }

            set
            {
                this.DataMapper.PropertyMappings = value;

                XmlRepository.RemoveRepositoryKey(this);

                this._propertyMappings = value;

                XmlRepository.AddRepositoryKey(this);
            }
        }

        /// <summary>
        /// Gets or sets the name of the entity's property that is used as default key within
        /// queries.
        /// </summary>
        /// <value>
        /// The name of the entity's property that is used as default key within queries.
        /// </value>
        internal string DefaultQueryProperty
        {
            get
            {
                return this._defaultQueryProperty;
            }
        }

        /// <summary>
        /// Loads the entity with the given identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>
        /// The entity, if an entity was found. Otherwise, an exception is thrown.
        /// </returns>
        public TEntity LoadBy(TIdentity identity)
        {
            lock (this._lockObject)
            {
                try
                {
                    return this.DataMapper.ToObject<TEntity>(
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
        public TEntity LoadBy(Func<TEntity, bool> predicate)
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
        public IEnumerable<TEntity> LoadAll()
        {
            lock (this._lockObject)
            {
                return
                    (from e in this._rootElement.Elements()
                     select this.DataMapper.ToObject<TEntity>(e)).ToList();
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
                Enforce.NotNull(() => entity);

                // Check whether the entity already exists. If so, remove it (and simulate an
                // update this way).
                var defaultQueryPropertyValue = _defaultQueryPropertyInfo.GetValue(entity, null);

                var element = (this.GetElementsByKeyValuePair(this._defaultQueryProperty, defaultQueryPropertyValue)).SingleOrDefault();

                if (element != null)
                {
                    element.Remove();
                }

                // Add the entity.
                this._rootElement.Add(this.DataMapper.ToXElement(entity));
                this._isDirty = true;
            }
        }

        /// <summary>
        /// Saves or updates the given entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public void SaveOnSubmit(IEnumerable<TEntity> entities)
        {
            lock (this._lockObject)
            {
                Enforce.NotNull(() => entities);

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
        public void DeleteOnSubmit(TIdentity identity)
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
        public void DeleteOnSubmit(Func<TEntity, bool> predicate)
        {
            lock (this._lockObject)
            {
                Enforce.NotNull(() => predicate);

                (from e in this._rootElement.Elements()
                 where predicate(this.DataMapper.ToObject<TEntity>(e))
                 select e).Remove();
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

                this._dataProvider.Save<TEntity>(
                    XmlRepository.DataSerializer.Serialize(
                        this._rootElement));
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
                this._rootElement =
                    XmlRepository.DataSerializer.Deserialize(
                        this._dataProvider.Load<TEntity>());
                this._isDirty = false;
            }
        }

        ///<summary>
        /// Gets the xml representation of the current inmemory status of this XmlRepository instance.
        ///</summary>
        ///<returns>The xml representation as <see cref="XElement" />.</returns>
        public XElement GetXmlRepresentation()
        {
            return XElement.Parse(this._rootElement.ToString());
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

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<TEntity> GetEnumerator()
        {
            lock (this._lockObject)
            {
                return
                    (from e in this._rootElement.Elements()
                     select this.DataMapper.ToObject<TEntity>(e)).GetEnumerator();
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