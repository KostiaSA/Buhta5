using BLToolkit.DataAccess;
using BLToolkit.Mapping;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public class SchemaModule : SchemaObject
    {
        public SchemaModule()
        {
            References = new ObservableCollection<Guid>();
            References.CollectionChanged += References_CollectionChanged;
        }

        void References_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            firePropertyChanged("References");
        }

        public override BaseEdit_Page GetEditForm_page()
        {
            return new BaseEdit_Page("SchemaModuleDesigner_page() { EditedRecord = this }");
        }

        public override string GetTypeDisplay
        {
            get
            {
                return "Модуль";
            }
        }

        private string prefix;
        public string Prefix
        {
            get { return prefix; }
            set { prefix = value; firePropertyChanged("Prefix"); }
        }

        public ObservableCollection<Guid> References { get; private set; }

        public override Bitmap GetImage()
        {
            return new Bitmap("global::Buhta.Properties.Resources.SchemaModule_16");
        }

        public override Color GetSchemaDesinerColor()
        {
            return new Color("Цвета.ДизайнерСхемы_Модуль");
        }

    }





}


