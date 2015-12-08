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


            var ret = new jsArray();

            foreach (var _obj in App.Schema.Objects_cache.Values.Where((o) => o.SampleObject is SchemaTable || o.SampleObject is SchemaFolder || o.SampleObject is SchemaModule))
            {
                var treeNode = new JsObject();
                SchemaObject obj = _obj.SampleObject;

                treeNode.AddProperty("expanded", true);
                treeNode.AddProperty("title", obj.Name);
                treeNode.AddProperty("id", obj.ID.ToString());
                if (!string.IsNullOrWhiteSpace(obj.ParentObjectID.ToString()))
                    treeNode.AddProperty("parent", obj.ParentObjectID.ToString());
                //treeNode.AddProperty("icon", row[iconFieldName].ToString());

                if (selectedRow != null && selectedRow == obj.ID)
                {
                    treeNode.AddProperty("selected", true);
                }

                //var jsRow = new JsObject();
                //foreach (var col in Tree.Columns)
                //{
                //    jsRow.AddRawProperty(col.Field_Bind, row[col.Field_Bind].AsJavaScript());
                //}
                //treeNode.AddProperty("row", jsRow);

                ret.AddObject(treeNode);

                Debug.Print(treeNode.ToJson());
            }

            return "$('#" + Control.UniqueId + "').fancytree('option','source',buhta.FancyTree.convertFlatDataToTree(" + ret.ToJson() + "));";


        }

    }

}