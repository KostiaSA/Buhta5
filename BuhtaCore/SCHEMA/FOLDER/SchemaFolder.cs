using BLToolkit.DataAccess;
using BLToolkit.Mapping;
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

namespace Buhta
{

    [Export(typeof(SchemaObject))]
    public class SchemaFolder : SchemaObject
    {
        public override BaseEdit_Page GetEditForm_page()
        {
            return new BaseEdit_Page("SchemaFolderDesigner_page() { EditedRecord = this }");
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
            return new Bitmap("global::Buhta.Properties.Resources.SchemaFolder_16");
        }

        public override Color GetSchemaDesinerColor()
        {
            return new Color("Цвета.ДизайнерСхемы_ПапкаОткрытая");
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


