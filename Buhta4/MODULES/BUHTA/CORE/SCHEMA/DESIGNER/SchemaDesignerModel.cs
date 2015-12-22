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


        public SchemaDesignerModel(Controller controller, BaseModel parentModel) : base(controller, parentModel)
        {
            SelectedRows = new ObservableCollection<string>();
            SelectedRows.CollectionChanged += SelectedRows_CollectionChanged;
        }

        private void SelectedRows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            TestProp1 = "выбрано " + SelectedRows.Count;
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
      ,[Description]
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
                    //SelectedRows.Add("84ff8c4c-2c17-4af8-a2a1-02c958bd75bf");

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

        public void OnCreateNewObjectButtonClick(dynamic args)
        {
            Guid parentFolder;
            if (ActiveRowId == null)
            {
                ShowInfoMessageDialog("новый объект", "Сначала выберите папку или модуль для добавления");
                return;
            }
            parentFolder = Guid.Parse(ActiveRowId);
            var parentObject = App.Schema.GetSampleObject<SchemaObject>(parentFolder);
            if (!(parentObject is SchemaFolder || parentObject is SchemaModule))
            {
                ShowErrorMessageDialog("новый объект", "Добавлять объекты можно только в модуль или папку");
                return;
            }

            var dialogModel = new SelectSchemaObjectTypeDialogModel(Controller, this, null);
            dialogModel.OkEventMethod = (arg) =>
            {
                CreateNewObject(dialogModel.Value, parentFolder);
            };
            var modal = CreateModal(@"~/MODULES/BUHTA/CORE/DIALOGS/SelectSchemaObjectTypeDialogView.cshtml", dialogModel);
            modal.Show();
        }

        public void CreateNewObject(string objectTypeName, Guid parentFolder)
        {
            if (objectTypeName == "E08D5D71-87CC-4D9C-8A5C-817F67ED5C1F")
            {
                //    treeList.RefreshDataSource();
                //    var importDialog = new ImportSchemaTablesFromSqlDatabase_dialog();
                //    importDialog.parentObject = focusedObject;
                //    if (importDialog.ShowDialog() == DialogResult.OK)
                //    {
                //        treeList.RefreshDataSource();
                //    }
                //    return;
            }

            foreach (Lazy<SchemaObject> dt in App.Mef.SchemaObjects)
            {
                if (dt.Value.GetType().FullName == objectTypeName)
                {

                    var newObject = (SchemaObject)Activator.CreateInstance(dt.Value.GetType());
                    newObject.ID = Guid.NewGuid();
                    newObject.ParentObjectID = parentFolder;
                    newObject.PrepareNew();


                    // добавляем в cache
                    var obj_cache_to_load = new SchemaObject_cache();
                    obj_cache_to_load.Schema = App.Schema;
                    obj_cache_to_load.Name = newObject.Name;
                    obj_cache_to_load.JSON = newObject.GetJsonText();
                    App.Schema.Objects_cache.Add(newObject.ID, obj_cache_to_load);


                    openSchemaObjectDesigner(newObject.ID.ToString(), "add");

                    return;
                }
            }

        }

        private void openSchemaObjectDesigner(string schemaObjectID, string mode = "edit")
        {
            foreach (var win in AppServer.CurrentAppNavBarModel.ChromeWindows.Values)
            {
                if (win.ModelBindingId == typeof(SchemaTableDesignerModel).FullName && win.RecordId == schemaObjectID)
                {
                    win.SetFocused();
                    return;
                }
            }

            var obj = App.Schema.GetObject<SchemaObject>(Guid.Parse(schemaObjectID));

            var action = new OpenChildWindowAction();
            action.Url = obj.GetDesignerUrl()+ "?ID=" + schemaObjectID + "&mode=" + mode;
            ExecuteJavaScript(action.GetJsCode());
        }

    }
}