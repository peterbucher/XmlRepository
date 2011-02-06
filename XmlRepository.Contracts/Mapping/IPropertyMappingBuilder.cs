using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace XmlRepository.Contracts.Mapping
{
    ///<summary>
    /// Represents a builder for property mappings, which contains the start point "Map", for fluently configure mappings
    /// and mapping options for a property of the given <see tref="TEntity" />.
    ///</summary>
    public interface IPropertyMappingBuilder<TEntity> : IDisposable
    {
        ///<summary>
        /// Takes the selection of a property of given <see tref="TEntity" /> for further configuration.
        ///</summary>
        ///<param name="propertySelector">The selection as lambda expression, which tells to builder, which property should be mapped.</param>
        ///<typeparam name="TProperty">The property type to map.</typeparam>
        ///<returns>Returns a fluent interface, to map some class property to an xml place, like, attribute, element or content.</returns>
        IFluentPropertyMapping<TEntity> Map<TProperty>(Expression<Func<TEntity, TProperty>> propertySelector);

        ///<summary>
        /// Gets the property mappings for use for one seperate XmlRepository instance with different mapping requirements.
        ///</summary>
        ///<returns>The property mappings builded with this instance.</returns>
        IEnumerable<IPropertyMapping> Build();

        ///<summary>
        /// Applies the resulted mappings with this builder globaly (application context) to all repositories.
        ///</summary>
        void ApplyMappingsGlobal();
    }
}