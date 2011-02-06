using XmlRepository.Contracts.Mapping;

namespace XmlRepository.Mapping
{
    /// <summary>
    /// Represents the implementation of a fluent interface, to map some class property to an xml place, like, attribute, element or content.
    /// </summary>
    /// <typeparam name="TEntity">The entity which declares this property to map.</typeparam>
    public class FluentPropertyMapping<TEntity> : IFluentPropertyMapping<TEntity>
    {
        private IPropertyMapping _propertyMapping;

        /// <summary>
        /// Initializes a new instance of <see cref="FluentPropertyMapping" />.
        /// </summary>
        /// <param name="propertyMapping">The property mapping to be used to set more mapping options.</param>
        public FluentPropertyMapping(IPropertyMapping propertyMapping)
        {
            this._propertyMapping = propertyMapping;
        }

        /// <summary>
        /// Maps the property content to an xml attribute.
        /// </summary>
        /// <returns>Returns an interface for more mapping options.</returns>
        public IFluentPropertyMapping<TEntity> ToAttribute()
        {
            return this.ToAttribute(null);
        }

        /// <summary>
        /// Maps the property content to an xml attribute with an alias name.
        /// </summary>
        /// <param name="attributeAliasName">The alias name for the attribute.</param>
        /// <returns>Returns an interface for more mapping options.</returns>
        public IFluentPropertyMapping<TEntity> ToAttribute(string attributeAliasName)
        {
            this._propertyMapping.XmlMappingType = XmlMappingType.Attribute;

            if (!string.IsNullOrEmpty(attributeAliasName))
            {
                this._propertyMapping.Alias = attributeAliasName;
            }

            return this;
        }

        /// <summary>
        /// Maps the property content to an xml element.
        /// </summary>
        /// <returns>Returns an interface for more mapping options.</returns>
        public IFluentPropertyMapping<TEntity> ToElement()
        {
            return this.ToElement(null);
        }

        /// <summary>
        /// Maps the property content to an xml element.
        /// </summary>
        /// <param name="elementAliasName">The alias name for the element.</param>
        /// <returns>Returns an interface for more mapping options.</returns>
        public IFluentPropertyMapping<TEntity> ToElement(string elementAliasName)
        {
            this._propertyMapping.XmlMappingType = XmlMappingType.Element;

            if (!string.IsNullOrEmpty(elementAliasName))
            {
                this._propertyMapping.Alias = elementAliasName;
            }

            return this;
        }

        /// <summary>
        /// Maps the property content to an xml content.
        /// </summary>
        /// <returns>Returns an interface for more mapping options.</returns>
        public IFluentPropertyMapping<TEntity> ToContent()
        {
            this._propertyMapping.XmlMappingType = XmlMappingType.Content;
            return this;
        }
    }
}