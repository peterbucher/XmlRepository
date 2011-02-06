using System;

namespace XmlRepository.Contracts.Mapping
{
    ///<summary>
    /// Contains a mapping for one class property to a xml representation.
    /// e.g. person.Name to <![CDATA[ <Person Name="Stefan"> ]]>
    ///</summary>
    public interface IPropertyMapping
    {
        ///<summary>
        /// Gets or sets the original name of the property.
        ///</summary>
        string Name
        {
            get;
            set;
        }

        ///<summary>
        /// Gets or sets optionally an alias for the property, to use within the xml representation.
        ///</summary>
        string Alias
        {
            get;
            set;
        }

        ///<summary>
        /// Gets rather the alias, if there is one, otherwise, the original property name.
        /// (To be used in all xml creating).
        ///</summary>
        string MappedName
        {
            get;
        }

        ///<summary>
        /// Gets or sets the <see cref="XmlMappingType" /> which describes in
        /// what kind of xml representation this property content should go, for e.g. element, attribute, content.
        ///</summary>
        XmlMappingType XmlMappingType
        {
            get;
            set;
        }

        ///<summary>
        /// Gets or sets the entity type, which declares the property that this <see pref="PropertyMapping" /> is representing.
        ///</summary>
        Type EntityType
        {
            get;
            set;
        }

        ///<summary>
        /// Gets or sets the property type, that maps to xml.
        ///</summary>
        Type PropertyType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets whether the <see cref="PropertyType" /> is a class.
        /// </summary>
        bool IsClassPropertyType
        {
            get;
            set;
        }

        ///<summary>
        /// Gets or sets whether the <see cref="PropertyType" /> is a generic collection.
        ///</summary>
        bool IsGenericCollectionPropertyType
        {
            get;
            set;
        }
    }
}