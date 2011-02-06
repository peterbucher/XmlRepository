using System.Xml.Linq;
using XmlRepository.Contracts.DataSerializers;

namespace XmlRepository.DataSerializers
{
    /// <summary>
    /// Serializes or deserializes an XElement to the appropriate format.
    /// </summary>
    internal class XmlDataSerializer : IDataSerializer
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
                return rootElement.ToString();
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
                return XElement.Parse(content);
            }
        }
    }
}