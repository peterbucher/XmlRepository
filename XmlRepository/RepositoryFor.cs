using System;
using XmlRepository.Contracts;
using System.Linq.Expressions;

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
        public static IRepositorySelector<TEntity, TIdentity> WithIdentity<TIdentity>(Expression<Func<TEntity, TIdentity>> identitySelector)
        {
            // Sets the property name from TIdentity as QueryProperty.
            var repositorySelector = new RepositorySelector<TEntity, TIdentity>();
            var memberExpression = identitySelector.Body as MemberExpression;

            if (memberExpression != null)
            {
                repositorySelector.QueryProperty = memberExpression.Member.Name;
            }

            return repositorySelector;
        }
    }
}