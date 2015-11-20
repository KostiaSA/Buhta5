using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Helpers;

namespace Buhta
{
    public class bsTreeDataSourceToDataViewBinder : BaseBsTreeDataSourceBinder
    {

        public string KeyFieldName;
        public string ParentFieldName;
        public string DisplayFieldName;
        public string IconFieldName;

        public bsTreeDataSourceToDataViewBinder() : base("") { }
        public bsTreeDataSourceToDataViewBinder(string propertyName) : base(propertyName) { }
        //public bsTreeDataSourceToDataViewBinder(string propertyName, string keyFieldName, string parentFieldName, string displayFieldName) : base(propertyName)
        //{
        //    KeyFieldName = keyFieldName;
        //    ParentFieldName = parentFieldName;
        //    DisplayFieldName = displayFieldName;
        //}

        public override string GetJsonDataTreeSource(BaseModel model)
        {
            var _view = model.GetPropertyValue(PropertyName);
            if (!(_view is DataView))
                throw new Exception(nameof(bsTreeDataSourceToDataViewBinder) + ": свойство '" + PropertyName + "' должно быть типа '" + nameof(DataView) + "'");
            DataView dataView = (DataView)_view;

            var ret = new List<Dictionary<string, string>>();

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
                var treeNode = new Dictionary<string, string>();

                treeNode.Add("title", row[displayFieldName].ToString());
                treeNode.Add("id", row[keyFieldName].ToString());
                treeNode.Add("parent", row[parentFieldName].ToString());
                treeNode.Add("icon", row[iconFieldName].ToString());

                ret.Add(treeNode);
            }
            return "convertFlatDataToFancyTree(" + Json.Encode(ret) + ")";

        }

        //public virtual string GetDisplayText(object value)
        //{
        //    throw new Exception("метод " + nameof(GetDisplayText) + " не рализован");
        //}

        //public virtual object ParseDisplayText(string text)
        //{
        //    throw new Exception("метод "+nameof(ParseDisplayText)+" не рализован");
        //}


    }
}