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
    public class bsTreeDataSourceBinder : OneWayBinder<object>
    {
        public string DatasourceModelPropertyName;

        public string DisplayFieldName;
        public string KeyFieldName;
        public string ParentFieldName;
        public string IconFieldName;
        //        public string SelectedRowsModelPropertyName;

        public bsTree Tree;

        public bsTreeDataSourceBinder()
        {
            IsNotAutoUpdate = true;
        }


        //        ObservableCollection<string> selectedRows;
        public override string GetJsForSettingProperty()
        {
            if (Tree.DataSource == null)
            {
                var _dataSource = (bsTreeDataSource)Tree.Model.GetPropertyValue(DatasourceModelPropertyName);
                if (_dataSource==null)
                    throw new Exception(nameof(bsTreeDataSourceBinder) + ": свойство '" + DatasourceModelPropertyName + "' должно быть типа '" + nameof(bsTreeDataSource) + "'");
                Tree.DataSource = _dataSource;
            }

            return Tree.DataSource.GetJsForSettingProperty(Tree);

        }


    }

}