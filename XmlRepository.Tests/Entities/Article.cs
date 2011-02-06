using System;
using System.Linq;
using System.Text;

namespace XmlRepository.Tests.Entities
{
    ///<summary>
    ///</summary>
    public class Article
    {
        ///<summary>
        ///</summary>
        public Article()
        {
            this.Id = Guid.NewGuid();
        }

        ///<summary>
        ///</summary>
        public Guid Id
        {
            get;
            set;
        }

        ///<summary>
        ///</summary>
        public Guid ArticleCategoryId
        {
            get;
            set;
        }

        ///<summary>
        ///</summary>
        public string Title
        {
            get;
            set;
        }
    }
}