using DevExpress.XtraTreeList;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{

    public class SchemaMenuActionItem : SchemaMenuBaseItem
    {

        private SchemaMenuBaseAction action;
        [DisplayName("Действие"), Description("Действие"), Category(" Действие")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public SchemaMenuBaseAction Action
        {
            get { return action; }
            set { action = value; firePropertyChanged("Action"); }
        }


        public SchemaMenuActionItem()
        {
        }

        public override void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info)
        {
            //info.Children = Items;
        }



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
