using System.Xml.Linq;
using Newtonsoft.Json;
using XmlRepository.Contracts;

namespace XmlRepository.DataSerializers
{
    /// <summary>
    /// Serializes or deserializes an XElement to the appropriate format.
    /// </summary>
    public class JsonDataSerializer : IDataSerializer
    {
        /// <summary>
        /// Contains the lock object.
        /// </summary>
        private readonly object _lockObject = new object();

        /// <summary>
        /// Serializes the given root element.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns>The serialized string representation.</returns>
        public string Serialize(XElement rootElement)
        {
            lock (this._lockObject)
            {
                return JsonConvert.SerializeXNode(rootElement, Formatting.Indented);
            }
        }

        /// <summary>
        /// Deserializes the given string representation.
        /// </summary>
        /// <param name="content">The string representation.</param>
        /// <returns>An XElement.</returns>
        public XElement Deserialize(string content)
        {
            lock (this._lockObject)
            {
                if(content.Length <= 8)
                {
                    return new XElement("root");
                }

                return JsonConvert.DeserializeXNode(content).Root;
            }
        }
    }
}