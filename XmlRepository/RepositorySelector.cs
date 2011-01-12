using XmlRepository.Contracts;

namespace XmlRepository
{
    /// <summary>
    /// Represents a repository selector.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    public class RepositorySelector<TEntity, TIdentity> : IRepositorySelector<TEntity, TIdentity>
    {
    }
}