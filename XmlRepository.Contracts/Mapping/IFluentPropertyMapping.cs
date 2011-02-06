namespace XmlRepository.Contracts.Mapping
{
    /// <summary>
    /// Represents a fluent interface, to map some class property to an xml place, like, attribute, element or content.
    /// </summary>
    /// <typeparam name="TEntity">The entity which declares this property to map.</typeparam>
    public interface IFluentPropertyMapping<TEntity>
    {
        /// <summary>
        /// Maps the property content to an xml attribute.
        /// </summary>
        /// <returns>Returns an interface for more mapping options.</returns>
        IFluentPropertyMapping<TEntity> ToAttribute();

        /// <summary>
        /// Maps the property content to an xml attribute with an alias name.
        /// </summary>
        /// <param name="attributeAliasName">The alias name for the attribute.</param>
        /// <returns>Returns an interface for more mapping options.</returns>
        IFluentPropertyMapping<TEntity> ToAttribute(string attributeAliasName);

        /// <summary>
        /// Maps the property content to an xml element.
        /// </summary>
        /// <returns>Returns an interface for more mapping options.</returns>
        IFluentPropertyMapping<TEntity> ToElement();

        /// <summary>
        /// Maps the property content to an xml element.
        /// </summary>
        /// <param name="elementAliasName">The alias name for the element.</param>
        /// <returns>Returns an interface for more mapping options.</returns>
        IFluentPropertyMapping<TEntity> ToElement(string elementAliasName);

        /// <summary>
        /// Maps the property content to an xml content.
        /// </summary>
        /// <returns>Returns an interface for more mapping options.</returns>
        IFluentPropertyMapping<TEntity> ToContent();
    }
}