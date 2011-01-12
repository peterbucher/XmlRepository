using System;

namespace XmlRepository
{
    /// <summary>
    /// Contains the identity management for the given entity type.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public class RepositoryFor<TEntity>
    {
        /// <summary>
        /// Returns the given identity selector.
        /// </summary>
        /// <typeparam name="TIdentity">The identity type.</typeparam>
        /// <param name="identitySelector">The identity selector.</param>
        /// <returns>The identity selector.</returns>
        public static Func<TEntity, TIdentity> WithIdentity<TIdentity>(Func<TEntity, TIdentity> identitySelector)
        {
            return identitySelector;
        }
    }
}