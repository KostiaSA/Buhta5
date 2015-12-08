using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class SchemaTableLinkedReport : INotifyPropertyChanged
    {
        private Guid? reportID;
        [DisplayName(" Шаблон"), Description(""), Category("")]
        ////[Editor(typeof(SchemaReportSelectorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(SchemaReportSelectorTypeConverter))]
        public Guid? ReportID
        {
            get { return reportID; }
            set { reportID = value; firePropertyChanged("ReportID"); }
        }

        private int copies = 1;
        [DisplayName("Копий"), Description("Количество печатных копий"), Category("")]
        public int Copies
        {
            get { return copies; }
            set { copies = value; firePropertyChanged("Copies"); }
        }

        private SchemaTable table;
        [Browsable(false)]
        public SchemaTable Table
        {
            get { return table; }
            set { table = value; firePropertyChanged("Table"); }
        }


        public SchemaTableLinkedReport()
        {
        }

        public virtual void Validate(ValidateErrorList error)
        {
            //if (ReportID == null)
            //{
            //    error.AppendLine("У привязанной печатной формы не указан шаблон.");
            //}

            //var report = App.Schema.GetSampleObject<SchemaReport>((Guid)ReportID);
            //if (ReportID == null)
            //{
            //    error.AppendLine("У привязанной печатной формы указан неверный шаблон.");
            //}

            //if (Copies < 0)
            //{
            //    error.AppendLine("У привязанной печатной формы указано отрицательное к-во копий");
            //}

            //if (Copies == 0)
            //{
            //    error.AppendLine("У привязанной печатной формы указано нулевое к-во копий");
            //}

        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void firePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (table != null)
                table.firePropertyChanged("LinkedReports");

        }


        ////public void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
        ////{
        ////    throw new NotImplementedException();
        ////}

        public class SchemaReportSelectorTypeConverter : TypeConverter
        {
            private TypeConverter mTypeConverter;

            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (mTypeConverter == null)
                    mTypeConverter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);

                if (context != null && destinationType == typeof(string))
                {
                    if (value == null)
                        value = "";
                    else
                        value = App.Schema.GetObjectTypeAndName((Guid?)value);
                }
                return mTypeConverter.ConvertTo(context, culture, value, destinationType);
            }
        }

        ////public class SchemaReportSelectorEditor : ObjectSelectorEditor
        ////{
        ////    public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        ////    {
        ////        return UITypeEditorEditStyle.Modal;
        ////    }

        ////    void dialog_OnFilterSchemaObject(Object schemaObject, out bool visible)
        ////    {
        ////        visible = schemaObject is SchemaReport;
        ////    }

        ////    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        ////    {
        ////        if (context != null && context.Instance != null && provider != null)
        ////        {
        ////            //SchemaQueryJoinColumn editedColumn = (SchemaQueryJoinColumn)context.Instance;

        ////            var dialog = new SchemaObjectSelect_dialog<ISchemaTreeListNode>();
        ////            dialog.IsIncludeRoleTables = true;
        ////            dialog.OnFilterSchemaObject += dialog_OnFilterSchemaObject;
        ////            dialog.LoadData();
        ////            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        ////            {
        ////                return (dialog.SelectedObject as ISchemaTreeListNode).ID;
        ////            }

        ////        }
        ////        return value;
        ////    }
        ////}

    }
}
