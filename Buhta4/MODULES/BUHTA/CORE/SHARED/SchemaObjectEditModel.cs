using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Buhta
{
    public class SchemaObjectEditModel<T> : BaseEditFormModel where T : SchemaObject
    {
        //public bool NeedSave;
        //public T EditedObject { get; set; }

        public SchemaObjectEditModel(Controller controller, BaseModel parentModel) : base(controller, parentModel) { }


        //public virtual string EditedObjectName { get { return EditedObject.Name; } }


        //public void InitEditor()
        //{
        //    NeedSave = false;
        //    EditedObject.PropertyChanged += EditedObject_PropertyChanged;
        //}

        //private void EditedObject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    NeedSave = true;
        //}

        //public void SaveButtonClick(dynamic args)
        //{
        //    App.Schema.SaveObject(EditedObject);
        //    NeedSave = false;
        //}

    }
}
