using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UmbMapper.Extensions;

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

        public Type MappedType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MappingDefinition{T}"/> class.
        /// </summary>
        public MappingDefinition()
        {
            this.mappingDefinitions = new List<PropertyMapDefinition<T>>();

            this.MappedType = typeof(T);
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
        /// <returns>The new/existing <see cref="PropertyMapDefinition{T}"/> for the property expressions</returns>
        public PropertyMapDefinition<T> AddMappingDefinition(Expression<Func<T, object>> propertyExpression)
        {
            if (!this.GetOrCreateMap(propertyExpression.ToPropertyInfo(), out PropertyMapDefinition<T> map))
            {
                this.mappingDefinitions.Add(map);
            }

            return map;
        }

        public IEnumerable<PropertyMapDefinition<T>> AddMappingDefinitions(params Expression<Func<T, object>>[] propertyExpressions)
        {
            if (propertyExpressions is null)
            {
                return Enumerable.Empty<PropertyMapDefinition<T>>();
            }

            var mapsTemp = new List<PropertyMapDefinition<T>>();

            foreach (Expression<Func<T, object>> property in propertyExpressions)
            {
                if (!this.GetOrCreateMap(property.ToPropertyInfo(), out PropertyMapDefinition<T> map))
                {
                    this.mappingDefinitions.Add(map);
                }

                mapsTemp.Add(map);
            }

            return this.mappingDefinitions.Intersect(mapsTemp);
        }

        public IEnumerable<PropertyMapDefinition<T>> MapAll()
        {
            foreach (PropertyInfo property in typeof(T).GetProperties(UmbMapperConstants.MappableFlags))
            {
                if (!this.GetOrCreateMap(property, out PropertyMapDefinition<T> map))
                {
                    this.mappingDefinitions.Add(map);
                }
            }

            return this.mappingDefinitions;
        }

        private bool GetOrCreateMap(PropertyInfo property, out PropertyMapDefinition<T> map)
        {
            bool exists = true;
            map = this.mappingDefinitions.Find(x => x.PropertyInfo.Name == property.Name);

            if (map is null)
            {
                exists = false;
                map = new PropertyMapDefinition<T>(property);
            }

            return exists;
        }
    }
}
