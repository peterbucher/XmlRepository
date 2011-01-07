using System;
using System.Linq;
using System.Web.UI;
using cherryflavored.net.ExtensionMethods.System;
using TodoWebApp.Entities;

namespace XmlRepository.UI.Web
{
    public partial class _Default : Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.btnSubmit.Click += btnSubmit_Click;
            this.btnClearContent.Click += btnClearContent_Click;
            this.Load += _Default_Load;
        }

        private void _Default_Load(object sender, EventArgs e)
        {
            using (var repository = XmlRepository<Todo>.Instance)
            {
                var currentId = Request.QueryString["id"].ToOrDefault<Guid>();
                if (currentId != Guid.Empty)
                {
                    this.btnSubmit.Text = "Ändern";
                }

                if (!this.IsPostBack && currentId != Guid.Empty)
                {
                    Todo todo = repository.Load(currentId);
                    txtTitle.Text = todo.Title;
                    txtText.Text = todo.Text;
                }

                rptTodos.DataSource = repository.LoadAll().OrderByDescending(t => t.DateCreated);
                rptTodos.DataBind();
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            using (var repository = XmlRepository<Todo>.Instance)
            {
                var currentId = Request.QueryString["id"].ToOrDefault<Guid>();
                var todo = (currentId == Guid.Empty) ? (new Todo { Id = Guid.NewGuid() }) : repository.Load(currentId);

                todo.Title = txtTitle.Text;
                todo.Text = txtText.Text;
                todo.DateCreated = DateTime.Now;

                repository.SaveOnSubmit(todo);

                Response.Redirect("~/Default.aspx");
            }
        }

        private void btnClearContent_Click(object sender, EventArgs e)
        {
            using (var repository = XmlRepository<Todo>.Instance)
            {
                repository.DeleteAllOnSubmit();
                this.Response.Redirect("~/Default.aspx");
            }
        }
    }
}