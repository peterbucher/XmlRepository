using System;
using System.Collections;
using System.Reflection;
using System.Xml.Linq;
using cherryflavored.net.ExtensionMethods.System;
using XmlRepository.Contracts;
using System.Collections.Generic;

namespace XmlRepository.DataMapper
{
    /// <summary>
    /// Provides methods to build conversion methods for T-to-XElement conversion for arbitrary
    /// types T, and vice-versa.
    /// </summary>
    internal class ReflectionDataMapper : IDataMapper
    {
        public XElement ToXElement<TEntity>(TEntity entity) where TEntity : new()
        {
            Type type = typeof(TEntity);
            XElement element = new XElement(type.Name);

            XmlRepository.AddDefaultMappingsFor(type);

            var mappings = XmlRepository.PropertyMappings[type];

            foreach (var mapping in mappings)
            {
                object rawValue = mapping.EntityType.GetProperty(mapping.Name).GetValue(entity, null);
                string value = string.Empty;

                if (rawValue != null)
                {
                    value = rawValue.ToString();
                }

                switch (mapping.MapType)
                {
                    case MapType.Attribute:
                        element.Add(new XAttribute(mapping.MappedName, value));
                        break;
                    case MapType.Content:
                        element.Add(value);
                        break;
                    default:
                        if (mapping.IsClassPropertyType)
                        {
                            if (mapping.IsGenericCollectionPropertyType)
                            {
                                XElement listElement = new XElement(mapping.MappedName);
                                element.Add(listElement);

                                PropertyInfo propertyType = type.GetProperty(mapping.Name);
                                Type listType = propertyType.PropertyType;
                                IEnumerable collection = propertyType.GetValue(entity, null) as IEnumerable;

                                if (collection != null)
                                {
                                    Type collectionItemType = listType.GetGenericArguments()[0];

                                    foreach (var item in collection)
                                    {
                                        // TODO: Propertymapping
                                        listElement.Add(
                                            this.GetType().
                                                GetMethod("ToXElement").
                                                MakeGenericMethod(collectionItemType)
                                                .Invoke(this, new[] { item }));
                                    }
                                }
                            }
                            else
                            {
                                object propertyValue = type.GetProperty(mapping.Name).GetValue(entity, null);

                                if (propertyValue != null)
                                {
                                    element.Add(this.GetType().GetMethod("ToXElement").MakeGenericMethod(
                                        mapping.PropertyType).Invoke(
                                            this, new[]
                                                  {
                                                      propertyValue
                                                  }));
                                }
                                else
                                {
                                    element.Add(new XElement(mapping.MappedName));
                                }
                            }
                        }
                        else
                        {
                            element.Add(new XElement(mapping.MappedName, value));
                        }
                        break;
                }
            }

            return element;
        }

        public TEntity ToObject<TEntity>(XElement entityElement) where TEntity : new()
        {
            Type type = typeof(TEntity);
            object entity = Activator.CreateInstance(type);

            XmlRepository.AddDefaultMappingsFor(type);

            var mappings = XmlRepository.PropertyMappings[type];

            string value = string.Empty;

            foreach (var mapping in mappings)
            {
                switch (mapping.MapType)
                {
                    case MapType.Attribute:
                        if (entityElement.HasAttributes)
                        {
                            value = entityElement.Attribute(mapping.MappedName).Value;
                        }
                        break;
                    case MapType.Content:
                        value = entityElement.Value;
                        break;
                    default:
                        if (mapping.IsClassPropertyType)
                        {
                            if (mapping.IsGenericCollectionPropertyType)
                            {
                                PropertyInfo propertyInfo = type.GetProperty(mapping.Name);

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
                                        // TODO: Propertymapping
                                        entityList.Add(this.GetType().GetMethod("ToObject").MakeGenericMethod(
                                            entityItemType).Invoke(
                                                this,
                                                new object[] { element }));
                                    }

                                    mapping.EntityType.GetProperty(mapping.Name).SetValue(entity, entityList, null);
                                }
                            }
                            else
                            {
                                object possibleValue = this.GetType().GetMethod("ToObject").MakeGenericMethod(
                                    mapping.PropertyType).Invoke(
                                        this, new object[] {entityElement.Element(mapping.Name)});

                                if(possibleValue != null)
                                {
                                    mapping
                                        .EntityType
                                        .GetProperty(mapping.Name)
                                        .SetValue(entity, possibleValue, null);
                                } else
                                {
                                    continue;
                                }
                            }

                            continue;
                        }

                        if (entityElement.HasElements)
                        {
                            value = entityElement.Element(mapping.MappedName).Value;
                        }
                        break;
                }

                mapping.EntityType.GetProperty(mapping.Name).SetValue(entity,
                                                                        GetConvertedValue(mapping.PropertyType, value),
                                                                        null);
            }

            return (TEntity)entity;
        }

        internal static readonly MethodInfo ToOrDefaultMethod = typeof(ExtensionMethods).GetMethod("ToOrDefault", new[] { typeof(string) });

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
    }
}