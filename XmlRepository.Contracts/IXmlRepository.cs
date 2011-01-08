using System;
using System.Collections.Generic;

namespace XmlRepository.Contracts
{
    /// <summary>
    /// Represents an xml repository.
    /// </summary>
    public interface IXmlRepository : IDisposable
    {
    }

    /// <summary>
    /// Represents an xml repository.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    public interface IXmlRepository<TEntity, in TIdentity> : IXmlRepository
    {
        /// <summary>
        /// Loads the entity with the specified value for the default query property.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The entity, if an entity was found. Otherwise, an exception is thrown.
        /// </returns>
        TEntity Load(TIdentity value);

        /// <summary>
        /// Loads the entity with the specified value for the given query property.
        /// </summary>
        /// <param name="queryProperty">The name of the query property.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The entity, if an entity was found. Otherwise, an exception is thrown.
        /// </returns>
        TEntity LoadBy(string queryProperty, object value);

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
        /// Loads all entities with the specified value for the given query property.
        /// </summary>
        /// <param name="queryProperty">The name of the query property.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// A list of all entities. If no entities were found, an empty list is returned.
        /// </returns>
        IEnumerable<TEntity> LoadAllBy(string queryProperty, object value);

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
        /// Deletes the entity with the given identity value.
        /// </summary>
        /// <param name="value">The identity value.</param>
        void DeleteOnSubmit(TIdentity value);

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