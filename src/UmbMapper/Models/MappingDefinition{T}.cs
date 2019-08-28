using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace UmbMapper.Models
{
    /// <summary>
    /// Defines the property map definitions for a mapped type
    /// </summary>
    /// <typeparam name="T">The type that is being mapped to with this definition</typeparam>
    public class MappingDefinition<T>
        where T : class
    {
        private List<PropertyMapDefinition<T>> mappingDefinitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingDefinition{T}"/> class.
        /// </summary>
        public MappingDefinition()
        {
            this.mappingDefinitions = new List<PropertyMapDefinition<T>>();
        }

        /// <summary>
        /// Gets a list of all created property map definitions for this Mapping Definition
        /// </summary>
        public IEnumerable<PropertyMapDefinition<T>> MappingDefinitions
            => this.mappingDefinitions;

        /// <summary>
        /// Adds a property mapping definition to the list based on an expression
        /// </summary>
        /// <param name="propertyExpression">Mapping rule to add</param>
        /// <returns>Newly created Property Map Definition</returns>
        public PropertyMapDefinition<T> AddMappingDefinition(Expression<Func<T, object>> propertyExpression)
        {
            var definition = new PropertyMapDefinition<T>(propertyExpression);
            this.mappingDefinitions.Add(definition);

            return definition;
        }
    }
}
