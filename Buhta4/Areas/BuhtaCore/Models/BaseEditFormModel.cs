using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Buhta
{

    public class BaseEditFormModel : BaseModel
    {
        public IBuhtaEditable EditedObject { get; set; }
        protected Object SavedEditedObject;

        public BaseEditFormModel(Controller controller, BaseModel parentModel) : base(controller, parentModel) { }

        public virtual bool GetNeedSave()
        {
            if (EditedObject != null)
                return EditedObject.GetNeedSave();
            else
                return false;
        }

        public virtual void SaveChanges()
        {
            if (EditedObject != null)
                EditedObject.SaveChanges();
        }

        public virtual void CancelChanges()
        {
            if (EditedObject != null)
                EditedObject.CancelChanges();
        }



        public virtual string GetEditedObjectName()
        {
            if (EditedObject != null)
                return EditedObject.GetEditedObjectName();
            else
                return "";
        }

        public virtual string GetSaveButtonText()
        {
            return "Сохранить";
        }

        public virtual bool GetSaveButtonDisabled()
        {
            return !GetNeedSave();
        }

        public virtual string GetCancelButtonText()
        {
            if (GetNeedSave())
                return "Отменить";
            else
                return "Закрыть";
        }


        public virtual void StartEditing()
        {
            if (EditedObject != null)
            {
                EditedObject.StartEditing();
                SavedEditedObject = EditedObject.DeepClone();
            }
        }

        public void SaveButtonClick(dynamic args)
        {
            SaveChanges();
        }

        public void CancelButtonClick(dynamic args)
        {
            CancelChanges();
        }

    }
}
