using System;
using System.Collections.Generic;
using XmlRepository.Contracts;
using XmlRepository.Contracts.Mapping;

namespace XmlRepository
{
    internal class RepositoryKey
    {
        public Type RepositoryType
        {
            get;
            set;
        }

        public string QueryProperty
        {
            get;
            set;
        }

        public IDictionary<Type, IList<IPropertyMapping>> PropertyMappings
        {
            get;
            set;
        }

        public IDataProvider DataProvider
        {
            get;
            set;
        }

        public bool Equals(RepositoryKey other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            return Equals(this, obj as RepositoryKey);
        }

        public static bool Equals(RepositoryKey obj1, RepositoryKey obj2)
        {
            if ((object.Equals(null, obj1) || object.Equals(null, obj2)) || (obj1.GetType() != obj2.GetType()))
            {
                return false;
            }

            return ReferenceEquals(obj1.PropertyMappings, obj2.PropertyMappings) &&
                   ReferenceEquals(obj1.DataProvider, obj2.DataProvider)
                   && obj1.RepositoryType.Equals(obj2.RepositoryType) && obj1.QueryProperty == obj2.QueryProperty;
        }

        public override int GetHashCode()
        {
            int hashCode = this.RepositoryType.GetHashCode();

            if (this.QueryProperty != null)
            {
                hashCode ^= this.QueryProperty.GetHashCode();
            }

            return hashCode;
        }
    }
}