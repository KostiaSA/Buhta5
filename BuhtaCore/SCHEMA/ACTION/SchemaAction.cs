using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Buhta
{
    [System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
    [JsonObject(IsReference = true)]
    public class SchemaAction : INotifyPropertyChanged
    {
        public virtual string GetDisplayName()
        {
            return "?Action";
        }

        [Browsable(false)]
        public event PropertyChangedEventHandler PropertyChanged;

        [Browsable(false)]
        public SchemaFormControl ParentControl;

        public virtual void firePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (ParentControl != null)
                ParentControl.firePropertyChanged("Action");
        }

        public override string ToString()
        {
            return GetDisplayName();
        }

        public virtual string GetFullName()
        {
            return GetDisplayName();
        }

        public virtual void Execute()
        {
            throw new Exception("Action.Execute() не определен.");
        }
    }

    //public class SchemaActionSelectorTypeConverter : TypeConverter
    //{
    //    private TypeConverter mTypeConverter;

    //    public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
    //    {
    //        if (mTypeConverter == null)
    //            mTypeConverter = TypeDescriptor.GetConverter(context.PropertyDescriptor.PropertyType);

    //        if (context != null && destinationType == typeof(string))
    //        {

    //            return (value as SchemaAction).GetDisplayName();
    //        }
    //        return mTypeConverter.ConvertTo(context, culture, value, destinationType);
    //    }
    //}

    ////public class SchemaActionSelectorEditor : ObjectSelectorEditor
    ////{
    ////    protected override void FillTreeWithData(System.ComponentModel.Design.ObjectSelectorEditor.Selector theSel,
    ////      ITypeDescriptorContext theCtx, IServiceProvider theProvider)
    ////    {
    ////        base.FillTreeWithData(theSel, theCtx, theProvider);  //clear the selection

    ////        object parentButton = theCtx.Instance;

    ////        //foreach (Type tableType in mixUtil.GetAllSubclassTypes(typeof(mixTable)))
    ////        //{
    ////        //SelectorNode aNd = new SelectorNode(tableType.FullName, tableType);
    ////        //theSel.Nodes.Add(aNd);
    ////        //}

    ////        foreach (Lazy<SchemaAction> dt in App.Mef.SchemaActions)
    ////        {
    ////            var actionInstance = Activator.CreateInstance(dt.Value.GetType());

    ////            if (parentButton is SchemaFormControl)
    ////                (actionInstance as SchemaAction).ParentControl = (parentButton as SchemaFormControl);
    ////            else
    ////                throw new Exception("Action parent not SchemaFormControl");



    ////            SelectorNode aNd = new SelectorNode(dt.Value.GetDisplayName(), actionInstance);
    ////            theSel.Nodes.Add(aNd);
    ////        }


    ////    }

    ////}

}
