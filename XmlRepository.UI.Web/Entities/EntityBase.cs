using System;

namespace XmlRepository.UI.Web.Entities
{
    public class EntityBase
    {
        public Guid Id
        {
            get;
            set;
        }

        public DateTime DateCreated
        {
            get;
            set;
        }
    }
}