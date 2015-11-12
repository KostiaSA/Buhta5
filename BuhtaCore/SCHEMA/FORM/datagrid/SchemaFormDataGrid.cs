using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;
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
    [Export(typeof(SchemaFormControl))]
    public class SchemaFormDataGrid : SchemaFormControl
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
                query_cached = null;
                firePropertyChanged("QueryID");
                if (NativeGridControl != null)
                {
                    // перезагрузка всей таблицы
                    //NativeDataGrid.FlowDirection = flowDirection;
                }
            }
        }

        SchemaQuery query_cached;
        public SchemaQuery GetQuery()
        {
            if (QueryID == null)
                return null;

            if (query_cached != null)
                return query_cached;

            query_cached = App.Schema.GetObject<SchemaQuery>((Guid)QueryID);

            return query_cached;
        }

        [JsonIgnore]
        public SchemaFormDataGridNativeControl NativeGridControl;

        public override void AddDisplayHtmlAttrs(StringBuilder sb)
        {
            base.AddDisplayHtmlAttrs(sb);
            if (GetQuery() != null)
                sb.Append(GetDisplayHtmlAttr("Query", GetQuery().Name));
        }

        public override void Render(Control parentControl)
        {
            NativeGridControl = new SchemaFormDataGridNativeControl();
            query_cached = null;
            NativeGridControl.Query = GetQuery();
            NativeGridControl.Dock = DockStyle.Fill;
            NativeGridControl.Parent = parentControl;
            parentControl.Controls.Add(NativeGridControl);


            NativeGridControl.LoadData();
            //foreach (SchemaFormControl schemaControl in Controls)
            //{
            //    schemaControl.Render((Control)NativeDataGrid);
            //}

        }

        public override bool IsContainer()
        {
            return false;
        }




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
