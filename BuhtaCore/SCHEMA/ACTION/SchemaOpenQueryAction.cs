using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Buhta
{
    [Export(typeof(SchemaAction))]
    public class SchemaOpenQueryAction : SchemaAction
    {
        Guid? queryID;
        [Editor(typeof(SchemaFormDataGridQuerySelectorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(SchemaFormDataGridQuerySelectorTypeConverter))]
        [DisplayName("Запрос")]
        public Guid? QueryID
        {
            get { return queryID; }
            set
            {
                queryID = value;
                firePropertyChanged("QueryID");
            }
        }

        public override string GetDisplayName()
        {
            return "Открыть запрос";
        }

        public override string GetFullName()
        {
            if (queryID != null)
                return GetDisplayName() + ":" + App.Schema.GetObjectName(queryID);
            else
                return GetDisplayName() + ": ?";
        }

        public override void Execute()
        {
            if (queryID != null)
            {
                var form = new SchemaForm();
                form.ID = Guid.NewGuid();
                form.Name = App.Schema.GetObjectName(queryID);
                form.RootControl = new SchemaFormPage() { ParentForm = form };

                var middlePanel = new SchemaFormPageMiddlePanel();
                middlePanel.ParentControl = form.RootControl;
                form.RootControl.Controls.Add(middlePanel);

                var grid = new SchemaFormDataGrid();
                grid.ParentControl = middlePanel;
                grid.QueryID = queryID;
                middlePanel.Controls.Add(grid);

                form.OpenInMainFormTab("запрос: " + App.Schema.GetObjectName(queryID));
            }
            else
                MessageBox.Show("Не заполнен Action.QueryID");

            //MessageBox.Show("Щзут йгукн");
        }

    }
}
