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
    [Export(typeof(SchemaAction))]
    public class SchemaOpenPageAction : SchemaAction
    {
        Guid? formID;
        [Editor(typeof(SchemaFormSelectorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(SchemaFormSelectorTypeConverter))]
        [DisplayName("Форма")]
        public Guid? FormID
        {
            get { return formID; }
            set
            {
                formID = value;
                firePropertyChanged("FormID");
            }
        }

        public override string GetDisplayName()
        {
            return "Открыть страницу";
        }

        public override string GetFullName()
        {
            if (formID != null)
                return GetDisplayName() + ":" + App.Schema.GetObjectName(formID);
            else
                return GetDisplayName() + ": ?";
        }

        public override void Execute()
        {
            if (formID != null)
            {
                var form = App.Schema.GetObject<SchemaForm>((Guid)formID);
                form.OpenInMainFormTab(App.Schema.GetObjectName(formID));
            }
            else
                MessageBox.Show("Не заполнен Action.FormID");
        }

    }

    public class SchemaFormSelectorTypeConverter : TypeConverter
    {
        private TypeConverter mTypeConverter;

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (mTypeConverter == null)
                mTypeConverter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);

            if (context != null && destinationType == typeof(string))
            {
                var form = App.Schema.GetObject<SchemaForm>((Guid)value);
                return form == null ? "<null>" : form.Name;
            }
            return mTypeConverter.ConvertTo(context, culture, value, destinationType);
        }

    }

    public class SchemaFormSelectorEditor : ObjectSelectorEditor
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

            foreach (SchemaObject_cache form in App.Schema.Objects_cache.Values)
            {
                if (form.RootClass == typeof(SchemaForm).Name)
                {
                    SelectorNode aNd = new SelectorNode(form.Name, form.ID);
                    theSel.Nodes.Add(aNd);
                }
            }

        }

    }

}
