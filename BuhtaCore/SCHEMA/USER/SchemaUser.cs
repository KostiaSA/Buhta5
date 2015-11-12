using BLToolkit.Aspects;
using Microsoft.SqlServer.Management.Smo;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{

    [Export(typeof(SchemaObject))]
    public class SchemaUser : SchemaObject
    {
        public ObservableCollection<SchemaAccessRoleItem> RoleItems { get; private set; }

        public SchemaUser()
        {
            RoleItems = new ObservableCollection<SchemaAccessRoleItem>();
            RoleItems.CollectionChanged += Items_CollectionChanged;

        }

        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            firePropertyChanged("RoleItems");
        }

        public override string GetTypeDisplay
        {
            get
            {
                return "Юзер";
            }
        }

        public override void PrepareNew()
        {
            base.PrepareNew();
            Name = "Имя нового пользователя";
        }

        public override BaseEdit_Page GetEditForm_page()
        {
            return new BaseEdit_Page("SchemaMenuDesigner_page() { EditedRecord = this }");
        }


        public string GetDisplayName()
        {
            return Name;
        }


        public string GetDisplayNameAndDataType()
        {
            throw new NotImplementedException();
        }


        public override Bitmap GetImage()
        {
            return new Bitmap("global::Buhta.Properties.Resources.SchemaUser_16");
        }

    }


}
