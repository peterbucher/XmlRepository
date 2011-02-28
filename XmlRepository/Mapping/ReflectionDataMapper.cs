using System;
using System.Collections;
using System.Reflection;
using System.Xml.Linq;
using System.Collections.Generic;
using XmlRepository.Contracts.Mapping;

namespace XmlRepository.Mapping
{
    /// <summary>
    /// Provides methods to build conversion methods for T-to-XElement conversion for arbitrary
    /// types T, and vice-versa.
    /// </summary>
    public class ReflectionDataMapper : IDataMapper
    {
        ///<summary>
        /// Gets or sets the property mappings that belongs to a specific XmlRepository instance.
        ///</summary>
        public IDictionary<Type, IList<IPropertyMapping>> PropertyMappings
        {
            get;
            set;
        }

        /// <summary>
        /// Maps the given object to a xml-dom.
        /// </summary>
        /// <param name="entity">The entity as object.</param>
        /// <returns>The XElement filled with data from the given entity..</returns>
        public XElement ToXElement<TEntity>(TEntity entity) where TEntity : new()
        {
            return ToXElement(entity, typeof(TEntity));
        }

        /// <summary>
        /// Maps the given object to a xml-dom.
        /// </summary>
        /// <param name="entity">The entity as object.</param>
        /// <param name="entityType">The entiy type.</param>
        /// <returns>The XElement filled with data from the given entity..</returns>
        protected XElement ToXElement(object entity, Type entityType)
        {
            XmlRepository.AddDefaultPropertyMappingsFor(entityType, this.PropertyMappings);

            var mappings = this.PropertyMappings[entityType];

            var element = this.CreateElement(entityType.Name);

            foreach (var mapping in mappings)
            {
                Func<string> valueGetter =
                    () =>
                    GetStringValueOrEmptyString(mapping.EntityType.GetProperty(mapping.Name).GetValue(entity, null));

                switch (mapping.XmlMappingType)
                {
                    case XmlMappingType.Attribute:
                        element.Add(new XAttribute(mapping.MappedName, valueGetter()));
                        break;
                    case XmlMappingType.Content:
                        element.Add(valueGetter());
                        break;
                    case XmlMappingType.Element:
                        // Primitive property.
                        if (!mapping.IsClassPropertyType)
                        {
                            element.Add(this.CreateElement(mapping.MappedName, valueGetter()));
                            continue;
                        }

                        // Class property.
                        if (!mapping.IsGenericCollectionPropertyType)
                        {
                            object propertyObjectValue = entityType.GetProperty(mapping.Name).GetValue(entity, null);

                            if (propertyObjectValue == null)
                            {
                                element.Add(this.CreateElement(mapping.MappedName));
                            }
                            else
                            {
                                element.Add(this.ToXElement(propertyObjectValue, mapping.PropertyType));
                            }

                            continue;
                        }

                        // Generic collection property.
                        XElement collectionElement = this.CreateElement(mapping.MappedName);
                        element.Add(collectionElement);

                        PropertyInfo propertyType = entityType.GetProperty(mapping.Name);
                        Type listType = propertyType.PropertyType;
                        IEnumerable collection = propertyType.GetValue(entity, null) as IEnumerable;

                        if (collection != null)
                        {
                            Type collectionItemType = listType.GetGenericArguments()[0];

                            foreach (var item in collection)
                            {
                                collectionElement.Add(this.ToXElement(item, collectionItemType));
                            }
                        }

                        break;
                }
            }

            return element;
        }

        /// <summary>
        /// Maps the given XElement that represents one node to a object.
        /// </summary>
        /// <param name="entityElement">The root element.</param>
        /// <returns>The object filled with the data of given XElement.</returns>
        public TEntity ToObject<TEntity>(XElement entityElement) where TEntity : new()
        {
            return (TEntity)this.ToObject(entityElement, typeof(TEntity));
        }

        /// <summary>
        /// Maps the given XElement that represents one node to a object.
        /// </summary>
        /// <param name="entityElement">The root element.</param>
        /// <param name="entityType">The entiy type.</param>
        /// <returns>The object filled with the data of given XElement.</returns>
        protected object ToObject(XElement entityElement, Type entityType)
        {
            XmlRepository.AddDefaultPropertyMappingsFor(entityType, this.PropertyMappings);

