using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Buhta
{
    public class SelectSchemaRolesDialogModel: BaseModel
    {
        public bool NeedSave;
        //public T EditedObject { get; set; }

        public SelectSchemaRolesDialogModel(Controller controller) : base(controller) { }


        public bool OkButtonDisabled { get { return !NeedSave; } }

        public void OkButtonClick( dynamic args)
        {
            //EditedObject.Save;
        }

    }
}
