using System.Xml.Linq;

namespace XmlRepository.Contracts
{
    /// <summary>
    /// Serializes or deserializes an XElement to the appropriate format.
    /// </summary>
    public interface IDataSerializer
    {
        /// <summary>
        /// Serializes the given root element.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns>The serialized string representation.</returns>
        string Serialize(XElement rootElement);

        /// <summary>
        /// Deserializes the given string representation.
        /// </summary>
        /// <param name="content">The string representation.</param>
        /// <returns>An XElement.</returns>
        XElement Deserialize(string content);
    }
}