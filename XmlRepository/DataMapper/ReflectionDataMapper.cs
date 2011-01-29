using System;
using System.Reflection;
using System.Xml.Linq;
using cherryflavored.net.ExtensionMethods.System;
using XmlRepository.Contracts;

namespace XmlRepository.DataMapper
{
    /// <summary>
    /// Provides methods to build conversion methods for T-to-XElement conversion for arbitrary
    /// types T, and vice-versa.
    /// </summary>
    internal class ReflectionDataMapper : IDataMapper
    {
        public TEntity ToObject<TEntity>(XElement entityElement) where TEntity : new()
        {
            Type type = typeof (TEntity);
            object entity = Activator.CreateInstance(type);

            if(!XmlRepository.PropertyMappings.ContainsKey(type))
            {
                XmlRepository.AddDefaultMappingsFor(type);
            }

            var mappings = XmlRepository.PropertyMappings[type];

            string value = null;

            foreach (var mapping in mappings)
            {
                switch (mapping.MapType)
                {
                    case MapType.Element:
                        if (mapping.IsClassPropertyType)
                        {
                            mapping
                                .EntityType
                                .GetProperty(mapping.Name)
                                .SetValue(entity,
                                          this.GetType().GetMethod("ToObject").MakeGenericMethod(
                                              mapping.PropertyType).Invoke(
                                                  this, new object[] {entityElement.Element(mapping.Name)})
                                          , null);

                            continue;
                        }

                        value = entityElement.Element(mapping.MappedName).Value;
                        break;
                    case MapType.Attribute:
                        value = entityElement.Attribute(mapping.MappedName).Value;
                        break;
                    case MapType.Content:
                        value = entityElement.Value;
                        break;
                }

                mapping.EntityType.GetProperty(mapping.Name).SetValue(entity,
                                                                        GetConvertedValue(mapping.PropertyType, value),
                                                                        null);
            }

            return (TEntity) entity;
        }

        public XElement ToXElement<TEntity>(TEntity entity) where TEntity : new()
        {
            Type type = typeof (TEntity);
            XElement element = new XElement(type.Name);

            if (!XmlRepository.PropertyMappings.ContainsKey(type))
            {
                XmlRepository.AddDefaultMappingsFor(type);
            }

            var mappings = XmlRepository.PropertyMappings[type];

            foreach (var mapping in mappings)
            {
                object rawValue = mapping.EntityType.GetProperty(mapping.Name).GetValue(entity, null);
                string value = string.Empty;

                if(rawValue != null)
                {
                    value = rawValue.ToString();
                }

                switch (mapping.MapType)
                {
                    case MapType.Element:
                        if (mapping.IsClassPropertyType)
                        {
                            element.Add(this.GetType().GetMethod("ToXElement").MakeGenericMethod(
                                mapping.PropertyType).Invoke(
                                    this, new[]
                                              {
                                                 type.GetProperty(mapping.Name).GetValue(entity, null)
                                              }));
                        }
                        else
                        {
                            element.Add(new XElement(mapping.MappedName, value));
                        }
                        break;
                    case MapType.Attribute:
                        element.Add(new XAttribute(mapping.MappedName, value));
                        break;
                    case MapType.Content:
                        element.Add(value);
                        break;
                }
            }

            return element;
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













        //public object ToObjectInternal(XElement singleElement, Type type, Type listType)
        //{
        //    object entity = Activator.CreateInstance(type);
        //    var properties = type.GetProperties();
        //    var elements = singleElement.Elements();

        //    foreach(var property in properties)
        //    {
        //        var mapping = XmlRepository.GetMappingFor(property);

        //        XElement element = null;

        //        if(mapping.MapType == MapType.Element)
        //        {
        //            element = elements.Where(e => e.Name == mapping.Name).Single();
        //        }
        //        else if (mapping.MapType == MapType.Attribute)
        //        {
        //            //element = singleElement.Attribute(mapping.MemberName);
        //        }

        //        if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
        //        {
        //            if (this.IsGenericCollectionType(property.PropertyType))
        //            {
        //                Type childType = property.PropertyType.GetGenericArguments()[0];
        //                var entityList = ((IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(childType)));

        //                foreach (var childElement in element.Elements())
        //                {
        //                    entityList.Add(this.ToObjectInternal(childElement, childType, entityList.GetType()));
        //                }

        //                property.SetValue(entity, entityList, null);
        //            }
        //            else
        //            {
        //                property.SetValue(entity, this.ToObjectInternal(element, property.PropertyType, null),
        //                                      null);
        //            }
        //        }
        //        else
        //        {
        //            mapping.FillObject(entity, property, element);
        //        }
        //    }

        //    return entity;
        //}


        //public XElement ToXElementInternal(object entity, Type type, XElement listElement)
        //{
        //    var properties = type.GetProperties();
        //    var element = new XElement(type.Name);

        //    foreach (var property in properties)
        //    {
        //        var mapping = XmlRepository.GetMappingFor(property);
                
        //        if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
        //        {
        //            if (this.IsGenericCollectionType(property.PropertyType))
        //            {
        //                if (listElement == null)
        //                {
        //                    listElement = new XElement(property.Name);
        //                    element.Add(listElement);
        //                }

        //                IEnumerable listPropertyValue =
        //                    property.GetValue(entity, null) as IEnumerable ?? this.CreateNewEmptyList(property.PropertyType);

        //                foreach (var item in listPropertyValue)
        //                {
        //                    listElement.Add(this.ToXElementInternal(item, item.GetType(), listElement));
        //                }
        //            }
        //        }
        //        else
        //        {
        //            mapping.FillElement(element, property, entity);
        //        }
        //    }

        //    return element;
        //}

        //private object GetConvertedValue(Type type, string value)
        //{
        //    // If the property type is an enumeration or of type datetime, use the ToOrDefault method
        //    // to convert without an exception. So there are null values valid.
        //    // Convert the XElement value expression to a string for use with ToOrDefault method.)
        //    if (type.IsEnum || type == typeof(DateTime) || type == typeof(DateTime?) || type == typeof(Guid))
        //    {
        //        return ToOrDefaultMethod.MakeGenericMethod(type).Invoke(null, new[] { value });
        //    }

        //    if (this.IsGenericCollectionType(type) && String.IsNullOrEmpty(value))
        //    {
        //        return CreateNewEmptyList(type);
        //    }

        //    return Convert.ChangeType(value, type);
        //}

        //private IEnumerable CreateNewEmptyList(Type listType)
        //{
        //    return (IEnumerable) Activator.CreateInstance(
        //        typeof (List<>)
        //            .MakeGenericType(listType
        //                                 .GetGenericArguments()[0]));
        //}

        //private bool CircularityDetected(Type lastType, Type currentType)
        //{
        //    var lastTypeProperties = lastType.GetProperties().Select(p => p.PropertyType);
        //    var currentTypeProperties = currentType.GetProperties().Select(p => p.PropertyType);

        //    return lastTypeProperties.Contains(currentType) && currentTypeProperties.Contains(lastType);
        //}



        //private bool IsGenericCollectionType(Type type)
        //{
        //    return type.IsGenericType && type.GetInterfaces()
        //        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)));
        //}
    }
}