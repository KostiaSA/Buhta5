using BLToolkit.Data;
using BLToolkit.Data.DataProvider;
using Microsoft.SqlServer.Management.Smo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{

    public class SchemaExternalDatabase : SchemaObject
    {
        private string linkedDatabase;
        public string LinkedDatabase
        {
            get { return linkedDatabase; }
            set { linkedDatabase = value; firePropertyChanged("LinkedDatabase"); }
        }

        private string linkedServer;
        public string LinkedServer
        {
            get { return linkedServer; }
            set { linkedServer = value; firePropertyChanged("LinkedServer"); }
        }

        private string linkedSchema="dbo";
        public string LinkedSchema
        {
            get { return linkedSchema; }
            set { linkedSchema = value; firePropertyChanged("LinkedSchema"); }
        }

        public override string GetTypeDisplay
        {
            get
            {
                return "Внешняя БД";
            }
        }

        public override Bitmap GetImage()
        {
            return new Bitmap("global::Buhta.Properties.Resources.SchemaExternalDatabase_16");
        }
    }

}


