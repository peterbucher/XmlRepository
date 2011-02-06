using System;
using System.Collections.Generic;
using XmlRepository.Contracts.DataProviders;
using XmlRepository.Contracts.Mapping;

namespace XmlRepository.Contracts
{
    /// <summary>
    /// Represents a repository modifier.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    public interface IRepositoryModifier<TEntity, TIdentity>
    {
        /// <summary>
        /// Gets or sets the query property type.
        /// </summary>
        Type QueryPropertyType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the query property.
        /// </summary>
        string QueryProperty
        {
            get;
            set;
        }

        ///<summary>
        /// Sets custom mappings for this XmlRepository instance.
        ///</summary>
        ///<param name="propertyMappings">The property mappings to use.</param>
        ///<returns>The repository modifier for further modification.</returns>
        IRepositoryModifier<TEntity, TIdentity> WithMappings(IDictionary<Type, IList<IPropertyMapping>> propertyMappings);

        ///<summary>
        /// Gets or sets the property mappings to use.
        ///</summary>
        IDictionary<Type, IList<IPropertyMapping>> PropertyMappings
        {
            get;
            set;
        }

        ///<summary>
        /// Sets custom data provider to use.
        ///</summary>
        ///<param name="dataProvider">The data provider to use.</param>
        ///<returns>The repository modifier for further modification.</returns>
        IRepositoryModifier<TEntity, TIdentity> WithDataProvider(IDataProvider dataProvider);

        ///<summary>
        /// Gets or sets the data provider to use.
        ///</summary>
        IDataProvider DataProvider
        {
            get;
            set;
        }
    }
}