using System;
using System.Xml.Linq;

namespace XmlRepositorySpeedTest
{
    /// <summary>
    /// Xml-Repository für Artikel
    /// </summary>
    public class XmlArticleRepository
        : XmlRepositoryDomainBase<Article>
    {
        public XmlArticleRepository(string basePath)
            : base(basePath, "articles")
        {
        }

        #region IRepository<Article> Members

        protected override void InsertCore(XElement element, Article entity)
        {
            element.Add(
                new XElement("title", entity.Title),
                new XElement("description", entity.Description),
                new XElement("categoryid", entity.CategoryId.ToString()),
                new XElement("navigateurl", entity.NavigateUrl),
                new XElement("downloadurl", entity.DownloadUrl),
                new XElement("downloadname", entity.DownloadName),
                new XElement("count", entity.Count)
                );
        }

        protected override void UpdateCore(XElement element, Article entity)
        {
            element.Element("title").Value = entity.Title;
            element.Element("description").Value = entity.Description;
            element.Element("categoryid").Value = entity.CategoryId.ToString();
            element.Element("navigateurl").Value = entity.NavigateUrl;
            element.Element("downloadurl").Value = entity.DownloadUrl;
            element.Element("downloadname").Value = entity.DownloadName;
            element.Element("count").Value = entity.Count.ToString();
        }

        protected override Article CreateEntityFromElementCore(XElement element, Article entity)
        {
            entity.Title = element.Element("title").Value;
            entity.Description = element.Element("description").Value;
            entity.CategoryId = new Guid(element.Element("categoryid").Value);
            entity.NavigateUrl = element.Element("navigateurl").Value;
            entity.DownloadUrl = element.Element("downloadurl").Value;
            entity.DownloadName = element.Element("downloadname").Value;
            entity.Count = (int)element.Element("count");

            return entity;
        }

        #endregion
    }
}