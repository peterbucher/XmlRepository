using System.Xml.Linq;

namespace XmlRepository.Contracts
{
    /// <summary>
    /// Maps the data of a XElement to an object and vice versa.
    /// </summary>
    public interface IDataMapper
    {
        /// <summary>
        /// Maps the given object to a xml-dom.
        /// </summary>
        /// <param name="entity">The entity as object.</param>
        /// <returns>The XElement filled with data from the given entity..</returns>
        XElement ToXElement<TEntity>(TEntity entity) where TEntity : new();

        /// <summary>
        /// Maps the given XElement that represents one node to a object.
        /// </summary>
        /// <param name="entityElement">The root element.</param>
        /// <returns>The object filled with the data of given XElement.</returns>
        TEntity ToObject<TEntity>(XElement entityElement) where TEntity : new();
    }
}