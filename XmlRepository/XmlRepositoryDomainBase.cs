using System;
using System.Xml.Linq;

namespace XmlRepositorySpeedTest
{
    /// <summary>
    /// Sample Implementation:
    /// <code>
    /// public class XmlUserRepository : XmlRepositoryBase<EntityType>
    /// 
    /// public XmlUserRepository(IAppContextService appContext)
    /// : base(appContext, "EntityFileName")
    /// 
    /// protected override void InsertCore(XElement element, EntityType entity)
    /// element.Add(New property data...
    /// 
    /// protected override void UpdateCore(XElement element, EntityType entity)
    /// element.Element("PropertyName").Value = entity.PropertyName;
    /// 
    /// protected override EntityType CreateEntityFromElementCore(XElement element, EntityType entity)
    /// entity.Name = element.Element("name").Value;
    /// ...
    ///  return entity;
    /// </code>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class XmlRepositoryDomainBase<T>
        : XmlRepositoryBase<T> where T : DomainBase, new()
    {
        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="XmlRepositoryDomainBase" /> type.
        /// Child classes have to set base path and filename for the entity.
        /// </summary>
        /// <param name="appContext">The <see cref="IAppContextService" /> which contains the base path.</param>
        /// <param name="fileName">The filename.</param>
        protected XmlRepositoryDomainBase(string basePath, string fileName)
            : base(basePath, fileName)
        {

        }

        #endregion

        #region Overrides

        /// <summary>
        /// Inserts an entity and returns the <see cref="XElement" /> instance for the child class.
        /// Add properties from the base entity to the <see cref="XElement" /> instance.
        /// 
        /// Do NOT override in child classes.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        protected override XElement InsertBaseCore(T entity)
        {
            return new XElement("entry",
                                   new XElement("id", entity.Id.ToString()),
                                   new XElement("online", entity.Online.ToString()),
                                   new XElement("datelastchange", entity.DateLastChange.ToString()),
                                   new XElement("datepublished", entity.DatePublished.ToString()));
        }

        /// <summary>
        /// Updates an entity.
        /// Set the properties like:
        /// <code>
        /// element.Element("name").Value = entity.Name;
        /// </code>
        /// 
        /// Do NOT override in child classes.
        /// </summary>
        /// <param name="element">The <see cref="XElement" /> instance.</param>
        /// <param name="entity">The entity for updating.</param>
        protected override void UpdateBaseCore(XElement element, T entity)
        {
            element.Element("online").Value = entity.Online.ToString();
            element.Element("datelastchange").Value = entity.DateLastChange.ToString();
            element.Element("datepublished").Value = entity.DatePublished.ToString();
        }

        /// <summary>
        /// Creates a new instance of the base entity type.
        /// Contains the mapping from the entity to the <see cref="XElement" />.
        /// Set the mapping like:
        /// <code>
        /// entity.Name = element.Element("name").Value;
        /// </code>
        /// and return the instance.
        /// 
        /// Do NOT override in child classes.
        /// </summary>
        /// <param name="element">The <see cref="XElement" /> containing all data.</param>
        /// <returns>A new entity instance with the given data from the 'element' parameter.</returns>
        protected override T CreateEntityFromElementBaseCore(XElement element)
        {
            return new T
            {
                Id = new Guid(element.Element("id").Value),
                Online = element.Element("online").Value.ToOrDefault<bool>(),
                DateLastChange =
                    element.Element("datelastchange").Value.ToOrDefault<DateTime>(),
                DatePublished =
                    element.Element("datepublished").Value.ToOrDefault<DateTime>()
            };
        }

        #endregion

        #region Abstract methods for child classes.

        /// <summary>
        /// Inserts an entity.
        /// Base classes properties are allready set.
        /// Add properties from the entity to the <see cref="XElement" /> instance.
        /// </summary>
        /// <param name="element">The <see cref="XElement" /> to save.</param>
        /// <param name="entity">The entity to insert.</param>
        protected abstract override void InsertCore(XElement element, T entity);

        /// <summary>
        /// Updates an entity.
        /// Base classes properties are allready set.
        /// Set the properties like:
        /// <code>
        /// element.Element("name").Value = entity.Name;
        /// </code>
        /// </summary>
        /// <param name="element">The <see cref="XElement" /> instance.</param>
        /// <param name="entity">The entity for updating.</param>
        protected abstract override void UpdateCore(XElement element, T entity);

        #endregion
    }
}