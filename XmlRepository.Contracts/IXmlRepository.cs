using System;
using System.Xml.Linq;

namespace XmlRepository.Contracts
{
    /// <summary>
    /// Represents an xml repository.
    /// </summary>
    public interface IXmlRepository : IDisposable
    {
        ///<summary>
        /// The repository key.
        ///</summary>
        RepositoryKey Key
        {
            get;
            set;
        }

        ///<summary>
        /// Gets the xml representation of the current inmemory status of this XmlRepository instance.
        ///</summary>
        ///<returns>The xml representation as <see cref="XElement" />.</returns>
        XElement GetXmlRepresentation();
    }
}