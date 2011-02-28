using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using XmlRepository;
using XmlRepository.Contracts;
using XmlRepository.Contracts.Mapping;
using XmlRepository.DataProviders;

namespace WindowsFormsMappingTestApp
{
    public partial class Form1 : Form
    {
        private Type _typeToMap;
        private IDictionary<Type, IList<IPropertyMapping>> _userMappings;

        private IXmlRepository<Article, Guid> _mappedRepository;

        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            this._userMappings = new Dictionary<Type, IList<IPropertyMapping>>();

            this._mappedRepository = XmlRepository
                .XmlRepository
                .Get(
                    RepositoryFor<Article>
                        .WithIdentity(p => p.Id)
                        .WithMappings(this._userMappings)
                        .WithDataProvider(new InMemoryDataProvider(this.txtDefault.Text)));

            this.LoadMappingList(true);
        }

        private void LoadMappingList()
        {
            this.LoadMappingList(false);
        }

        private void LoadMappingList(bool initial)
        {
            if (initial)
            {
                this._typeToMap = typeof(Article);
                var dataSource = this._typeToMap.GetProperties().Select(p => p.Name).ToArray();
                this.ddlPropertyName.DataSource = dataSource;

                this.ddlMappingType.DataSource = Enum.GetNames(typeof(XmlMappingType));

                this.LoadMappings();
            }
            else
            {
                this.LoadMappings(false);
            }
        }

        private void LoadMappings()
        {
            this.LoadMappings(false);
        }

        private void LoadMappings(bool initial)
        {
            this.listBoxMappings.DataSource = this._userMappings[this._typeToMap];
            this.listBoxMappings.DisplayMember = "Name";
        }

        private void txtDefault_TextChanged(object sender, EventArgs e)
        {
            var defaultRepository = XmlRepository.XmlRepository.Get(RepositoryFor<Article>
                                                                        .WithIdentity(a => a.Id)
                                                                        .WithDataProvider(
                                                                            new InMemoryDataProvider(
                                                                                this.txtDefault.Text)));

            string test = defaultRepository.GetXmlRepresentation().ToString();

            defaultRepository.DiscardChanges();

            var articleList = defaultRepository.LoadAll();

            this._mappedRepository.DataProvider = new InMemoryDataProvider();
            this._mappedRepository.SaveOnSubmit(articleList);

            this.txtMapped.Text = this._mappedRepository.GetXmlRepresentation().ToString();

            this._mappedRepository.DiscardChanges();
        }

        private void listBoxMappings_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.LoadDetail(((IPropertyMapping)listBoxMappings.SelectedValue));
        }

        private void LoadDetail(IPropertyMapping propertyMapping)
        {
            this.ddlPropertyName.SelectedItem = propertyMapping.Name;
            this.txtPropertyAlias.Text = propertyMapping.Alias;
            this.ddlMappingType.SelectedItem = propertyMapping.XmlMappingType.ToString();
        }

        private void SaveOrUpdateDetail()
        {
            if (listBoxMappings.SelectedValue == null)
            {
                return;
            }

            var mappingToUpdate = this._userMappings[this._typeToMap]
                .Where(mapping => mapping.Name == ((IPropertyMapping)listBoxMappings.SelectedValue).Name)
                .Single();

            var indexOfMappingToUpdate = this._userMappings[this._typeToMap].IndexOf(mappingToUpdate);

            mappingToUpdate.Alias = this.txtPropertyAlias.Text;

            mappingToUpdate.XmlMappingType = (XmlMappingType)Enum.Parse(
                typeof(XmlMappingType),
                this.ddlMappingType.SelectedItem.ToString());

            //this._mappings[this._typeToMap].RemoveAt(indexOfMappingToUpdate);
            //this._mappings[this._typeToMap].Insert(indexOfMappingToUpdate, mappingToUpdate);

            this.LoadMappings();
        }

        private void DeleteDetail()
        {
            var mappingToDelete = this._userMappings[this._typeToMap]
                .Where(mapping => mapping.Name == ((IPropertyMapping)listBoxMappings.SelectedValue).Name)
                .Single();

            this._userMappings[this._typeToMap].Remove(mappingToDelete);

            // Add default mappings if needed.
            XmlRepository.XmlRepository.AddDefaultPropertyMappingsFor(this._typeToMap, this._userMappings);
        }

        private void btnSaveUpdate_Click(object sender, EventArgs e)
        {
            this.SaveOrUpdateDetail();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            this.DeleteDetail();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.txtDefault_TextChanged(null, EventArgs.Empty);
        }

        private void ddlPropertyName_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SaveOrUpdateDetail();
            this.txtDefault_TextChanged(null, EventArgs.Empty);
        }

        private void txtPropertyAlias_TextChanged(object sender, EventArgs e)
        {
            this.SaveOrUpdateDetail();
            this.txtDefault_TextChanged(null, EventArgs.Empty);
        }

        private void ddlMappingType_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SaveOrUpdateDetail();
            this.txtDefault_TextChanged(null, EventArgs.Empty);
        }
    }
}