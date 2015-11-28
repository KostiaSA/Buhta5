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
    public class SelectSchemaRolesDialogModel : BaseModel
    {
        public bool NeedSave;
        //public T EditedObject { get; set; }

        public SelectSchemaRolesDialogModel(Controller controller) : base(controller)
        {
            SelectedRows = new ObservableCollection<Guid>();
            SelectedRows.CollectionChanged += SelectedRows_CollectionChanged;
        }


        //public bool OkButtonDisabled { get { return !NeedSave; } }

        public void OkButtonClick(dynamic args)
        {
            //EditedObject.Save;
        }

        public void CancelButtonClick(dynamic args)
        {
            //EditedObject.Save;
        }

        public ObservableCollection<Guid> SelectedRows { get; set; }


        private void SelectedRows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NeedSave = true;
            // TestProp1 = "выбрано " + SelectedRows.Count;
        }


        //        public DataView SchemaObjectList
        //        {
        //            get
        //            {

        //                using (var db = App.Schema.GetMetadataDbManager())
        //                {
        //                    db.SetCommand(
        //@"
        //SELECT [ID]
        //      ,[ParentObjectID]
        //      ,[Name]
        //      ,[RootClass]
        //      ,[RootType]
        //      ,[CreateDateTime]
        //      ,[UpdateDateTime]
        //      ,[CreateUser]
        //      ,[UpdateUser]
        //      ,[LockedByUser]
        //      ,[LockDateTime]
        //      ,'Areas/BuhtaSchemaDesigner/Content/icon/'+RootClass+'_16.png' AS [__TreeGridIcon__]
        //      ,'Areas/BuhtaSchemaDesigner/Content/icon/'+RootClass+'_16.png' AS [__Icon__]
        //  FROM [SchemaObject]
        //");
        //                    //SelectedRows.Add("84ff8c4c-2c17-4af8-a2a1-02c958bd75bf");

        //                    var objs = db.ExecuteDataTable();
        //                    return objs.AsDataView();
        //                }
        //            }
        //        }
    }
}
