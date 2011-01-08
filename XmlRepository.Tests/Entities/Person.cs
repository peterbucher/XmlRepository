using System;

namespace XmlRepository.Tests.Entities
{
    public class Person
    {
        public Guid Id
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public string FirstName
        {
            get;
            set;
        }

        public DateTime Birthday
        {
            get;
            set;
        }
    }
}