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
    public class SelectSchemaObjectDialogModel : MessageDialogModel
    {
        public Type SchemaObjectType;
        public bool NeedSave;

        public Guid? Value { get; set; }
        public SelectSchemaObjectDialogModel(Controller controller, BaseModel parentModel, Guid? oldValue, Type schemaObjectType) : base(controller, parentModel)
        {
            SchemaObjectType = schemaObjectType;
            Value = oldValue;
            NeedSave = false;
        }

        public override void OkButtonClick(dynamic args)
        {
            base.OkButtonClick(null);
        }

        //public void CancelButtonClick(dynamic args)
        //{
        //    Modal.Close();
        //}

        public void RowSelect(dynamic args)
        {
            Guid id = Guid.Parse(args.rowId.Value);

            if (args.isSelected.Value)
            {
                Value = id;
            }
        }

    }
}
