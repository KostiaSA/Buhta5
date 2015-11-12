using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Buhta
{
    public static class App
    {
        ////public static MainForm MainForm;
        ////public static XtraTabControl MainTabControl;
        ////public static MenuNavigator_page NavigatorPage;
        ////public static XtraTabPage NavigatorTab;
        public static Schema Schema;
        ////public static Mef Mef;
        public static Guid UserID;

        public static Random Random;

        ////public static Dictionary<Guid, Type> RegisteredScripts = new Dictionary<Guid, Type>();

        ////public static Font DefaultFont = new Font("Tahoma", 9f);

        ////public const string RegistryPath = @"SOFTWARE\BuhtaNew";

        //static App()
        //{
        //    Random = new Random();

        //    UserID = Guid.Parse("10000001-28E0-4E14-B18C-E8185351E5C7");
        //    Mef = new Mef();

        //    var mefCatalog = new AggregateCatalog();
        //    mefCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));

        //    CompositionContainer mefContainer = new CompositionContainer(mefCatalog);
        //    mefContainer.ComposeParts(Mef);

        //    SchemaBaseRole.LoadRoles();

        //    Schema = new Schema("ps-web", null, "BuhtaSchema", "sa1", "sonyk");    // работа

        //    var sqlDatabases = Schema.GetObjects<SchemaDatabase>();

        //    if (sqlDatabases.Count == 0)
        //        throw new Exception("Нет баз данных SQL в схеме '" + Schema.SchemaSqlDatabase + "'");
        //    else
        //        Schema.SqlDB = sqlDatabases[0];

        //    //Schema = new Schema("5.19.239.191", 64123, "Buhta", "kostia", "sonyk");  // дома

        //    //var connectionString = "mongodb://localhost";
        //    //var client = new MongoClient(connectionString);
        //    //MongoServer = client.GetServer();
        //    //MongoDB = MongoServer.GetDatabase("Buhta"); // "test" is the name of the database

        //    //MongoSchemaCollection = MongoDB.GetCollection<SchemaObject>("SchemaObject");

        //}

        public static bool IsBuhtaDeveloperMode { get { return Schema.SqlDB.SqlUrl.ToLower() == "ps-web"; } }

        public static void Start()
        {

            ////JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            ////{
            ////    Converters = new List<JsonConverter> { new JsonImageConverter() }
            ////};

            ////Application.EnableVisualStyles();
            ////Application.SetCompatibleTextRenderingDefault(false);

            ////DevExpress.Skins.SkinManager.EnableFormSkins();
            ////DevExpress.UserSkins.BonusSkins.Register();
            ////UserLookAndFeel.Default.SetSkinStyle("DevExpress Style");

            ////if (!System.Diagnostics.Debugger.IsAttached)
            ////{
            ////    Application.ThreadException += new ThreadExceptionEventHandler(Application_ThreadException);
            ////    AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            ////}

            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru");
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru");
            System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentUICulture.NumberFormat.CurrencyDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentUICulture.NumberFormat.NumberDecimalSeparator = ".";

            Random = new Random();

            UserID = Guid.Parse("10000001-28E0-4E14-B18C-E8185351E5C7");
            ////Mef = new Mef();

            ////var mefCatalog = new AggregateCatalog();
            ////mefCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Program).Assembly));

            ////mefCatalog.Catalogs.Add(new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory,"company.dll"));
            ////mefCatalog.Catalogs.Add(new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory, "nopCommerce.dll"));

            ////CompositionContainer mefContainer = new CompositionContainer(mefCatalog);
            ////mefContainer.ComposeParts(Mef);


            Schema = new Schema("ps-web", null, "BuhtaSchema", "sa1", "sonyk");    // работа
            //Schema = new Schema(@"black\black2012", null, "KutinSchema", "sa1", "sonyk");    // работа

            var sqlDatabases = Schema.GetObjects<SchemaDatabase>();

            if (sqlDatabases.Count == 0)
                throw new Exception("Нет баз данных SQL в схеме '" + Schema.SchemaSqlDatabase + "'");
            else
                Schema.SqlDB = sqlDatabases[0];

            SchemaBaseRole.LoadRoles();

            SchemaVirtualTable.LoadVirtualTables();

            //Schema = new Schema("5.19.239.191", 64123, "Buhta", "kostia", "sonyk");  // дома


        }

        ////static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        ////{
        ////    throw new Exception(e.Exception.GetType().FullName + "\n" + e.Exception.GetFullMessage(), "Ошибка");
        ////}

        ////static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        ////{
        ////    throw new Exception(e.ExceptionObject.GetType().FullName + "\n" + (e.ExceptionObject as Exception).GetFullMessage(), "Ошибка");
        ////}

    }
}
