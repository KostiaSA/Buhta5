using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public class SchemaFolderDesignerModel : SchemaObjectEditModel<SchemaFolder>
    {

        public SchemaFolder Folder { get { return (SchemaFolder)EditedObject; } }

        public SchemaFolderDesignerModel(Controller controller, BaseModel parentModel) : base(controller, parentModel) { }


        public override void StartEditing()
        {
            if (EditedObject != null)
            {
                EditedObject.StartEditing();
            }
        }
     
    }
}