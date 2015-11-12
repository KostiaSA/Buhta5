using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{

    public class SchemaMenuBaseItem : ISupportInitialize, INotifyPropertyChanged
    {
        public const string Menu_category = "  Меню";
        public const string Image_category = "Иконка";

        private string name;
        [DisplayName("Имя"), Description("Имя пункта меню"), Category(Menu_category)]
        public string Name
        {
            get { return name; }
            set { name = value; firePropertyChanged("Name"); }
        }

        private string description;
        [DisplayName("Описание"), Description("Описание пункта меню"), Category(Menu_category)]
        public string Description
        {
            get { return description; }
            set { description = value; firePropertyChanged("Description"); }
        }

        private int position;
        [DisplayName("Позиция"), Description("Порядок/позиция в списке"), Category(Menu_category)]
        public int Position
        {
            get { return position; }
            set { position = value; firePropertyChanged("Position"); }
        }

        private Bitmap image;          
        ////[EditorAttribute(typeof(System.Drawing.Design.ImageEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [DisplayName("Иконка"), Description("Иконка"), Category(Image_category)]
        public Bitmap Image
        {
            get
            {
                return image;
            }
            set
            {
                ////if (value.Width != 32 && value.Height != 32)
                ////    image = Images.ResizeBitmap(value, 32);
                ////else
                ////    image = value;
                ////firePropertyChanged("Image");
            }
        }

        private Guid? parentMenuFolderID;
        [Browsable(false)]
        public Guid? ParentMenuFolderID
        {
            get { return parentMenuFolderID; }
            set { parentMenuFolderID = value; firePropertyChanged("ParentMenuFolderID"); }
        }

        private SchemaMenu parentMenu;
        [Browsable(false)]
        public SchemaMenu ParentMenu
        {
            get { return parentMenu; }
            set { parentMenu = value; firePropertyChanged("ParentMenu"); }
        }

        [JsonIgnore]
        public SchemaMenu DesignTimeEditedMenu;


        public virtual void GetAllItems(List<SchemaMenuBaseItem> list)
        {
            list.Add(this);
        }

        public virtual List<SchemaMenuBaseItem> GetAllColumns()
        {
            var list = new List<SchemaMenuBaseItem>();
            GetAllItems(list);
            return list;
        }

        public int GetLevel()
        {
            if (ParentMenuFolderID == null)
                return 0;
            else
            {
                var parentFolder = SchemaMenu.GetFolderItemByID(ParentMenuFolderID);
                if (parentFolder != null)
                    return parentFolder.GetLevel() + 1;
                else
                    return 0;
            }
        }

        void ISupportInitialize.BeginInit()
        {
        }

        void ISupportInitialize.EndInit()
        {
        }

        ////public virtual void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
        ////{
        ////    if (info.Column.FieldName == "Name")
        ////        info.CellData = (info.Node as SchemaMenuBaseItem).Name;
        ////    else
        ////        if (info.Column.FieldName == "Description")
        ////            info.CellData = (info.Node as SchemaMenuBaseItem).Description;
        ////        else
        ////            info.CellData = null;
        ////}

        ////public virtual void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
        ////{
        ////    info.Children = null;
        ////}

        ////public void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info)
        ////{
        ////    throw new NotImplementedException();
        ////}

        public event PropertyChangedEventHandler PropertyChanged;

        public void firePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (parentMenu != null)
                parentMenu.firePropertyChanged("Items");

        }


        public string GetDisplayName()
        {
            return Name;
        }


        public string GetDisplayNameAndDataType()
        {
            return "GetFullAlias()";
        }


    }

}
