using System;
using System.Collections;
using System.Reflection;
using System.Xml.Linq;
using XmlRepository.Contracts;
using System.Collections.Generic;

namespace XmlRepository.Mapping
{
    /// <summary>
    /// Provides methods to build conversion methods for T-to-XElement conversion for arbitrary
    /// types T, and vice-versa.
    /// </summary>
    internal class ReflectionDataMapper : IDataMapper
    {
        public XElement ToXElement<TEntity>(TEntity entity) where TEntity : new()
        {
            return ToXElement(entity, typeof(TEntity));
        }

        protected XElement ToXElement(object entity, Type entityType)
        {
            XmlRepository.AddDefaultPropertyMappingsFor(entityType);
            var mappings = XmlRepository.PropertyMappings[entityType];

            var element = this.CreateElement(entityType.Name);

            foreach (var mapping in mappings)
            {
                Func<string> valueGetter =
                    () =>
                    this.GetStringValueOrEmptyString(mapping.EntityType.GetProperty(mapping.Name).GetValue(entity, null));

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

        public TEntity ToObject<TEntity>(XElement entityElement) where TEntity : new()
        {
            return (TEntity)this.ToObject(entityElement, typeof(TEntity));
        }

        protected object ToObject(XElement entityElement, Type entityType)
        {
            XmlRepository.AddDefaultPropertyMappingsFor(entityType);
            var mappings = XmlRepository.PropertyMappings[entityType];

            object entity = Activator.CreateInstance(entityType);

            foreach (var mapping in mappings)
            {
                Action<string> valueSetter =
                    value =>
                    mapping.EntityType.GetProperty(mapping.Name).SetValue(entity,
                                                                          this.GetConvertedValue(mapping.PropertyType,
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

                        if (entityElement.Element(mapping.MappedName).HasElements)
                        {
                            foreach (var element in entityElement.Element(mapping.Name).Elements())
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

        internal static readonly MethodInfo ToOrDefaultMethod =
            typeof(cherryflavored.net.ExtensionMethods.System.ExtensionMethods).GetMethod("ToOrDefault", new[] { typeof(string) });

        private object GetConvertedValue(Type type, string value)
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

        private string GetStringValueOrEmptyString(object propertyValueAsObject)
        {
            return propertyValueAsObject != null ? propertyValueAsObject.ToString() : string.Empty;
        }

        private XElement CreateElement(XName name)
        {
            return this.CreateElement(name, null);
        }

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