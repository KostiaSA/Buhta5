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
    public class bsGridDataSourceToObjectsListBinder : OneWayBinder
    {
        public string DatasourceModelPropertyName;

        public string KeyFieldName;
        public string IconFieldName;
        public string SelectedRowsModelPropertyName;

        public bsGrid Tree;

        public bsGridDataSourceToObjectsListBinder()
        {
            IsNotAutoUpdate = true;
        }


        ObservableCollection<string> selectedRows;
        public override string GetJsForSettingProperty()
        {
            var _view = Control.Model.GetPropertyValue(DatasourceModelPropertyName);

            var view = _view as System.Collections.IEnumerable;

            if (view == null)
                throw new Exception(nameof(bsGridDataSourceToObjectsListBinder) + ": свойство '" + DatasourceModelPropertyName + "' должно быть типа '" + nameof(System.Collections.IEnumerable) + "'");


            //if (SelectedRowsModelPropertyName != null)
            //{
            //    var _selectedRows = Control.Model.GetPropertyValue(SelectedRowsModelPropertyName);
            //    if (!(_selectedRows is ObservableCollection<string>))
            //        throw new Exception(nameof(bsGridDataSourceToObjectsListBinder) + ": свойство '" + SelectedRowsModelPropertyName + "' должно быть типа '" + nameof(ObservableCollection<string>) + "'");
            //    selectedRows = (ObservableCollection<string>)_selectedRows;
            //}

            var ret = new jsArray();


            foreach (var row in view)
            {
                //DataRow row = rowView.Row;
                var jsrow = new jsArray();

                foreach (var col in Tree.Columns)
                {
                    jsrow.AddObject(row.GetPropertyValue(col.Field_Bind));
                }

                //if (selectedRows != null && selectedRows.Contains(row[keyFieldName].ToString()))
                //{
                //    jsrow.AddProperty("selected", true);
                //    //  treeNode.AddProperty("expanded", true);
                //}

                ret.AddObject(jsrow);
            }
   
            return "$('#" + Control.UniqueId + "').DataTable().clear().rows.add(" + ret.ToJson() + ").draw();";


        }


    }

}