using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{
    public class bsTreeDataSourceBinder : BaseBinder
    {
        public string DatasourceModelPropertyName;

        public string DisplayFieldName;
        public string KeyFieldName;
        public string ParentFieldName;
        public string IconFieldName;
        public string SelectedRowsModelPropertyName;

        public bsNewTree Tree;


        public override BinderEventMethod ModelEventMethod { get; set; }
        public override BinderSetMethod ModelSetMethod { get; set; }


        ObservableCollection<string> selectedRows;
        public override string GetJsForSettingProperty()
        {
            var _view = Control.Model.GetPropertyValue(DatasourceModelPropertyName);
            if (!(_view is DataView))
                throw new Exception(nameof(bsTreeDataSourceToSqlDataViewBinder) + ": свойство '" + DatasourceModelPropertyName + "' должно быть типа '" + nameof(DataView) + "'");
            DataView dataView = (DataView)_view;

            if (SelectedRowsModelPropertyName != null)
            {
                var _selectedRows = Control.Model.GetPropertyValue(SelectedRowsModelPropertyName);
                if (!(_selectedRows is ObservableCollection<string>))
                    throw new Exception(nameof(bsTreeDataSourceBinder) + ": свойство '" + SelectedRowsModelPropertyName + "' должно быть типа '" + nameof(ObservableCollection<string>) + "'");
                selectedRows = (ObservableCollection<string>)_selectedRows;
            }

            var ret = new jsArray();

            var displayFieldName = DisplayFieldName;
            if (displayFieldName == null && dataView.Table.Columns.Contains("__title__"))
                displayFieldName = "__title__";

            var keyFieldName = KeyFieldName;
            if (keyFieldName == null && dataView.Table.Columns.Contains("__ID__"))
                keyFieldName = "__ID__";
            if (keyFieldName == null && dataView.Table.Columns.Contains("ID"))
                keyFieldName = "ID";

            var parentFieldName = ParentFieldName;
            if (parentFieldName == null && dataView.Table.Columns.Contains("__PARENTID__"))
                parentFieldName = "__PARENTID__";
            if (parentFieldName == null && dataView.Table.Columns.Contains("PARENTID"))
                parentFieldName = "PARENTID";

            var iconFieldName = IconFieldName;
            if (iconFieldName == null && dataView.Table.Columns.Contains("__icon__"))
                iconFieldName = "__icon__";

            foreach (DataRowView rowView in dataView)
            {
                DataRow row = rowView.Row;
                var treeNode = new JsObject();

                treeNode.AddProperty("title", row[displayFieldName].ToString());
                treeNode.AddProperty("id", row[keyFieldName].ToString());
                treeNode.AddProperty("parent", row[parentFieldName].ToString());
                treeNode.AddProperty("icon", row[iconFieldName].ToString());

                if (selectedRows != null && selectedRows.Contains(row[keyFieldName].ToString()))
                {
                    treeNode.AddProperty("selected", true);
                    //  treeNode.AddProperty("expanded", true);
                }

                var jsRow = new JsObject();
                foreach (var col in Tree.Columns)
                {
                    jsRow.AddRawProperty(col.Field_Bind, row[col.Field_Bind].AsJavaScript());
                }
                treeNode.AddProperty("row", jsRow);

                ret.AddObject(treeNode);
            }
   
            return "$('#" + Control.UniqueId + "').fancytree('option','source',convertFlatDataToFancyTree(" + ret.ToJson() + "));";


            //var value = Control.Model.GetPropertyValue(ModelPropertyName);
            //if (value is IEnumerable<Guid>)
            //{
            //    var list = new List<Таблица_TableRole>();
            //    string errorStr = ""; ;
            //    foreach (var roleID in (value as IEnumerable<Guid>))
            //    {
            //        if (SchemaBaseRole.Roles.ContainsKey(roleID) && SchemaBaseRole.Roles[roleID] is Таблица_TableRole)
            //            list.Add(SchemaBaseRole.Roles[roleID] as Таблица_TableRole);
            //        else
            //            errorStr += ", ?ошибка";
            //    }
            //    var sb = new StringBuilder();
            //    foreach (var tableRole in from role in list orderby role.Position, role.Name select role)
            //    {
            //        sb.Append(tableRole.Name + ", ");
            //    }
            //    sb.RemoveLastChar(2);
            //    sb.Append(errorStr);
            //    return "$('#" + Control.UniqueId + "').val(" + sb.ToString().AsJavaScript() + ");";
            //}
            //else
            //    throw new Exception(nameof(bsInputToTableRolesBinder) + "." + nameof(GetJsForSettingProperty) + "(): привязанное свойство должено быть 'IEnumerable<Guid>'");
        }

        public override void EmitBindingScript(StringBuilder script)
        {
            Control.Model.RegisterBinder(this);
            LastSendedText = GetJsForSettingProperty();
            script.AppendLine(LastSendedText);
        }

    }

}