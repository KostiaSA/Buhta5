using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Buhta
{
    [Export(typeof(SchemaMenuBaseAction))]
    public class SchemaMenuOpenQueryAction : SchemaMenuBaseAction
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

        public override string Name
        {
            get
            {
                return "Открыть запрос";
            }
        }

        public override string GetFullName()
        {
            if (queryID != null)
                return Name + ":" + App.Schema.GetObjectName(queryID);
            else
                return Name + ": ?";
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

        public class SchemaFormDataGridQuerySelectorTypeConverter : TypeConverter
        {
            private TypeConverter mTypeConverter;

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (value == null)
                    return "";

                if (mTypeConverter == null)
                    mTypeConverter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);

                if (context != null && destinationType == typeof(string))
                {
                    var query = App.Schema.GetObject<SchemaQuery>((Guid)value);
                    return query == null ? "<null>" : query.Name;
                }
                return mTypeConverter.ConvertTo(context, culture, value, destinationType);
            }
        }

        public class SchemaFormDataGridQuerySelectorEditor : ObjectSelectorEditor
        {
            protected override void FillTreeWithData(System.ComponentModel.Design.ObjectSelectorEditor.Selector theSel,
              ITypeDescriptorContext theCtx, IServiceProvider theProvider)
            {
                base.FillTreeWithData(theSel, theCtx, theProvider);  //clear the selection

                //    jsqlTableColumn aCtl = (jsqlTableColumn)theCtx.Instance;

                //foreach (Type tableType in mixUtil.GetAllSubclassTypes(typeof(mixTable)))
                //{
                //SelectorNode aNd = new SelectorNode(tableType.FullName, tableType);
                //theSel.Nodes.Add(aNd);
                //}

                foreach (SchemaObject_cache query in App.Schema.Objects_cache.Values)
                {
                    if (query.RootClass == typeof(SchemaQuery).Name)
                    {
                        SelectorNode aNd = new SelectorNode(query.Name, query.ID);
                        theSel.Nodes.Add(aNd);
                    }
                }

            }

        }

    }
}
