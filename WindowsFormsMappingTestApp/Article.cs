using System;

namespace WindowsFormsMappingTestApp
{
    public class Article
    {
        public Article()
        {
            this.Id = Guid.NewGuid();
        }

        public Guid Id
        {
            get;
            set;
        }

        public string Title
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }
    }
}