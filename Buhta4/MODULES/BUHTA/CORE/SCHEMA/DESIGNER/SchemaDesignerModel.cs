using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.ObjectModel;

namespace Buhta
{
    public class SchemaDesignerModel : BaseModel
    {
        public string ActiveRowId;

        public string TestProp1 { get; set; }

        public ObservableCollection<string> SelectedRows { get; set; }


        public SchemaDesignerModel(Controller controller, BaseModel parentModel) : base(controller,parentModel) {
            SelectedRows = new ObservableCollection<string>();
            SelectedRows.CollectionChanged += SelectedRows_CollectionChanged;
        }

        private void SelectedRows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            TestProp1 = "выбрано "+SelectedRows.Count;
        }

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
      ,'~/MODULES/BUHTA/CORE/Content/icon/'+RootClass+'_16.png' AS [__TreeGridIcon__]
      ,'~/MODULES/BUHTA/CORE/Content/icon/'+RootClass+'_16.png' AS [__Icon__]
  FROM [SchemaObject]
");
                    SelectedRows.Add("84ff8c4c-2c17-4af8-a2a1-02c958bd75bf");

                    var objs = db.ExecuteDataTable();
                    return objs.AsDataView();
                }
            }
        }

        public void OnRowDoubleClick(dynamic args)
        {
            openSchemaObjectDesigner(ActiveRowId);
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
            foreach (var win in AppServer.CurrentAppNavBarModel.ChromeWindows.Values)
            {
                if (win.ModelName == typeof(SchemaTableDesignerModel).FullName && win.RecordId== schemaObjectID)
                {
                    win.SetFocused();
                    return;
                }
            }

            var action = new OpenChildWindowAction();
            action.Url = "/Buhta/SchemaTableDesigner?ID=" + schemaObjectID; 
            ExecuteJavaScript(action.GetJsCode());
        }

    }
}