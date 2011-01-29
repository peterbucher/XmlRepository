using System;

namespace XmlRepository.Tests.Entities
{
    public class Geek
    {
        public Geek()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id
        {
            get;
            set;
        }

        public string SuperId
        {
            get;
            set;
        }

        public string Alias
        {
            get;
            set;
        }
    }
}