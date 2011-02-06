using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using XmlRepository.Contracts.Mapping;

namespace XmlRepository.Mapping
{
    ///<summary>
    /// Represents a builder implementation for property mappings, which contains the start point "Map", for fluently configure mappings
    /// and mapping options for a property of the given <see tref="TEntity" />.
    ///</summary>
    internal class PropertyMappingBuilder<TEntity> : IPropertyMappingBuilder<TEntity>
    {
        private readonly IList<IPropertyMapping> _propertyMappings;

        /// <summary>
        /// Initializes a new instance of <see cref="PropertyMappingBuilder{T}" />.
        /// </summary>
        public PropertyMappingBuilder()
        {
            this._propertyMappings = new List<IPropertyMapping>();
        }

        ///<summary>
        /// Takes the selection of a property of given <see tref="TEntity" /> for further configuration.
        ///</summary>
        ///<param name="propertySelector">The selection as lambda expression, which tells to builder, which property should be mapped.</param>
        ///<typeparam name="TProperty">The property type to map.</typeparam>
        ///<returns>Returns a fluent interface, to map some class property to an xml place, like, attribute, element or content.</returns>
        public IFluentPropertyMapping<TEntity> Map<TProperty>(Expression<Func<TEntity, TProperty>> propertySelector)
        {
            var memberExpression = propertySelector.Body as MemberExpression;

            if (memberExpression == null)
            {
                throw new Exception("Mo valid property seleced with the PropertyMappingBuilder.Map()-Method.");
            }

            var propertyMapping = new PropertyMapping(typeof(TEntity).GetProperty((memberExpression.Member.Name)));
            this._propertyMappings.Add(propertyMapping);

            return new FluentPropertyMapping<TEntity>(propertyMapping);
        }

        ///<summary>
        /// Gets the property mappings for use for one seperate XmlRepository instance with different mapping requirements.
        ///</summary>
        ///<returns>The property mappings builded with this instance.</returns>
        public IEnumerable<IPropertyMapping> Build()
        {
            return this._propertyMappings;
        }

        ///<summary>
        /// Applies the resulted mappings with this builder globaly (application context) to all repositories.
        ///</summary>
        public void ApplyMappingsGlobal()
        {
            foreach (var mapping in this._propertyMappings)
            {
                XmlRepository.AddPropertyMappingFor(typeof(TEntity), mapping);
            }
        }

        /// <summary>
        /// Calls the <see cref="ApplyMappingsGlobal" /> method.
        /// </summary>
        public void Dispose()
        {
            this.ApplyMappingsGlobal();
        }
    }
}