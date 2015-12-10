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
    public class bsTreeDataSourceToSqlDataViewBinder : OneWayBinder<object>
    {
        public string DatasourceModelPropertyName;

        public string DisplayFieldName;
        public string KeyFieldName;
        public string ParentFieldName;
        public string IconFieldName;
        public string SelectedRowsModelPropertyName;

        public bsTree Tree;

        public bsTreeDataSourceToSqlDataViewBinder()
        {
            IsNotAutoUpdate = true;
        }


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
                    throw new Exception(nameof(bsTreeDataSourceToSqlDataViewBinder) + ": свойство '" + SelectedRowsModelPropertyName + "' должно быть типа '" + nameof(ObservableCollection<string>) + "'");
                selectedRows = (ObservableCollection<string>)_selectedRows;
            }

            var ret = new JsArray();

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
                treeNode.AddProperty("icon", new UrlHelper(HttpContext.Current.Request.RequestContext).Content(row[iconFieldName].ToString()));

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
   
            return "$('#" + Control.UniqueId + "').fancytree('option','source',buhta.FancyTree.convertFlatDataToTree(" + ret.ToJson() + "));";


        }


    }

}