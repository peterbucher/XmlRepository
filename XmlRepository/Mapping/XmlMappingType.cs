namespace XmlRepository.Mapping
{
    ///<summary>
    /// Contains all possible places to map property content on.
    ///</summary>
    public enum XmlMappingType
    {
        ///<summary>
        /// Map content to xml subelement.
        ///</summary>
        Element,

        ///<summary>
        /// Map content to xml attribute.
        ///</summary>
        Attribute,

        ///<summary>
        /// Map content to xml content.
        ///</summary>
        Content
    }
}