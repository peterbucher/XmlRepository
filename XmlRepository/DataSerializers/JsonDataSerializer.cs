using System.Xml.Linq;
using Newtonsoft.Json;
using XmlRepository.Contracts.DataSerializers;

namespace XmlRepository.DataSerializers
{
    /// <summary>
    /// Serializes or deserializes an XElement to the appropriate format.
    /// </summary>
    public class JsonDataSerializer : IDataSerializer
    {
        /// <summary>
        /// Serializes the given root element.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns>The serialized string representation.</returns>
        public string Serialize(XElement rootElement)
        {
            return JsonConvert.SerializeXNode(rootElement, Formatting.Indented);
        }

        /// <summary>
        /// Deserializes the given string representation.
        /// </summary>
        /// <param name="content">The string representation.</param>
        /// <returns>An XElement.</returns>
        public XElement Deserialize(string content)
        {
                if(content.Length <= XmlRepository.RootElementXml.Length)
                {
                    return new XElement(XmlRepository.RootElementXml);
                }

                return JsonConvert.DeserializeXNode(content, XmlRepository.RootElementName).Root;
        }
    }
}
