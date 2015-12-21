using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Buhta
{
    public class SelectSchemaObjectTypeDialogModel : MessageDialogModel
    {
        public Type SchemaObjectType;
        public bool NeedSave;

        public Guid? Value { get; set; }
        public SelectSchemaObjectTypeDialogModel(Controller controller, BaseModel parentModel, Guid? oldValue, Type schemaObjectType) : base(controller, parentModel)
        {
            SchemaObjectType = schemaObjectType;
            Value = oldValue;
            NeedSave = false;
        }


        public List<Object> ObjectTypesList
        {
            get
            {
                var list = new List<Object>();
                foreach (Lazy<SchemaObject> dt in App.Mef.SchemaObjects)
                    list.Add(new { ID = dt.Value, Title = dt.Value.GetTypeDisplay });
                return list;
            }
        }

        public override void OkButtonClick(dynamic args)
        {
            base.OkButtonClick(null);
        }

        //public void CancelButtonClick(dynamic args)
        //{
        //    Modal.Close();
        //}

        public void RowActivate(dynamic args)
        {
            Value = Guid.Parse(args.rowId.Value);
        }

    }
}
