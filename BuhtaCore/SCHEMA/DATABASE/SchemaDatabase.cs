using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using Microsoft.SqlServer.Management.Smo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{

    [Export(typeof(SchemaObject))]
    public class SchemaDatabase : SchemaObject
    {
        public string SqlUrl { get; set; }
        public int? SqlPort { get; set; }
        public string SqlDatabase { get; set; }
        public string SqlLogin { get; set; }
        public string SqlPassword { get; set; }
        public bool IsTest { get; set; }

        public override BaseEdit_Page GetEditForm_page()
        {
            return new BaseEdit_Page("SchemaDatabaseDesigner_page() { EditedRecord = this };");
        }

        public override string GetTypeDisplay
        {
            get
            {
                return "База данных";
            }
        }

        [JsonIgnore]
        public string ConnectionString
        {
            get
            {
                //Debug.Assert(!string.IsNullOrWhiteSpace(SqlServer), "Не заполнен Server");
                //Debug.Assert(!string.IsNullOrWhiteSpace(SqlDatabase), "Не заполнен Database");
                //Debug.Assert(!string.IsNullOrWhiteSpace(SqlLogin), "Не заполнен Login");
                //Debug.Assert(SqlPassword != null, "Не заполнен Password?");
                var sb = new StringBuilder();
                if (SqlPort != null && SqlPort != 1433)
                    sb.Append("data source=" + SqlUrl + "," + SqlPort + ";");
                else
                    sb.Append("data source=" + SqlUrl + ";");
                sb.Append("initial catalog=" + SqlDatabase + ";");
                sb.Append("user id=" + SqlLogin + ";");
                sb.Append(@"password=""" + SqlPassword + @""";");
                sb.Append("max pool size=1000; Connection Timeout=2");
                return sb.ToString();
            }
        }
        public DbManager GetDbManager()
        {
            SqlDataProvider dataProvider = new SqlDataProvider();
            var db = new DbManager(dataProvider, ConnectionString);
            return db;
        }


        public Server GetSmoServer()
        {
            Server srv;
            if (SqlPort != null && SqlPort != 1433)
                srv = new Server(SqlUrl + "," + SqlPort);
            else
                srv = new Server(SqlUrl);
            srv.ConnectionContext.LoginSecure = false;   // set to true for Windows Authentication
            srv.ConnectionContext.Login = SqlLogin;
            srv.ConnectionContext.Password = SqlPassword;
            return srv;
        }

        public Database GetSmoDatabase()
        {
            return GetSmoServer().Databases[SqlDatabase];
        }

        public Table GetSmoTable(SchemaTable schemaTable)
        {
            return GetSmoDatabase().Tables[schemaTable.Name];
        }

        public void SynchronizeTable(SchemaTable schemaTable)
        {
            var smoTable = GetSmoTable(schemaTable);
            if (smoTable == null)
            {
                smoTable = new Table(GetSmoDatabase(), schemaTable.Name);
                foreach (var column in schemaTable.Columns)
                {
                    var newcol = column.GetSmoColumn(smoTable);
                    smoTable.Columns.Add(newcol);
                }
                smoTable.Create();


                // primary key
                if (!schemaTable.IsLogTable && !schemaTable.IsProvodkasTable)
                {
                    var pk = new Index(smoTable, "PK_" + schemaTable.Name);
                    var icol = new IndexedColumn(pk, schemaTable.GetPrimaryKeyColumn().Name, false);
                    pk.IndexedColumns.Add(icol);
                    pk.IndexKeyType = IndexKeyType.DriPrimaryKey;

                    pk.IsClustered = true;
                    pk.FillFactor = 50;

                    pk.Create();
                }
            }
            else
            {
                foreach (var schemaColumn in schemaTable.Columns)
                {
                    if (smoTable.Columns.Contains(schemaColumn.Name))
                    {
                        if ((schemaColumn.Table.GetPrimaryKeyColumn()==null || schemaColumn.Name != schemaColumn.Table.GetPrimaryKeyColumn().Name) && 
                            !(schemaColumn.DataType is TimestampDataType))
                        {
                            var smoColumn = smoTable.Columns[schemaColumn.Name];
                            var newDataType = schemaColumn.DataType.GetSmoDataType();
                            if (!smoColumn.DataType.Equals(newDataType))
                                smoColumn.DataType = newDataType;
                            if (smoColumn.Nullable != !schemaColumn.IsNotNullable)
                                smoColumn.Nullable = !schemaColumn.IsNotNullable;
                        }
                    }
                    else
                    {
                        var newcol = schemaColumn.GetSmoColumn(smoTable);
                        smoTable.Columns.Add(newcol);
                    }
                }
                smoTable.Alter();
            }

        }

        public override Bitmap GetImage()
        {
            return new Bitmap("global::Buhta.Properties.Resources.SchemaDatabase_16");
        }

        public void SynchronizeSchemaHelperTables(Schema schema)
        {
            foreach(var helperTable in schema.HelperTables)
            {
                SynchronizeTable(helperTable);

                using (var db = GetDbManager())
                {
                    db.SetCommand(helperTable.GetFillSql()).ExecuteNonQuery();
                }
            }
        }

    //    public void Synch

        void Test1()
        {
            //DbManager.DefaultConfiguration
            using (var db = GetDbManager())
            {
                db
                    .SetCommand("select * from ")
                    .ExecuteDataSet();
            }
        }
    }

}


