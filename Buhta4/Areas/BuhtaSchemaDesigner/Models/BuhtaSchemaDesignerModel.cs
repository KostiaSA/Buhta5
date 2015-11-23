using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public class BuhtaSchemaDesignerModel : BaseModel
    {
        public string ActiveRowId;

        public BuhtaSchemaDesignerModel(Controller controller) : base(controller) { }

        public DataView SchemaObjectList
        {
            get
            {

                using (var db = App.Schema.GetMetadataDbManager())
                {
                    db.SetCommand(
@"
SELECT [ID]
      ,[ParentObjectID]
      ,[Name]
      ,[RootClass]
      ,[RootType]
      ,[CreateDateTime]
      ,[UpdateDateTime]
      ,[CreateUser]
      ,[UpdateUser]
      ,[LockedByUser]
      ,[LockDateTime]
      ,'Areas/BuhtaSchemaDesigner/Content/icon/'+RootClass+'_16.png' AS [__TreeGridIcon__]
      ,'Areas/BuhtaSchemaDesigner/Content/icon/'+RootClass+'_16.png' AS [__Icon__]
  FROM [SchemaObject]
");

                    var objs = db.ExecuteDataTable();
                    return objs.AsDataView();
                }
            }
        }

        public void OnRowDoubleClick(dynamic args)
        {
            openSchemaObjectDesigner(args.rowId.ToString());
            //var action = new OpenChildWindowAction();
            //action.Url = "BuhtaSchemaDesigner/SchemaTableDesigner?ID=" + args.rowId; 
            //ExecuteJavaScript(chromeWindowId, action.GetJsCode());
        }

        public void OnRowSelect(dynamic args)
        {
            var xx = 100;
            //openSchemaObjectDesigner(args.rowId.ToString());
            //var action = new OpenChildWindowAction();
            //action.Url = "BuhtaSchemaDesigner/SchemaTableDesigner?ID=" + args.rowId; 
            //ExecuteJavaScript(chromeWindowId, action.GetJsCode());
        }

        public void OnRowActivate(dynamic args)
        {
            ActiveRowId = args.rowId;
            //openSchemaObjectDesigner(args.rowId.ToString());
            //var action = new OpenChildWindowAction();
            //action.Url = "BuhtaSchemaDesigner/SchemaTableDesigner?ID=" + args.rowId; 
            //ExecuteJavaScript(chromeWindowId, action.GetJsCode());
        }

        public void OnChangeButtonClick(dynamic args)
        {
            if (ActiveRowId != null)
                openSchemaObjectDesigner(ActiveRowId);
        }

        private void openSchemaObjectDesigner(string schemaObjectID)
        {
            var action = new OpenChildWindowAction();
            action.Url = "BuhtaSchemaDesigner/SchemaTableDesigner?ID=" + schemaObjectID; 
            ExecuteJavaScript(action.GetJsCode());
        }

    }
}