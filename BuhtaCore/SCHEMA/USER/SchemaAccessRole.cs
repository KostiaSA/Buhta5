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
    public class SchemaAccessRole : SchemaObject
    {
        public ObservableCollection<SchemaAccessRoleItem> Items { get; private set; }

        public SchemaAccessRole()
        {
            Items = new ObservableCollection<SchemaAccessRoleItem>();
            Items.CollectionChanged += Items_CollectionChanged;

        }

        void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            firePropertyChanged("Items");
        }

        public override string GetTypeDisplay
        {
            get
            {
                return "Роль доступа";
            }
        }

        public override void PrepareNew()
        {
            base.PrepareNew();
            Name = "Имя новой роли доступа";
        }

        public override BaseEdit_Page GetEditForm_page()
        {
            return new BaseEdit_Page("SchemaAccessRoleDesigner_page() { EditedRecord = this }");
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
            return new Bitmap("global::Buhta.Properties.Resources.SchemaAccessRole_16");
        }

        public static List<SchemaAccessRoleItem> GetAllItems(Guid? parentItemID)
        {
            var list = new List<SchemaAccessRoleItem>();

            foreach (var schemaMenu in App.Schema.GetSampleObjects<SchemaAccessRole>())
            {
                foreach (SchemaAccessRoleItem item in schemaMenu.Items)
                {
                    if (item.ParentRoleItemID == parentItemID)
                        list.Add(item);
                }

            }

            return list.OrderBy(x => x.Name).ToList();
        }

        public static SchemaAccessRoleItem GetItemByID(Guid? id)
        {
            foreach (var schemaMenu in App.Schema.GetSampleObjects<SchemaAccessRole>())
            {
                foreach (SchemaAccessRoleItem item in schemaMenu.Items)
                {
                    if (item.ID == id)
                        return item as SchemaAccessRoleItem;
                }
            }
            return null;
        }

    }


}
