using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using cherryflavored.net.ExtensionMethods.System;

namespace XmlRepository
{
    /// <summary>
    /// Provides methods to build conversion methods for T-to-XElement conversion for arbitrary
    /// types T, and vice-versa.
    /// </summary>
    /// <typeparam name="T">The type T.</typeparam>
    internal static class XmlMappingBuilder<T>
    {
        /// <summary>
        /// Builds the conversion method to convert from T to XElement.
        /// </summary>
        /// <returns>The conversion method.</returns>
        internal static Func<T, XElement> ToXElement()
        {
            Type typeOfEntity = typeof(T);
            PropertyInfo[] properties = typeOfEntity.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var parameter = Expression.Parameter(typeOfEntity, "entity");

            // Represents: entity => new XElement("article", <All content expressions from above>)

            var typeNameAsXNameExpression =
                Expression.Convert(Expression.Constant(typeOfEntity.Name, typeof(string)),
                                   typeof(XName));

            var objectInitializerForContent = Expression.NewArrayInit(typeof(object),
                (from property in properties
                 let propertyNameToXName = Expression.Convert(Expression.Constant(property.Name), typeof (XName))
                 let propertyValueFromParameter = Expression.Convert(Expression.Property(parameter, property.Name), typeof (object))
                 select Expression.New(typeof (XElement).GetConstructor(new[] {typeof (XName), typeof (object)}), propertyNameToXName, propertyValueFromParameter)).Cast<Expression>().ToArray());

            var newRootExpression = Expression.New(typeof(XElement).GetConstructor(new[] { typeof(XName), typeof(object[]) }),
                                               new Expression[]
                                                   {
                                                       typeNameAsXNameExpression,
                                                       objectInitializerForContent
                                                   });

            var lambda = Expression.Lambda<Func<T, XElement>>(
                newRootExpression,
                new[] { parameter });

            // Compile the expression and return the delegate to the caller.
            return lambda.Compile();
        }

        /// <summary>
        /// Builds the conversion method to convert from XElement to T.
        /// </summary>
        /// <returns>The conversion method.</returns>
        internal static Func<XElement, T> ToObject()
        {
            // Represents: element => new Article { Id = element.Element("id").Value.ToOrDefault<Guid>(),
            //                                                           Title = element.Element("title").Value,
            // ...

            Type typeOfEntity = typeof(T);

            PropertyInfo[] properties = typeOfEntity.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Constructor for entity with no parameters (Type.EmptyTypes),
            // we will use object initalizer throught Expression.MemberInit() later...
            ConstructorInfo constructor = typeOfEntity.GetConstructor(Type.EmptyTypes);

            var parameter = Expression.Parameter(typeof(XElement), "element");
            var memberBindings = new List<MemberBinding>();

            MethodInfo toOrDefaultMethod = typeof(ExtensionMethods).GetMethod("ToOrDefault", new[] { typeof(string) });

            foreach (var property in properties)
            {
                Type propertyType = property.PropertyType;

                // Get value as XElement object.
                var valueAsXElementExpression = Expression.Call(parameter, typeof(XElement).GetMethod("Element"), Expression.Constant(
                                                                                              XName.Get(
                                                                                                  property.
                                                                                                      Name),
                                                                                              typeof(XName)));

                Expression valueExpression;

                // If the property type is an enumeration or of type datetime, use the ToOrDefault method
                // to convert without an exception. So there are null values valid.
                // Convert the XElement value expression to a string for use with ToOrDefault method.
                if (propertyType.IsEnum || propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                {
                    valueExpression = Expression.Call(
                        toOrDefaultMethod.MakeGenericMethod(new[] { propertyType }),
                        Expression.Convert(valueAsXElementExpression, typeof(string)));
                }
                else
                {
                    // Its save to convert, so use Expression.Convert().
                    // XElement can converted directly because the type defines explicit operators.
                    valueExpression = Expression.Convert(
                        valueAsXElementExpression,
                        property.PropertyType);
                }

                var memberBindingExpression = Expression.Bind(property, valueExpression);

                memberBindings.Add(memberBindingExpression);
            }

            var lambda = Expression.Lambda<Func<XElement, T>>(
                Expression.MemberInit(Expression.New(constructor),
                                      memberBindings.ToArray()),
                new[] { parameter });

            // Compile the expression and return the delegate to the caller.
            return lambda.Compile();
        }
    }
}