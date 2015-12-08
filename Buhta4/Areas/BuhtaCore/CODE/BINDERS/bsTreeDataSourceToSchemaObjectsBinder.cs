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
    public class bsTreeDataSourceToSchemaObjectsBinder : OneWayBinder<object>
    {
        public string selectedObjectModelPropertyName;
        public Type SchemaObjectType;

        public Guid? selectedRow;
        public bsTree Tree;

        public bsTreeDataSourceToSchemaObjectsBinder()
        {
            IsNotAutoUpdate = true;
        }

        public override string GetJsForSettingProperty()
        {

            if (selectedObjectModelPropertyName != null)
            {
                var _selectedRows = Control.Model.GetPropertyValue(selectedObjectModelPropertyName);
                if (_selectedRows != null)
                {
                    if (!(_selectedRows is Guid?))
                        throw new Exception(nameof(bsTreeDataSourceToSchemaObjectsBinder) + ": свойство '" + selectedObjectModelPropertyName + "' должно быть 'Guid'");
                    selectedRow = (Guid?)_selectedRows;
                }
            }


            HashSet<SchemaObject> list = new HashSet<SchemaObject>();
            foreach (var _obj in App.Schema.Objects_cache.Values.Where((o) => o.SampleObject is SchemaTable).ToList())
            {
                list.Add(_obj.SampleObject);

                SchemaObject parent = _obj.SampleObject.GetParentObject();
                while (parent != null)
                {
                    if (!list.Contains(parent))
                        list.Add(parent);
                    parent = parent.GetParentObject();
                }
            }



            var ret = new jsArray();

            foreach (var obj in list)
            {
                var treeNode = new JsObject();

                treeNode.AddProperty("expanded", true);
                treeNode.AddProperty("title", obj.Name);
                treeNode.AddProperty("id", obj.ID.ToString());
                if (!string.IsNullOrWhiteSpace(obj.ParentObjectID.ToString()))
                    treeNode.AddProperty("parent", obj.ParentObjectID.ToString());
                treeNode.AddProperty("icon", new UrlHelper(HttpContext.Current.Request.RequestContext).Content(@"~/Areas/BuhtaSchemaDesigner/Content/icon/" + obj.GetType().Name + "_16.png"));

                if (selectedRow != null && selectedRow == obj.ID)
                {
                    treeNode.AddProperty("selected", true);
                }

                var jsRow = new JsObject();
                foreach (var col in Tree.Columns)
                {
                    var _propValue = obj.GetPropertyValue(col.Field_Bind);
                    if (_propValue != null)
                        jsRow.AddRawProperty(col.Field_Bind, _propValue.AsJavaScript());
                }
                treeNode.AddProperty("row", jsRow);

                ret.AddObject(treeNode);

                Debug.Print(treeNode.ToJson());
            }

            return "$('#" + Control.UniqueId + "').fancytree('option','source',buhta.FancyTree.convertFlatDataToTree(" + ret.ToJson() + "));";


        }

    }

}