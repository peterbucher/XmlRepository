using System;
using System.Collections.Generic;

namespace XmlRepository.Tests.Entities
{
    ///<summary>
    ///</summary>
    public class ArticleCategory
    {
        ///<summary>
        ///</summary>
        public ArticleCategory()
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
        public string Name
        {
            get;
            set;
        }

        ///<summary>
        ///</summary>
        public List<Article> Articles
        {
            get;
            set;
        }
    }
}