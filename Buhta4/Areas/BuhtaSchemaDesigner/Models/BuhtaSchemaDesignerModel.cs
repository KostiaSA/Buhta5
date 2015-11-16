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
  FROM [SchemaObject]
");

                    var objs = db.ExecuteDataTable();
                    return objs.AsDataView();
                }
            }
        }

        public void OnRowDoubleClick(string chromeWindowId, dynamic args)
        {
            var action = new OpenChildWindowAction();
            action.Url = "BuhtaSchemaDesigner/SchemaTableDesigner?ID=" + args.rowId; // Controller.Url.Action("SchemaTableDesigner", "BuhtaSchemaDesigner", new { ID = args.rowId });
            ExecuteJavaScript(chromeWindowId, action.GetJsCode());
            //var xx = 1;
            //var model = new SchemaTableColumnEditModel();
            //model.Column = Table.Columns[0];

            ////var xx = R.RenderViewToString(Controller, @"~\Areas\BuhtaCore\Views\TableColumnEditorWindow.cshtml", model); //-это работает


            //var win = CreateWindow(chromeWindowId, @"~\Areas\BuhtaCore\Views\TableColumnEditorWindow.cshtml", model);
            //win.OnClose_Bind = nameof(CloseColumnEditor);
            //win.Show();
            ////ShowWindow(@"~\Areas\BuhtaCore\Views\TableColumnEditorWindow.cshtml", model); //-это работает
        }

    }
}