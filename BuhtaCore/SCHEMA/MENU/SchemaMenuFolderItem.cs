using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{

    public class SchemaMenuFolderItem : SchemaMenuBaseItem
    {
        Guid id;
        [Browsable(false)]
        public Guid ID
        {
            get { return id; }
            set { id = value; firePropertyChanged("ID"); }
        }

        private string nameForToolbar;
        [DisplayName("Имя для toolbar-а"), Description("Имя пункта меню для toolbar-а в главном окне."), Category(Menu_category)]
        ////[Editor(typeof(MultilineStringEditor), typeof(UITypeEditor))]
        public string NameForToolbar
        {
            get { return nameForToolbar; }
            set { nameForToolbar = value; firePropertyChanged("NameForToolbar"); }
        }

        public SchemaMenuFolderItem()
        {
        }

        //public override void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
        //{
        //    var menuFolder = info.Node as SchemaMenuFolderItem;
        //    List<SchemaMenuBaseItem> list = new List<SchemaMenuBaseItem>();

        //    foreach (SchemaMenu menu in App.Schema.GetSampleObjects<SchemaMenu>())
        //    {
        //        foreach (var menuItem in menu.Items)
        //        {
        //            if (menuItem.ParentMenuFolderID == menuFolder.ID && //(menuItem.ParentMenu == menuFolder.ParentMenu ||
        //                menuItem.ParentMenu != App.Schema.GetSampleObject<SchemaMenu>(menuItem.ParentMenu.ID))
        //                list.Add(menuItem);
        //        }
        //    }
        //    info.Children = list;
        //}



        //public override void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info)
        //{
        //        if (info.Column.FieldName == "Name")
        //        {
        //            if (GetJoinView() != null)
        //                info.CellData = Name + " -> " + GetJoinView().GetDisplayName();
        //            else
        //                info.CellData = Name + " -> ?";
        //        }
        //        else
        //            if (info.Column.FieldName == "Position")
        //                info.CellData = (info.Node as SchemaQueryBaseColumn).Position;
        //            else
        //                info.CellData = null;
        //    }
        //}

        //public override IViewColumn GetJoinView()
        //{
        //    if (GetSourceView() != null && GetSourceView().GetColumnByName(Name) != null)
        //        return GetSourceView().GetColumnByName(Name).GetJoinView();
        //    else
        //        return null;
        //}


        public override void GetAllItems(List<SchemaMenuBaseItem> list)
        {
            //list.Add(this);
            //foreach (var col in Items)
            //    col.GetAllItems(list);
        }

        //public override void EmitJoinOnSql(StringBuilder sql, string indent)
        //{
        //    sql.AppendLine("FROM");
        //    sql.AppendLine("  [" + GetJoinView().Name + "] AS [" + GetJoinTableFillAlias() + "]");

        //    foreach (var col in Columns)
        //        col.EmitJoinSql(sql, indent + "  ");
        //}

        //public override IView GetJoinQueryTable()
        //{
        //    return GetSourceColumn().GetQueryTable();
        //}

        //public virtual IView GetSourceTable()
        //{
        //    return ParentColumn.GetJoinQueryTable();
        //}
    }


}
