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
    public class SchemaMenu : SchemaObject
    {
        public ObservableCollection<SchemaMenuBaseItem> Items { get; private set; }

        public SchemaMenu()
        {
            Items = new ObservableCollection<SchemaMenuBaseItem>();
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
                return "Меню";
            }
        }

        public override void PrepareNew()
        {
            base.PrepareNew();
            Name = "Имя нового пункта меню";
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
            return new Bitmap("global::Buhta.Properties.Resources.SchemaMenu_16");
        }

        public static List<SchemaMenuBaseItem> GetAllItems(Guid? parentItemID)
        {
            var list = new List<SchemaMenuBaseItem>();

            foreach (var schemaMenu in App.Schema.GetSampleObjects<SchemaMenu>())
            {
                foreach (SchemaMenuBaseItem item in schemaMenu.Items)
                {
                    if (item.ParentMenuFolderID == parentItemID)
                        list.Add(item);
                }

            }

            return list.OrderBy(x => x.Name).OrderBy(x => x.Position).ToList();
        }

        public static List<SchemaMenuFolderItem> GetFolderItems(Guid? parentItemID)
        {
            var list = new List<SchemaMenuFolderItem>();

            foreach (var schemaMenu in App.Schema.GetSampleObjects<SchemaMenu>())
            {
                foreach (SchemaMenuBaseItem item in schemaMenu.Items)
                {
                    if (item.ParentMenuFolderID == parentItemID && item is SchemaMenuFolderItem)
                        list.Add(item as SchemaMenuFolderItem);
                }

            }

            return list.OrderBy(x => x.Name).OrderBy(x => x.Position).ToList();
        }

        public static List<SchemaMenuActionItem> GetActionItems(Guid? parentItemID)
        {
            var list = new List<SchemaMenuActionItem>();

            foreach (var schemaMenu in App.Schema.GetSampleObjects<SchemaMenu>())
            {
                foreach (SchemaMenuBaseItem item in schemaMenu.Items)
                {
                    if (item.ParentMenuFolderID == parentItemID && item is SchemaMenuActionItem)
                    {
                        list.Add(item as SchemaMenuActionItem);
                    }
                }

            }

            return list.OrderBy(x => x.Name).OrderBy(x => x.Position).ToList();
        }

        public static SchemaMenuFolderItem GetFolderItemByID(Guid? id)
        {
            foreach (var schemaMenu in App.Schema.GetSampleObjects<SchemaMenu>())
            {
                foreach (SchemaMenuBaseItem item in schemaMenu.Items)
                {
                    if (item is SchemaMenuFolderItem && (item as SchemaMenuFolderItem).ID == id)
                        return item as SchemaMenuFolderItem;
                }

            }
            return null;
        }

    }


}
