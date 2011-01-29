using System;
using System.Collections.Generic;

namespace XmlRepository.Tests.Entities
{
    public class Person
    {
        public Person()
        {
            this.Id = Guid.NewGuid();
        }

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

        public Geek Geek
        {
            get;
            set;
        }

        public DateTime Birthday
        {
            get;
            set;
        }

        public List<Geek> Friends
        {
            get;
            set;
        }
    }
}