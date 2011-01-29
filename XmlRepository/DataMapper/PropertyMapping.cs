using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace XmlRepository.DataMapper
{
    ///<summary>
    /// 
    ///</summary>
    public class PropertyMapping
    {
        ///<summary>
        ///</summary>
        public PropertyMapping()
        {

        }

        ///<summary>
        ///</summary>
        ///<param name="propertyInfo"></param>
        public PropertyMapping(PropertyInfo propertyInfo)
        {
            this.PropertyType = propertyInfo.PropertyType;
            this.EntityType = propertyInfo.DeclaringType;
            this.Name = propertyInfo.Name;

            this.IsClassPropertyType = this.PropertyType.IsClass && this.PropertyType != typeof(string);
            
            if(this.IsClassPropertyType)
            {
                this.IsGenericCollectionPropertyType = this.IsGenericCollectionType(this.PropertyType);
            }
        }

        ///<summary>
        ///</summary>
        public Type PropertyType
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsClassPropertyType
        {
            get;
            set;
        }

        ///<summary>
        ///</summary>
        public bool IsGenericCollectionPropertyType
        {
            get;
            set;
        }

        ///<summary>
        ///</summary>
        public Type EntityType
        {
            get;
            set;
        }

        ///<summary>
        ///</summary>
        public string Name
        {
            get;
            set;
        }

        ///<summary>
        ///</summary>
        public string Alias
        {
            get;
            set;
        }

        ///<summary>
        ///</summary>
        public string MappedName
        {
            get
            {
                return this.Alias ?? this.Name;
            }
        }

        ///<summary>
        ///</summary>
        public MapType MapType
        {
            get;
            set;
        }

        private bool IsGenericCollectionType(Type type)
        {
            return type.IsGenericType && type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)));
        }
    }
}