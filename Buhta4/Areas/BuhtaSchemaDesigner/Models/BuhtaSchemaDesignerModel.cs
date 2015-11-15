using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Buhta
{
    public class BuhtaSchemaDesignerModel : BaseModel
    {
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
  FROM [SchemaObject]
");

                    var objs = db.ExecuteDataTable();
                    return objs.AsDataView();
                }
            }
        }
    }
}