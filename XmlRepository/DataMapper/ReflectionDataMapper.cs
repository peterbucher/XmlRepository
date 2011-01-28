using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using cherryflavored.net.ExtensionMethods.System;
using System.Collections;
using XmlRepository.Contracts;

namespace XmlRepository.DataMapper
{
    /// <summary>
    /// Provides methods to build conversion methods for T-to-XElement conversion for arbitrary
    /// types T, and vice-versa.
    /// </summary>
    internal class ReflectionDataMapper : IDataMapper
    {
        public TEntity ToObject<TEntity>(XElement singleElement) where TEntity : new()
        {
            return (TEntity)this.ToObjectInternal(singleElement, typeof(TEntity), null);
        }

        public object ToObjectInternal(XElement singleElement, Type type, Type listType)
        {
            object entity = Activator.CreateInstance(type);
            var properties = type.GetProperties();

            foreach (var element in singleElement.Elements())
            {
                var propertyInfo = properties.Single(property => property.Name == element.Name);

                if (propertyInfo.PropertyType.IsClass && propertyInfo.PropertyType != typeof (string))
                {
                    if (this.IsGenericCollectionType(propertyInfo.PropertyType))
                    {
                        Type childType = propertyInfo.PropertyType.GetGenericArguments()[0];
                        var entityList = ((IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(childType)));

                        foreach (var childElement in element.Elements())
                        {
                            entityList.Add(this.ToObjectInternal(childElement, childType, entityList.GetType()));
                        }

                        propertyInfo.SetValue(entity, entityList, null);
                    }
                    else
                    {
                        propertyInfo.SetValue(entity, this.ToObjectInternal(element, propertyInfo.PropertyType, null),
                                              null);
                    }
                }
                else
                {
                    propertyInfo.SetValue(entity, GetConvertedValue(propertyInfo.PropertyType, element.Value), null);
                }
            }

            return entity;
        }

        public XElement ToXElement<TEntity>(TEntity entity) where TEntity : new()
        {
            return this.ToXElementInternal(entity, typeof(TEntity), null);
        }

        public XElement ToXElementInternal(object entity, Type type, XElement listElement)
        {
            var properties = type.GetProperties();
            var element = new XElement(type.Name);

            foreach (var property in properties)
            {
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    if (this.IsGenericCollectionType(property.PropertyType))
                    {
                        if (listElement == null)
                        {
                            listElement = new XElement(property.Name);
                            element.Add(listElement);
                        }

                        IEnumerable listPropertyValue =
                            property.GetValue(entity, null) as IEnumerable ?? this.CreateNewEmptyList(property.PropertyType);

                        foreach (var item in listPropertyValue)
                        {
                            listElement.Add(this.ToXElementInternal(item, item.GetType(), listElement));
                        }
                    }
                }
                else
                {
                    element.Add(new XElement(property.Name, property.GetValue(entity, null)));
                }
            }

            return element;
        }

        private object GetConvertedValue(Type type, string value)
        {
            // If the property type is an enumeration or of type datetime, use the ToOrDefault method
            // to convert without an exception. So there are null values valid.
            // Convert the XElement value expression to a string for use with ToOrDefault method.)
            if (type.IsEnum || type == typeof(DateTime) || type == typeof(DateTime?) || type == typeof(Guid))
            {
                return ToOrDefaultMethod.MakeGenericMethod(type).Invoke(null, new[] { value });
            }

            if (this.IsGenericCollectionType(type) && String.IsNullOrEmpty(value))
            {
                return CreateNewEmptyList(type);
            }

            return Convert.ChangeType(value, type);
        }

        private IEnumerable CreateNewEmptyList(Type listType)
        {
            return (IEnumerable) Activator.CreateInstance(
                typeof (List<>)
                    .MakeGenericType(listType
                                         .GetGenericArguments()[0]));
        }

        private bool CircularityDetected(Type lastType, Type currentType)
        {
            var lastTypeProperties = lastType.GetProperties().Select(p => p.PropertyType);
            var currentTypeProperties = currentType.GetProperties().Select(p => p.PropertyType);

            return lastTypeProperties.Contains(currentType) && currentTypeProperties.Contains(lastType);
        }

        internal static readonly MethodInfo ToOrDefaultMethod = typeof(ExtensionMethods).GetMethod("ToOrDefault", new[] { typeof(string) });

        private bool IsGenericCollectionType(Type type)
        {
            return type.IsGenericType && type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)));
        }
    }
}