using System;
using System.Collections.Generic;
using XmlRepository.Contracts;
using XmlRepository.Contracts.Mapping;

namespace XmlRepository
{
    /// <summary>
    /// Represents a repository selector.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    internal class RepositoryModifier<TEntity, TIdentity> : IRepositoryModifier<TEntity, TIdentity>
    {
        /// <summary>
        /// Gets or sets the query property type.
        /// </summary>
        public Type QueryPropertyType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the query property.
        /// </summary>
        public string QueryProperty
        {
            get;
            set;
        }

        ///<summary>
        /// Sets custom mappings for this XmlRepository instance.
        ///</summary>
        ///<param name="propertyMappings">The property mappings to use.</param>
        ///<returns>The repository modifier for further modification.</returns>
        public IRepositoryModifier<TEntity, TIdentity> WithMappings(IDictionary<Type, IList<IPropertyMapping>> propertyMappings)
        {
            this.PropertyMappings = propertyMappings;
            return this;
        }

        ///<summary>
        /// Gets or sets the property mappings to use.
        ///</summary>
        public IDictionary<Type, IList<IPropertyMapping>> PropertyMappings
        {
            get;
            set;
        }

        ///<summary>
        /// Sets custom data provider to use.
        ///</summary>
        ///<param name="dataProvider">The data provider to use.</param>
        ///<returns>The repository modifier for further modification.</returns>
        public IRepositoryModifier<TEntity, TIdentity> WithDataProvider(IDataProvider dataProvider)
        {
            this.DataProvider = dataProvider;
            return this;
        }

        ///<summary>
        /// Gets or sets the data provider to use.
        ///</summary>
        public IDataProvider DataProvider
        {
            get;
            set;
        }
    }
}