            var propertyMappings = this.PropertyMappings ?? XmlRepository.PropertyMappings;
            var mappings = propertyMappings[entityType];

            object entity = Activator.CreateInstance(entityType);

            foreach (var mapping in mappings)
            {
                Action<string> valueSetter =
                    value =>
                    mapping.EntityType.GetProperty(mapping.Name).SetValue(entity,
                                                                          GetConvertedValue(mapping.PropertyType,
                                                                                                 value), null);

                switch (mapping.XmlMappingType)
                {
                    case XmlMappingType.Attribute:
                        if (entityElement.HasAttributes)
                        {
                            valueSetter(entityElement.Attribute(mapping.MappedName).Value);
                        }
                        break;
                    case XmlMappingType.Content:
                        valueSetter(entityElement.Value);
                        break;
                    case XmlMappingType.Element:
                        // Primitive propert.
                        if (!mapping.IsClassPropertyType)
                        {
                            if (entityElement.HasElements)
                            {
                                valueSetter(entityElement.Element(mapping.MappedName).Value);
                            }

                            continue;
                        }

                        // Class property.
                        if (!mapping.IsGenericCollectionPropertyType)
                        {
                            object possibleValue = this.ToObject(entityElement.Element(mapping.Name),
                                                                 mapping.PropertyType);

                            if (possibleValue != null)
                            {
                                mapping
                                    .EntityType
                                    .GetProperty(mapping.Name)
                                    .SetValue(entity, possibleValue, null);

                                continue;
                            }
                        }

                        // Generic collection property.
                        PropertyInfo propertyInfo = entityType.GetProperty(mapping.Name);

                        Type listType = propertyInfo.PropertyType;
                        IList entityList = propertyInfo.GetValue(entity, null) as IList;
                        Type entityItemType = listType.GetGenericArguments()[0];

                        if (entityList == null)
                        {
                            entityList =
                                ((IList)
                                 Activator.CreateInstance(typeof(List<>).MakeGenericType(entityItemType)));
                        }

                        var collectionElement = entityElement.Element(mapping.MappedName);

                        if (collectionElement.HasElements)
                        {
                            foreach (var element in collectionElement.Elements())
                            {
                                entityList.Add(this.ToObject(element, entityItemType));
                            }

                            mapping.EntityType.GetProperty(mapping.Name).SetValue(entity, entityList, null);
                        }

                        break;
                }
            }

            return entity;
        }

        // Cached ToOrDefault-method.
        internal static readonly MethodInfo ToOrDefaultMethod =
            typeof(cherryflavored.net.ExtensionMethods.System.ExtensionMethods).GetMethod("ToOrDefault", new[] { typeof(string) });

        /// <summary>
        /// General type convertion.
        /// </summary>
        /// <param name="type">The type in which the value to be parsed / converted.</param>
        /// <param name="value">The value as string.</param>
        /// <returns>The parsed / converted value.</returns>
        private static object GetConvertedValue(Type type, string value)
        {
            // If the property type is an enumeration or of type datetime, use the ToOrDefault method
            // to convert without an exception. So there are null values valid.
            // Convert the XElement value expression to a string for use with ToOrDefault method.)
            if (type.IsEnum || type == typeof(DateTime) || type == typeof(DateTime?) || type == typeof(Guid))
            {
                return ToOrDefaultMethod.MakeGenericMethod(type).Invoke(null, new[] { value });
            }

            return Convert.ChangeType(value, type);
        }

        /// <summary>
        /// Gets the value as string or an empty string if the value is null.
        /// </summary>
        /// <param name="propertyValueAsObject">The object to be represent in a string.</param>
        /// <returns>The string representation of the value.</returns>
        private static string GetStringValueOrEmptyString(object propertyValueAsObject)
        {
            return propertyValueAsObject != null ? propertyValueAsObject.ToString() : string.Empty;
        }

        /// <summary>
        /// Creates a new <see cref="XElement" />.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The <see cref="XElement" />.</returns>
        private XElement CreateElement(XName name)
        {
            return this.CreateElement(name, null);
        }

        /// <summary>
        /// Creates a new <see cref="XElement" />.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="content">The content to be added to the <see cref="XElement" />.</param>
        /// <returns>The <see cref="XElement" />.</returns>
        private XElement CreateElement(XName name, object content)
        {
            if (content == null)
            {
                return new XElement(name);
            }

            return new XElement(name, content);
        }
    }
}