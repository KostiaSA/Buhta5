using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{
    [Serializable]
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


        public void SelectRowById(string id)
        {
            Tree.Model.ExecuteJavaScript("buhta.DataTables.SelectRowById(" + Tree.UniqueId.AsJavaScript() + ", " + KeyFieldName.AsJavaScript() + ", " + id.AsJavaScript() + ")");
            //buhta.DataTables.SelectRowById = function(tableId, keyColumnName, id) {            }

        }
        //ObservableCollection<string> selectedRows;

        public override string GetJsForSettingProperty()
        {
            var _objectList = Control.Model.GetPropertyValue(DatasourceModelPropertyName);

            var objectList = _objectList as System.Collections.IEnumerable;

            if (objectList == null)
                throw new Exception(nameof(bsGridDataSourceToObjectsListBinder) + ": свойство '" + DatasourceModelPropertyName + "' должно быть типа '" + nameof(System.Collections.IEnumerable) + "'");

            //if (_objectList.GetType().IsGenericType && _objectList.GetType().GetGenericTypeDefinition() == typeof(ObservableCollection<>))
            //{
            //    ((dynamic)_objectList).CollectionChanged += new NotifyCollectionChangedEventHandler(DataSourceRows_CollectionChanged);
            //}

            var ret = new jsArray();


            foreach (var obj in objectList)
            {
                //DataRow row = rowView.Row;
                var jsrow = new jsArray();

                //if (obj is INotifyPropertyChanged)
                //{
                //    (obj as INotifyPropertyChanged).PropertyChanged += BsGridDataSourceToObjectsListBinder_PropertyChanged1;

                //    //(obj as INotifyPropertyChanged).PropertyChanged += (sender, e) => {
                //    //    var updateData = new jsArray();
                //    //    foreach (var col in Tree.Columns)
                //    //    {
                //    //        updateData.AddObject(obj.GetPropertyValue(col.Field_Bind));
                //    //    }
                //    //    var updatejs="alert('" + ret.ToJson() + "');";
                //    //    Control.Model.ExecuteJavaScript(updatejs);
                //    //    //var updatejs = "$('#" + Control.UniqueId + "').DataTable().clear().rows.add(" + ret.ToJson() + ").draw();";

                //    //};
                //}

                foreach (var col in Tree.Columns)
                {
                    jsrow.AddObject(obj.GetPropertyValue(col.Field_Bind));
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