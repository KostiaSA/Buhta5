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
    public class bsSelectOptionsToObjectsListBinder : OneWayBinder<object>
    {
        public string DatasourceModelPropertyName;

        public string KeyFieldName;
        public string TitleFieldName;
        public string SortFieldName;

        public bsSelect Select;

        public bsSelectOptionsToObjectsListBinder()
        {
            IsNotAutoUpdate = true;
        }


        public override string GetJsForSettingProperty()
        {
            var _objectList = Control.Model.GetPropertyValue(DatasourceModelPropertyName);

            var objectList = _objectList as System.Collections.IEnumerable;

            if (objectList == null)
                throw new Exception(nameof(bsSelectOptionsToObjectsListBinder) + ": свойство '" + DatasourceModelPropertyName + "' должно быть типа '" + nameof(System.Collections.IEnumerable) + "'");

            if (KeyFieldName == null)
                throw new Exception(nameof(bsSelectOptionsToObjectsListBinder) + " не указан KeyFieldName");

            //if (_objectList.GetType().IsGenericType && _objectList.GetType().GetGenericTypeDefinition() == typeof(ObservableCollection<>))
            //{
            //    ((dynamic)_objectList).CollectionChanged += new NotifyCollectionChangedEventHandler(DataSourceRows_CollectionChanged);
            //}

            var ret = new jsArray();


            foreach (var obj in objectList)
            {
                var jsrow = new JsObject();

                jsrow.AddProperty("id", obj.GetPropertyValue(KeyFieldName));
                if (TitleFieldName != null)
                    jsrow.AddProperty("title", obj.GetPropertyValue(TitleFieldName));
                if (SortFieldName != null)
                    jsrow.AddProperty("sort", obj.GetPropertyValue(SortFieldName));

                ret.AddObject(jsrow);
            }

            return "$('#" + Control.UniqueId + "')[0].selectize.clearOptions();$('#" + Control.UniqueId + "')[0].selectize.addOption(" + ret.ToJson() + ")";

        }


    }

}