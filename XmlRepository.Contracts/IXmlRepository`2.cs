﻿using System;
using System.Collections.Generic;
using XmlRepository.Contracts.DataProviders;
using XmlRepository.Contracts.Mapping;

namespace XmlRepository.Contracts
{
    /// <summary>
    /// Represents an xml repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    public interface IXmlRepository<TEntity, TIdentity> : IXmlRepository, IEnumerable<TEntity> where TEntity : class, new()
    {
        /// <summary>
        /// Gets or sets the data provider that is being used to access the data sources. If not
        /// set, the FileDataProvider is being used with the current folder as data folder.
        /// </summary>
        /// <value>The data provider.</value>
        IDataProvider DataProvider
        {
            get;
            set;
        }

        ///<summary>
        /// Gets or sets the data mapper that is being used to map object to xml and vice versa.
        ///</summary>
        IDataMapper DataMapper
        {
            get;
            set;
        }

        /// <summary>
        /// Contains a list of all propertymappings for each available repositorytype.
        /// </summary>
        IDictionary<Type, IList<IPropertyMapping>> PropertyMappings
        {
            get;
            set;
        }

        /// <summary>
        /// Loads the entity with the given identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        /// <returns>
        /// The entity, if an entity was found. Otherwise, an exception is thrown.
        /// </returns>
        TEntity LoadBy(TIdentity identity);

        /// <summary>
        /// Loads the entity that matches the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// The entity, if an entity was found. Otherwise, an exception is thrown.
        /// </returns>
        TEntity LoadBy(Func<TEntity, bool> predicate);

        /// <summary>
        /// Loads all entities.
        /// </summary>
        /// <returns>
        /// A list of all entities. If no entities were found, an empty list is returned.
        /// </returns>
        IEnumerable<TEntity> LoadAll();

        /// <summary>
        /// Loads all entities that match the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// A list of all entities. If no entities were found, an empty list is returned.
        /// </returns>
        IEnumerable<TEntity> LoadAllBy(Func<TEntity, bool> predicate);

        /// <summary>
        /// Saves or updates the given entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        void SaveOnSubmit(TEntity entity);

        /// <summary>
        /// Saves or updates the given entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        void SaveOnSubmit(IEnumerable<TEntity> entities);

        /// <summary>
        /// Deletes the entity with the given identity.
        /// </summary>
        /// <param name="identity">The identity.</param>
        void DeleteOnSubmit(TIdentity identity);

        /// <summary>
        /// Deletes the entities that match given predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        void DeleteOnSubmit(Func<TEntity, bool> predicate);

        /// <summary>
        /// Deletes all entities.
        /// </summary>
        void DeleteAllOnSubmit();

        /// <summary>
        /// Submits all changes to the repository so that the changes are persistent.
        /// </summary>
        void SubmitChanges();

        /// <summary>
        /// Discards all changes that have not yet been submitted.
        /// </summary>
        void DiscardChanges();
    }
}