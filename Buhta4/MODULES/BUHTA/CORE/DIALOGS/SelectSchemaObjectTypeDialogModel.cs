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
        public bool NeedSave;

        public string Value { get; set; }
        public SelectSchemaObjectTypeDialogModel(Controller controller, BaseModel parentModel, string oldValue) : base(controller, parentModel)
        {
            Value = oldValue;
            NeedSave = false;
        }


        public List<Object> ObjectTypesList
        {
            get
            {
                var list = new List<Object>();
                foreach (Lazy<SchemaObject> dt in App.Mef.SchemaObjects.OrderBy((t)=>t.Value.GetTypeDisplay))
                    list.Add(new { ID = dt.Value.GetType().FullName, Title = dt.Value.GetTypeDisplay, Icon= @"~/MODULES/BUHTA/CORE/Content/icon/" + dt.Value.GetType().Name + "_16.png" });

                var importTableFakeObject = new SchemaObject() { ID = Guid.Parse("E08D5D71-87CC-4D9C-8A5C-817F67ED5C1F") };
                list.Add(new { ID = "Импорт таблицы из базы данных", Title = "Импорт таблицы из базы данных", Icon = @"~/MODULES/BUHTA/CORE/Content/icon/import-table-from-sql_16.png" });

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
            Value = args.rowId.Value;
        }

    }
}
