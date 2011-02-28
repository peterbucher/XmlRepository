using System;
using System.Reflection;
using XmlRepository.Contracts.Mapping;
using XmlRepository.ExtensionMethods.System;

namespace XmlRepository.Mapping
{
    ///<summary>
    /// Contains a mapping for one class property to a xml representation.
    /// e.g. person.Name to <![CDATA[ <Person Name="Stefan"> ]]>
    ///</summary>
    public class PropertyMapping : IPropertyMapping
    {
        ///<summary>
        /// Initializes a new instance of <see cref="PropertyMapping" />.
        ///</summary>
        public PropertyMapping()
        {

        }

        ///<summary>
        /// Initializes a new instance of <see cref="PropertyMapping" />.
        ///</summary>
        ///<param name="propertyInfo">The property info for fetching infos.</param>
        public PropertyMapping(PropertyInfo propertyInfo)
        {
            this.PropertyType = propertyInfo.PropertyType;
            this.EntityType = propertyInfo.DeclaringType;
            this.Name = propertyInfo.Name;

            this.IsClassPropertyType = this.PropertyType.IsClassType();

            if (this.IsClassPropertyType)
            {
                this.IsGenericCollectionPropertyType = this.PropertyType.IsGenericCollectionType();
            }
        }

        ///<summary>
        /// Gets or sets the original name of the property.
        ///</summary>
        public string Name
        {
            get;
            set;
        }

        ///<summary>
        /// Gets or sets optionally an alias for the property, to use within the xml representation.
        ///</summary>
        public string Alias
        {
            get;
            set;
        }

        ///<summary>
        /// Gets rather the alias, if there is one, otherwise, the original property name.
        /// (To be used in all xml creating).
        ///</summary>
        public string MappedName
        {
            get
            {
                if(string.IsNullOrEmpty(this.Alias))
                {
                    return this.Name;
                }

                return this.Alias;
            }
        }

        ///<summary>
        /// Gets or sets the <see cref="XmlMappingType" /> which describes in
        /// what kind of xml representation this property content should go, for e.g. element, attribute, content.
        ///</summary>
        public XmlMappingType XmlMappingType
        {
            get;
            set;
        }

        ///<summary>
        /// Gets or sets the entity type, which declares the property that this <see cref="PropertyMapping" /> is representing.
        ///</summary>
        public Type EntityType
        {
            get;
            set;
        }

        ///<summary>
        /// Gets or sets the property type, that maps to xml.
        ///</summary>
        public Type PropertyType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the <see cref="PropertyType" /> is a class.
        /// </summary>
        public bool IsClassPropertyType
        {
            get;
            set;
        }

        ///<summary>
        /// Gets or sets whether the <see cref="PropertyType" /> is a generic collection.
        ///</summary>
        public bool IsGenericCollectionPropertyType
        {
            get;
            set;
        }
    }
}