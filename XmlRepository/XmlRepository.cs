using System;
using System.Collections.Generic;
using System.Linq;
using XmlRepository.Contracts;
using XmlRepository.Contracts.Mapping;
using XmlRepository.Contracts.DataProviders;
using XmlRepository.Contracts.DataSerializers;
using XmlRepository.DataProviders;
using XmlRepository.DataSerializers;
using XmlRepository.Mapping;

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
        private static readonly IDictionary<RepositoryKey, IXmlRepository> Repositories;

        /// <summary>
        /// Contains a list of all propertymappings for each available repositorytype.
        /// </summary>
        public static readonly IDictionary<Type, IList<IPropertyMapping>> PropertyMappings;

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
        internal static readonly string DefaultQueryProperty = "Id";

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
        /// set, the <see cref="FileDataProvider" /> is being used with the current folder as data folder.
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
            Repositories = new Dictionary<RepositoryKey, IXmlRepository>();
            PropertyMappings = new Dictionary<Type, IList<IPropertyMapping>>();
            DataMapper = new ReflectionDataMapper();
            DataSerializer = new XmlDataSerializer();
            DataProvider = new FileDataProvider(Environment.CurrentDirectory, ".xml");
        }

        /// <summary>
        /// Gets a property mapping builder instance for a given entity type.
        /// </summary>
        /// <typeparam name="TEntity">The entity type which property mappings should be defined with the mapping builder.</typeparam>
        /// <returns>A property mapping builder instance for the given entity type.</returns>
        public static IPropertyMappingBuilder<TEntity> GetPropertyMappingBuilderFor<TEntity>()
        {
            return new PropertyMappingBuilder<TEntity>();
        }

        /// <summary>
        /// Gets an xml repository for the specified entity type. If the repository has not been
        /// created yet, a new one is created; otherwise, the existing one is returned.
        /// e.g. RepositoryFor{Test}.WithIdentity(p => p.Name) for a entity named 'Test' and a identity property named 'Name' which is typed as string.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <typeparam name="TIdentity">The identity type.</typeparam>
        /// <param name="repositoryModifier">The selector for entity type and identity type.</param>
        /// <returns>An xml repository.</returns>
        public static IXmlRepository<TEntity, TIdentity> Get<TEntity, TIdentity>(IRepositoryModifier<TEntity, TIdentity> repositoryModifier) where TEntity : class, new()
        {
            lock (LockObject)
            {
                var key = new RepositoryKey
                              {
                                  RepositoryType = typeof(IXmlRepository<TEntity, TIdentity>),
                                  DataProvider = repositoryModifier.DataProvider ?? DataProvider,
                                  PropertyMappings = repositoryModifier.PropertyMappings ?? PropertyMappings,
                                  QueryProperty = repositoryModifier.QueryProperty
                              };

                IXmlRepository<TEntity, TIdentity> repository;

                if (!Repositories.ContainsKey(key))
                {
                    repository = new XmlRepository<TEntity, TIdentity>(repositoryModifier);
                    repository.Key = key;

                    Repositories.Add(key, repository);

                    // Add default mappings.
                    if (repositoryModifier.PropertyMappings != null)
                    {
                        AddDefaultPropertyMappingsFor(typeof(TEntity), repositoryModifier.PropertyMappings);
                    }
                    {
                        AddDefaultPropertyMappingsFor(typeof (TEntity));
                    }
                }
                else
                {
                    repository = Repositories[key] as IXmlRepository<TEntity, TIdentity>;
                }

                // Return the requested repository to the caller.
                return repository;
            }
        }

        internal static void RemoveRepositoryKey<TEntity, TIdentity>(XmlRepository<TEntity, TIdentity> repository) where TEntity : class, new()
        {
            Repositories.Remove(repository.Key);
        }

        internal static void AddRepositoryKey<TEntity, TIdentity>(XmlRepository<TEntity, TIdentity> repository) where TEntity : class, new()
        {
            var newKey = GetRepositoryKeyFromInstance(repository);
            repository.Key = newKey;

            Repositories.Add(newKey, repository);
        }

        private static RepositoryKey GetRepositoryKeyFromInstance<TEntity, TIdentity>(XmlRepository<TEntity, TIdentity> repository) where TEntity : class, new()
        {
            return new RepositoryKey
                       {
                           RepositoryType = repository.GetType(),
                           DataProvider = repository.DataProvider ?? DataProvider,
                           PropertyMappings = repository.PropertyMappings ?? PropertyMappings,
                           QueryProperty = repository.DefaultQueryProperty
                       };
        }

                ///<summary>
        /// Adds default object propeties to xml structure mappings for a type, if not already created.
        ///</summary>
        ///<param name="type">The type who may need default mappings.</param>
        public static void AddDefaultPropertyMappingsFor(Type type)
                {
            AddDefaultPropertyMappingsFor(type, PropertyMappings);
                }

        ///<summary>
        /// Adds default object propeties to xml structure mappings for a type, if not already created.
        ///</summary>
        ///<param name="type">The type who may need default mappings.</param>
        ///<param name="propertyMappings">The existing property mappings to which the default properties should be added.</param>
        public static void AddDefaultPropertyMappingsFor(Type type, IDictionary<Type, IList<IPropertyMapping>> propertyMappings)
        {
            lock (LockObject)
            {
                // Add default mappings.
                foreach (var property in type.GetProperties())
                {
                    AddPropertyMappingFor(property.DeclaringType, new PropertyMapping(property), propertyMappings);
                }
            }
        }

                ///<summary>
        /// Adds a custom object properties to xml structure mapping for a type.
        /// <param name="type">The type who is the mapping for.</param>
        /// <param name="mapping">The mapping for one property of the given type.</param>
        ///</summary>
        public static void AddPropertyMappingFor(Type type, IPropertyMapping mapping)
                {
                    AddPropertyMappingFor(type, mapping, PropertyMappings);
                }

        ///<summary>
        /// Adds a custom object properties to xml structure mapping for a type.
        /// <param name="type">The type who is the mapping for.</param>
        /// <param name="mapping">The mapping for one property of the given type.</param>
        ///<param name="propertyMappings">The existing property mappings to which the default properties should be added.</param>
        ///</summary>
        public static void AddPropertyMappingFor(Type type, IPropertyMapping mapping, IDictionary<Type, IList<IPropertyMapping>> propertyMappings)
        {
            lock (LockObject)
            {
                // If the mapping list for this member does not yet exist, create it.
                if(!propertyMappings.ContainsKey(type))
                {
                    propertyMappings.Add(type, new List<IPropertyMapping>());
                }

                // Do not override property mappings. (TODO: maybe a "Override" property on PropertyMappingType?)
                if(!propertyMappings[type].Any(p => p.Name == mapping.Name))
                {
                    propertyMappings[type].Add(mapping);
                }
            }
        }
    }
}