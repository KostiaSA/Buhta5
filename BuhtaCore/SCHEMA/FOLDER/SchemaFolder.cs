using BLToolkit.DataAccess;
using BLToolkit.Mapping;
using csscript;
using CSScriptLibrary;
using DevExpress.XtraTreeList;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Buhta
{

    [Export(typeof(SchemaObject))]
    public class SchemaFolder : SchemaObject
    {
        public override BaseEdit_Page GetEditForm_page()
        {
            return new SchemaFolderDesigner_page() { EditedRecord = this };
        }

        public override string GetTypeDisplay
        {
            get
            {
                return "Папка";
            }
        }
        public override Bitmap GetImage()
        {
            return global::Buhta.Properties.Resources.SchemaFolder_16;
        }

        public override Color GetSchemaDesinerColor()
        {
            return Цвета.ДизайнерСхемы_ПапкаОткрытая;
        }

        public override DateTime? GetSchemaDesignerChangeDate()
        {
            return null;
        }

        public override string GetSchemaDesignerChangeUser()
        {
            return null;
        }
    }





}


