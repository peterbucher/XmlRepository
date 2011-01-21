namespace XmlRepository.Contracts
{
    /// <summary>
    /// Represents a repository selector.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TIdentity">The identity type.</typeparam>
    public interface IRepositorySelector<TEntity, TIdentity>
    {
        /// <summary>
        /// Gets or sets the query property.
        /// </summary>
        string QueryProperty
        {
            get;
            set;
        }
    }
}