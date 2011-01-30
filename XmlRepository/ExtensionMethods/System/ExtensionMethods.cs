using System;
using System.Collections.Generic;
using System.Linq;

namespace XmlRepository.ExtensionMethods.System
{
    /// <summary>
    /// Contains extension methods for System types.
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Checks whether a type is a generic collection, or any implementation of it.
        /// </summary>
        /// <param name="source">The source Type.</param>
        /// <returns><value>true</value> it the type is or implements a generic collection, otherwise <value>false</value>.</returns>
        internal static bool IsGenericCollectionType(this Type source)
        {
            return source.IsGenericType && source.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)));
        }

        /// <summary>
        /// Checks whether a type is a class, not a primitive type and also not a System.String type.
        /// </summary>
        /// <param name="source">The source Type.</param>
        /// <returns><value>true</value> it the type is a class, not a primitive or System.String, otherwise <value>false</value>.</returns>
        internal static bool IsClassType(this Type source)
        {
            return source.IsClass && source != typeof(string);
        }
    }
}