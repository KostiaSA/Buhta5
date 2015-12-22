using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class SchemaObject_cache
    {
        public Schema Schema;
        public Guid ID;
        public string Name;
        public Guid? ParentObjectID;
        public DateTime CreateDate;
        public Guid? CreateUserID;
        public DateTime? ChangeDate;
        public Guid? ChangeUserID;
        public DateTime? LockDateTime;
        public Guid? LockedByUserID;
        public int Position;
        public string ScriptCode;
        public string JSON;
        public string RootClass;
        public string RootType;

        private SchemaObject sampleObject;
        public SchemaObject SampleObject
        {
            get
            {
                if (sampleObject == null)
                {
                    sampleObject = Schema.GetObject<SchemaObject>(ID);
                }
                return sampleObject;
            }
        }
    }

    public class Schema
    {
        //public MongoServer MongoServer;
        //public MongoDatabase MongoDB;
        public SchemaDatabase SqlDB;
        //public MongoCollection SchemaObjectsCollection;
        //public MongoCollection ConfigObjectsCollection;
        //public MongoCollection SqlDatabasesCollection;

        public string SchemaSqlUrl { get; set; }
        public int? SchemaSqlPort { get; set; }
        public string SchemaSqlDatabase { get; set; }
        public string SchemaSqlLogin { get; set; }
        public string SchemaSqlPassword { get; set; }
        public bool SchemaIsTest { get; set; }

        public delegate void AfterSaveSchemaObjectEventHandler(SchemaObject schemaObject);

        public event AfterSaveSchemaObjectEventHandler AfterSaveSchemaObject; 

        Dictionary<Guid, SchemaObject_cache> objects_cache;
        public Dictionary<Guid, SchemaObject_cache> Objects_cache
        {
            get
            {
                if (objects_cache == null || objects_cache.Count == 0)
                {
                    using (var db = GetMetadataDbManager())
                    {
                        objects_cache = db.SetCommand("SELECT * FROM SchemaObject").ExecuteDictionary<Guid, SchemaObject_cache>("ID");
                        foreach (var cache in objects_cache.Values)
                            cache.Schema = this;
                    }
                }
                return objects_cache;

            }
        }

        //public ImageCollection Images16 = new ImageCollection();

        public void ReloadObjectCache(Guid ID)
        {
            Objects_cache.Remove(ID);
            GetObject<SchemaObject>(ID);
        }

        public T GetSampleObject<T>(Guid? ID) where T : SchemaObject
        {
            if (ID == null)
                return null;
            var obj = App.Schema.Objects_cache[(Guid)ID].SampleObject;
            if (obj is T)
                return obj as T;
            else
                return null;
        }

        public List<T> GetSampleObjects<T>() where T : SchemaObject
        {
            var list = new List<T>();
            foreach (var cache_obj in App.Schema.Objects_cache.Values)
            {
                if (cache_obj.SampleObject is T)
                    list.Add(cache_obj.SampleObject as T);
            }
            return list;
        }


        public T GetObject<T>(Guid ID) where T : SchemaObject
        {
            foreach (var helperTable in HelperTables)
                if (helperTable.ID==ID)
                    return helperTable as T;

            if (!Objects_cache.Keys.Contains(ID))
            {
                using (var db = GetMetadataDbManager())
                {
                    var obj_cache_to_load = db.SetCommand("SELECT * FROM SchemaObject WHERE ID=" + ID.AsSQL()).ExecuteObject<SchemaObject_cache>();
                    if (obj_cache_to_load == null)
                        return null;
                    obj_cache_to_load.Schema = this;
                    Objects_cache.Add(ID, obj_cache_to_load);
                }
            }
            var obj_cache = Objects_cache[ID];

            var jsonSettings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.Objects,
                NullValueHandling = NullValueHandling.Ignore
            };

            //obj_cache.JSON.ToString().SaveToFile(@"c:\$\q.txt");
            T obj = JsonConvert.DeserializeObject<T>(obj_cache.JSON.Replace(@", Buhta""", @", BuhtaCore"""), jsonSettings);
            obj.EndInit();
            return obj;
            //return SchemaObjectsCollection.FindOneByIdAs<T>(ID);
        }

        public Schema(string serverUrl, int? serverPort, string database, string login, string password)
        {
            SchemaSqlUrl = serverUrl;
            SchemaSqlPort = serverPort;
            SchemaSqlDatabase = database;
            SchemaSqlLogin = login;
            SchemaSqlPassword = password;

            //            var connectionString = "mongodb://" + mongoServerUrl;
            //            if (mongoServerPort != null)
            //                connectionString += ":" + mongoServerPort;

            //            var client = new MongoClient(connectionString);
            //            MongoServer = client.GetServer();
            //            MongoDB = MongoServer.GetDatabase(mongoDatabase);

            //            SchemaObjectsCollection = MongoDB.GetCollection<SchemaObject>("SchemaObject");
            //            ConfigObjectsCollection = MongoDB.GetCollection("SchemaObject");
            //            SqlDatabasesCollection = MongoDB.GetCollection<SchemaSqlDatabase>("SchemaSqlDatabase");


            //            if (SqlDB == null)
            //                throw new Exception("В конфигурации '" + mongoDatabase + "' нет настроенных SQL-баз данных.");

        }

        public string ConnectionString
        {
            get
            {
                //Debug.Assert(!string.IsNullOrWhiteSpace(SqlServer), "Не заполнен Server");
                //Debug.Assert(!string.IsNullOrWhiteSpace(SqlDatabase), "Не заполнен Database");
                //Debug.Assert(!string.IsNullOrWhiteSpace(SqlLogin), "Не заполнен Login");
                //Debug.Assert(SqlPassword != null, "Не заполнен Password?");
                var sb = new StringBuilder();
                if (SchemaSqlPort != null && SchemaSqlPort != 1433)
                    sb.Append("data source=" + SchemaSqlUrl + "," + SchemaSqlPort + ";");
                else
                    sb.Append("data source=" + SchemaSqlUrl + ";");
                sb.Append("initial catalog=" + SchemaSqlDatabase + ";");
                sb.Append("user id=" + SchemaSqlLogin + ";");
                sb.Append(@"password=""" + SchemaSqlPassword + @""";");
                sb.Append("max pool size=1000; Connection Timeout=2");
                return sb.ToString();
            }
        }

        public DbManager GetMetadataDbManager()
        {
            SqlDataProvider dataProvider = new SqlDataProvider();
            var db = new DbManager(dataProvider, ConnectionString);
            return db;
        }

        public void DeleteObject(SchemaObject objectToDelete)
        {
            var sql = new StringBuilder();
            sql.AppendLine(@"BEGIN TRAN");

            sql.AppendLine(@"
INSERT [dbo].[SchemaObjectHistory](
  [ID],[ParentObjectID],[Name],[JSON],
  [RootClass],[RootType],[CreateDateTime],[UpdateDateTime],
  [CreateUser],[UpdateUser])
SELECT 
  [ID],[ParentObjectID],[Name],[JSON],
  [RootClass],[RootType],[CreateDateTime],[UpdateDateTime],
  [CreateUser],[UpdateUser]
  FROM [dbo].[SchemaObject]
WHERE ID=" + objectToDelete.ID.AsSQL());


            sql.AppendLine(@"DELETE FROM SchemaObject WHERE ID=" + objectToDelete.ID.AsSQL());

            sql.AppendLine(@"COMMIT");

            using (var db = GetMetadataDbManager())
            {
                db.SetCommand(sql.ToString()).ExecuteNonQuery();
            }

            Objects_cache.Remove(objectToDelete.ID);
        }

        public void SaveObject(SchemaObject objectToSave)
        {

            if (objectToSave.CreateDate.Year <= 1)
                objectToSave.CreateDate = DateTime.Now;

            var sql = new StringBuilder();
            sql.AppendLine(@"BEGIN TRAN");

            sql.AppendLine(@"
INSERT [dbo].[SchemaObjectHistory](
  [ID],[ParentObjectID],[Name],[Description],[JSON],
  [RootClass],[RootType],[CreateDateTime],[UpdateDateTime],
  [CreateUser],[UpdateUser])
SELECT 
  [ID],[ParentObjectID],[Name],[Description],[JSON],
  [RootClass],[RootType],[CreateDateTime],[UpdateDateTime],
  [CreateUser],[UpdateUser]
  FROM [dbo].[SchemaObject]
WHERE ID=" + objectToSave.ID.AsSQL());


            sql.AppendLine(@"IF NOT EXISTS(SELECT ID FROM SchemaObject WHERE ID=" + objectToSave.ID.AsSQL() + @")");
            sql.AppendLine(@"  INSERT SchemaObject(ID) VALUES(" + objectToSave.ID.AsSQL() + @")");

            sql.AppendLine(@"UPDATE SchemaObject SET");
            if (objectToSave.Name != null)
                sql.AppendLine(@"  Name=" + objectToSave.Name.AsSQL() + @",");

            if (objectToSave.Description != null)
                sql.AppendLine(@"  Description=" + objectToSave.Description.AsSQL() + @",");

            sql.AppendLine(@"  RootClass=" + objectToSave.GetType().Name.AsSQL() + @",");
            sql.AppendLine(@"  RootType=" + objectToSave.GetTypeDisplay.AsSQL() + @",");

            if (objectToSave.ParentObjectID != null)
                sql.AppendLine(@"  ParentObjectID=" + objectToSave.ParentObjectID.AsSQL() + @",");
            else
                sql.AppendLine(@"  ParentObjectID=NULL,");

            if (objectToSave.CreateDate.Year > 1)
                sql.AppendLine(@"  CreateDateTime=" + objectToSave.CreateDate.AsSQL() + @",");
            else
                sql.AppendLine(@"  CreateDateTime=GetDate(),");

            //if (objectToSave.ChangeDate != null)
            //sql.AppendLine(@"  UpdateDateTime=" + objectToSave.ChangeDate.AsSQL() + @",");
            //else
            sql.AppendLine(@"  UpdateDateTime=GetDate(),");

            if (objectToSave.CreateUserID != null)
                sql.AppendLine(@"  CreateUser=" + objectToSave.CreateUserID.AsSQL() + @",");
            else
                sql.AppendLine(@"  CreateUser=NULL,");

            if (objectToSave.ChangeUserID != null)
                sql.AppendLine(@"  UpdateUser=" + objectToSave.ChangeUserID.AsSQL() + @",");
            else
                sql.AppendLine(@"  UpdateUser=NULL,");



            sql.AppendLine(@"  JSON=" + objectToSave.GetJsonText().AsSQL() + @" ");

            sql.AppendLine(@"WHERE ID=" + objectToSave.ID.AsSQL());

            sql.AppendLine(@"COMMIT");

            using (var db = GetMetadataDbManager())
            {
                db.SetCommand(sql.ToString()).ExecuteNonQuery();
            }

            ReloadObjectCache(objectToSave.ID);

            if (AfterSaveSchemaObject!=null)
            {
                AfterSaveSchemaObject(objectToSave);
            }

        }


        public List<T> GetObjects<T>() where T : SchemaObject
        {
            List<T> retList = new List<T>();
            foreach (var obj_cache in Objects_cache.Values)
            {
                if (obj_cache.RootClass == typeof(T).Name)
                {
                    retList.Add(GetObject<T>(obj_cache.ID));
                }
            }
            return retList;
        }

        public List<T> GetChildObjects<T>(Guid parentObjectID) where T : SchemaObject
        {
            List<T> retList = new List<T>();
            foreach (var obj_cache in Objects_cache.Values)
            {
                if (obj_cache.RootType == typeof(T).Name && obj_cache.ParentObjectID == parentObjectID)
                {
                    retList.Add(GetObject<T>(obj_cache.ID));
                }
            }
            return retList;
        }

        public List<SchemaObject> GetChildObjects(Guid? parentObjectID)
        {
            List<SchemaObject> retList = new List<SchemaObject>();
            foreach (var obj_cache in Objects_cache.Values)
            {
                if (obj_cache.ParentObjectID == parentObjectID)
                {
                    retList.Add(GetObject<SchemaObject>(obj_cache.ID));
                }
            }
            return retList;
        }

        public List<SchemaObject> GetSampleChildObjects(Guid? parentObjectID)
        {
            List<SchemaObject> retList = new List<SchemaObject>();
            foreach (var obj_cache in Objects_cache.Values)
            {
                if (obj_cache.ParentObjectID == parentObjectID)
                {
                    retList.Add(GetSampleObject<SchemaObject>(obj_cache.ID));
                }
            }
            return retList.OrderBy<SchemaObject, int>(obj => obj.Position).ToList<SchemaObject>();
        }

        public string GetObjectName(Guid? objectID)
        {
            if (objectID != null && Objects_cache.Keys.Contains((Guid)objectID))
                return Objects_cache[(Guid)objectID].Name;
            else
                return "";
        }

        public string GetObjectTypeAndName(Guid? objectID)
        {
            if (objectID != null && Objects_cache.Keys.Contains((Guid)objectID))
                return Objects_cache[(Guid)objectID].RootType + ": " + Objects_cache[(Guid)objectID].Name;
            else
                return "";
        }

        public T GetObjectByName<T>(string name) where T : SchemaObject
        {

            Guid obj_ID = (from obj_cache in Objects_cache.Values
                           where obj_cache.RootClass == typeof(T).Name && obj_cache.Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                           select obj_cache.ID).ToList().FirstOrDefault();
            if (obj_ID == Guid.Empty)
                return null;
            else
                return GetObject<T>(obj_ID);
        }

        public List<SchemaTable> GetAllDetailTablesSamples()
        {
            var retList = new List<SchemaTable>();
            foreach (var obj_cache in Objects_cache.Values)
            {
                if (obj_cache.SampleObject is SchemaTable && (obj_cache.SampleObject as SchemaTable).TableRoles.Contains(RoleConst.ВложеннаяТаблица))
                    retList.Add((obj_cache.SampleObject as SchemaTable));

            }
            return retList;
        }

        List<Base_HelperTable> helperTables;
        public List<Base_HelperTable> HelperTables
        {
            get
            {
                if (helperTables == null)
                {
                    helperTables = new List<Base_HelperTable>();
                    helperTables.Add(new SchemaTable_HelperTable(this));
                    helperTables.Add(new SchemaTableDetail_HelperTable(this));
                    helperTables.Add(new SchemaTableDetailOper_HelperTable(this));
                    helperTables.Add(new SchemaTableDetailOperProvodka_HelperTable(this));
                }
                return helperTables;
            }
        }

        public void SynchronizeSchemaHelperTables()
        {
            SqlDB.SynchronizeSchemaHelperTables(this);
        }

        public void CreateStoredProc__Удаление_проводок__()
        {
            var sql = new StringBuilder();
            sql.AppendLine("IF OBJECT_ID('dbo.__Удаление_проводки__') IS NULL");
            sql.AppendLine("  EXEC('CREATE PROCEDURE __Удаление_проводки__ AS SET NOCOUNT ON;')");

            using (var db = SqlDB.GetDbManager())
            {
                db.SetCommand(sql.ToString()).ExecuteNonQuery();
            }

            sql.Clear();

            sql.AppendLine("ALTER PROCEDURE __Удаление_проводки__(@provodkaRecordID uniqueidentifier, @registrID uniqueidentifier) AS");
            sql.AppendLine("BEGIN");
            sql.AppendLine("  SET NOCOUNT ON");

            foreach (var table in App.Schema.GetSampleObjects<SchemaTable>().Where(tbl=>tbl.TableRoles.Contains(RoleConst.Регистр)).OrderBy(tbl => tbl.Name))
            {
                sql.AppendLine("  IF @registrID="+table.ID.AsSQL()+" DELETE FROM "+table.Get4PartsTableName()+
                               " WHERE [" + table.GetColumnByRole(RoleConst.Таблица_Ключ).Name + "]=@provodkaRecordID ELSE");
            }
            sql.RemoveLastChar(6);
            sql.AppendLine();

            sql.AppendLine("END");

            using (var db = SqlDB.GetDbManager())
            {
                db.SetCommand(sql.ToString()).ExecuteNonQuery();
            }

        }

        public List<IPermissionSupportObject> GetAllPermissionObjects(Guid? parentObjectID, string parentFieldName)
        {
            List<IPermissionSupportObject> retList = new List<IPermissionSupportObject>();
            GetAllPermissionObjects(parentObjectID, parentFieldName, retList);
            return retList;
        }

        public void GetAllPermissionObjects(Guid? parentObjectID, string parentFieldName, List<IPermissionSupportObject> list)
        {
            foreach (var obj in GetSampleObjects<SchemaObject>())
            {
                if (obj is IPermissionSupportObject)
                {
                    var perm_obj = obj as IPermissionSupportObject;
                    if (parentObjectID == null && parentFieldName == null)
                    {
                        list.Add(perm_obj);
                        perm_obj.GetChildPermissionObjects(list);
                    }
                    else
                        if (parentObjectID == null && parentFieldName != null)
                        {
                            throw new Exception("internal error EEBD82CD");
                        }
                        else
                            if (parentObjectID != null && parentFieldName == null)
                            {
                                if (parentObjectID == perm_obj.GetID())
                                {
                                    list.Add(perm_obj);
                                    perm_obj.GetChildPermissionObjects(list);
                                }
                            }
                            else
                                if (parentObjectID != null && parentFieldName != null)
                                {
                                    if (parentObjectID == perm_obj.GetID() && parentFieldName == perm_obj.GetFieldName())
                                    {
                                        list.Add(perm_obj);
                                        perm_obj.GetChildPermissionObjects(list);
                                    }

                                }
                }
            }
        }
    }
}
