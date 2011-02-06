namespace XmlRepository.Contracts.Mapping
{
    ///<summary>
    /// Contains all possible xml places to map a property value to.
    ///</summary>
    public enum XmlMappingType
    {
        ///<summary>
        /// Map the property value to xml subelement.
        ///</summary>
        Element,

        ///<summary>
        /// Map the property value to xml attribute.
        ///</summary>
        Attribute,

        ///<summary>
        /// Map the property value to xml content.
        ///</summary>
        Content
    }
}