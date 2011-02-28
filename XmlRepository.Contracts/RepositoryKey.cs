using System;
using System.Collections.Generic;
using XmlRepository.Contracts.DataProviders;
using XmlRepository.Contracts.Mapping;

namespace XmlRepository.Contracts
{
    /// <summary>
    /// Represents a key for a specific repository.
    /// </summary>
    public class RepositoryKey
    {
        /// <summary>
        /// Gets or sets the repository type.
        /// </summary>
        public Type RepositoryType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the query property.
        /// </summary>
        public string QueryProperty
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the property mappings.
        /// </summary>
        public IDictionary<Type, IList<IPropertyMapping>> PropertyMappings
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data provider.
        /// </summary>
        public IDataProvider DataProvider
        {
            get;
            set;
        }

        /// <summary>
        /// Gets whether this key equals to another one.
        /// </summary>
        /// <param name="other">The other key.</param>
        /// <returns><true /> if the <paramref name="other"/> equals this key, otherwise false.</returns>
        public bool Equals(RepositoryKey other)
        {
            return Equals(this, other);
        }

        /// <summary>
        /// Gets whether this key equals to another one.
        /// </summary>
        /// <param name="obj">The other key.</param>
        /// <returns><true /> if the <paramref name="obj"/> equals this key, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            return Equals(this, obj as RepositoryKey);
        }

        /// <summary>
        /// Gets whether two keys equals, or not.
        /// </summary>
        /// <param name="obj1">The first key.</param>
        /// <param name="obj2">The second key.</param>
        /// <returns><true /> if the keys equals each other, otherwise false.</returns>
        public static bool Equals(RepositoryKey obj1, RepositoryKey obj2)
        {
            if((object.Equals(null, obj1) || object.Equals(null, obj2)) || (obj1.GetType() != obj2.GetType()))
            {
                return false;
            }

            return ReferenceEquals(obj1.PropertyMappings, obj2.PropertyMappings) &&
                   ReferenceEquals(obj1.DataProvider, obj2.DataProvider)
                   && obj1.RepositoryType.Equals(obj2.RepositoryType) && obj1.QueryProperty == obj2.QueryProperty;
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            int hashCode = this.RepositoryType.GetHashCode();

            if(this.DataProvider != null)
            {
                hashCode ^= this.DataProvider.GetHashCode();
            }

            if(this.QueryProperty != null)
            {
                hashCode ^= this.QueryProperty.GetHashCode();
            }

            return hashCode;
        }
    }
}