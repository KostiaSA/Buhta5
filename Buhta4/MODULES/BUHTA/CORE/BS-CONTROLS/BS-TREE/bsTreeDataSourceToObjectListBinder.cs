using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Buhta
{
    public class bsTreeDataSourceToObjectListBinder : OneWayBinder<object>
    {
        public string DatasourceModelPropertyName;

        public string DisplayFieldName;
        public string KeyFieldName;
        public string ParentFieldName;
        public string IconFieldName;
        public string SelectedRowsModelPropertyName;

        public bsTree Tree;

        public bsTreeDataSourceToObjectListBinder()
        {
            IsNotAutoUpdate = true;
        }


        ObservableCollection<string> selectedRows;
        public override string GetJsForSettingProperty()
        {
            var _list = Control.Model.GetPropertyValue(DatasourceModelPropertyName);
            var list = _list as IEnumerable<object>;
            if (list == null)
                throw new Exception(nameof(bsTreeDataSourceToObjectListBinder) + ": свойство '" + DatasourceModelPropertyName + "' должно быть типа 'IEnumerable<object>'");

            if (SelectedRowsModelPropertyName != null)
            {
                var _selectedRows = Control.Model.GetPropertyValue(SelectedRowsModelPropertyName);
                if (!(_selectedRows is ObservableCollection<string>))
                    throw new Exception(nameof(bsTreeDataSourceToObjectListBinder) + ": свойство '" + SelectedRowsModelPropertyName + "' должно быть типа '" + nameof(ObservableCollection<string>) + "'");
                selectedRows = (ObservableCollection<string>)_selectedRows;
            }

            var ret = new JsArray();

            var displayFieldName = DisplayFieldName;
            if (displayFieldName == null)
                throw new Exception(nameof(bsTreeDataSourceToObjectListBinder) + ": не заполнено свойство '" + nameof(DisplayFieldName) + "'");
            //if (displayFieldName == null && dataView.Table.Columns.Contains("__title__"))
            //    displayFieldName = "__title__";

            var keyFieldName = KeyFieldName;
            if (keyFieldName == null)
                throw new Exception(nameof(bsTreeDataSourceToObjectListBinder) + ": не заполнено свойство '" + nameof(KeyFieldName) + "'");
            //if (keyFieldName == null && dataView.Table.Columns.Contains("__ID__"))
            //    keyFieldName = "__ID__";
            //if (keyFieldName == null && dataView.Table.Columns.Contains("ID"))
            //    keyFieldName = "ID";

            var parentFieldName = ParentFieldName;
            //if (parentFieldName == null && dataView.Table.Columns.Contains("__PARENTID__"))
            //    parentFieldName = "__PARENTID__";
            //if (parentFieldName == null && dataView.Table.Columns.Contains("PARENTID"))
            //    parentFieldName = "PARENTID";

            var iconFieldName = IconFieldName;
            //if (iconFieldName == null && dataView.Table.Columns.Contains("__icon__"))
            //    iconFieldName = "__icon__";

            foreach (object row in list)
            {
                var treeNode = new JsObject();

                var _title_obj = row.GetPropertyValue(displayFieldName);
                if (_title_obj != null)
                    treeNode.AddProperty("title", _title_obj.ToString());
                else
                    treeNode.AddProperty("title", "");

                var _id_obj = row.GetPropertyValue(keyFieldName);
                if (_id_obj != null)
                    treeNode.AddProperty("id", _id_obj.ToString());
                else
                    treeNode.AddProperty("id", "");

                if (parentFieldName != null)
                {
                    var _parent_obj = row.GetPropertyValue(parentFieldName);
                    if (_parent_obj != null)
                        treeNode.AddProperty("parent", _parent_obj.ToString());
                    else
                        treeNode.AddProperty("parent", "");
                }

                if (iconFieldName != null)
                {
                    var _icon_obj = row.GetPropertyValue(iconFieldName);
                    if (_icon_obj != null)
                        treeNode.AddProperty("icon", new UrlHelper(HttpContext.Current.Request.RequestContext).Content(_icon_obj.ToString()));
                }

                if (selectedRows != null && _id_obj != null && selectedRows.Contains(_id_obj.ToString()))
                {
                    treeNode.AddProperty("selected", true);
                    //  treeNode.AddProperty("expanded", true);
                }

                if (Tree.IsExpandAllNodes)
                    treeNode.AddProperty("expanded", true);

                var jsRow = new JsObject();
                foreach (var col in Tree.Columns)
                {
                    var _obj = row.GetPropertyValue(col.Field_Bind);
                    if (_obj != null)
                        jsRow.AddRawProperty(col.Field_Bind, row.GetPropertyValue(col.Field_Bind).AsJavaScript());
                    else
                        jsRow.AddRawProperty(col.Field_Bind, "");
                }
                treeNode.AddProperty("row", jsRow);

                ret.AddObject(treeNode);
            }

            return Tree.GetLoadDataScript(ret, true);

            //return "$('#" + Control.UniqueId + "').fancytree('option','source',buhta.FancyTree.convertFlatDataToTree(" + ret.ToJson() + "));";


        }


    }

}