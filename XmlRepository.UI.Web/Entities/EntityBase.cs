using System;

namespace TodoWebApp.Entities
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