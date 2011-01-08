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
    public interface IXmlRepository<TEntity> : IXmlRepository
    {
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