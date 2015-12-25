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
            foreach (var _obj in App.Schema.Objects_cache.Values.Where((o) => SchemaObjectType.IsAssignableFrom(o.SampleObject.GetType())).ToList())
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

            // helper-tables
            var helperFolder = new SchemaFolder();
            helperFolder.ID = Guid.Parse("9FC553AC-448C-426C-916A-EFCF10775666");
            helperFolder.Name = "Объекты конфигурации";
            list.Add(helperFolder);

            foreach (var helperTable in App.Schema.HelperTables)
            {
                helperTable.ParentObjectID = helperFolder.ID;
                list.Add(helperTable);
            }

            // virtual tables
            var virtualFolder = new SchemaFolder();
            virtualFolder.ID = Guid.Parse("528C5B68-E8A5-4EB6-892F-169B083AF847");
            virtualFolder.Name = "Виртуальные таблицы";
            list.Add(virtualFolder);


            //if (IsIncludeRoleTables)
            //{
            //    foreach (var role in SchemaBaseRole.Roles.Values)
            //    {
            //        if (role is Таблица_TableRole)
            //        {
            //            list.Add(role);
            //        }
            //    }
            //}




            var ret = new JsArray();

            foreach (var obj in list)
            {
                var treeNode = new JsObject();

                treeNode.AddProperty("expanded", true);
                treeNode.AddProperty("title", obj.Name);
                treeNode.AddProperty("id", obj.ID.ToString());
                if (!string.IsNullOrWhiteSpace(obj.ParentObjectID.ToString()))
                    treeNode.AddProperty("parent", obj.ParentObjectID.ToString());
                treeNode.AddProperty("icon", new UrlHelper(HttpContext.Current.Request.RequestContext).Content(@"~/MODULES/BUHTA/CORE/Content/icon/" + obj.GetType().Name + "_16.png"));

                if (selectedRow != null && selectedRow == obj.ID)
                {
                    treeNode.AddProperty("selected", true);
                }

                if (Tree.IsExpandAllNodes)
                    treeNode.AddProperty("expanded", true);

                var jsRow = new JsObject();
                foreach (var col in Tree.Columns)
                {
                    var _propValue = obj.GetPropertyValue(col.Field_Bind);
                    if (_propValue != null)
                        jsRow.AddRawProperty(col.Field_Bind, _propValue.AsJavaScript());
                }
                treeNode.AddProperty("row", jsRow);

                ret.AddObject(treeNode);
            }

            foreach (var virtualTable in SchemaVirtualTable.VirtualTables.Values)
            {
                virtualTable.ParentObjectID = virtualTable.ID;

                var treeNode = new JsObject();

                treeNode.AddProperty("expanded", true);
                treeNode.AddProperty("title", virtualTable.Name);
                treeNode.AddProperty("id", virtualTable.ID.ToString());
                if (!string.IsNullOrWhiteSpace(virtualTable.ParentObjectID.ToString()))
                    treeNode.AddProperty("parent", virtualTable.ParentObjectID.ToString());
                treeNode.AddProperty("icon", new UrlHelper(HttpContext.Current.Request.RequestContext).Content(@"~/MODULES/BUHTA/CORE/Content/icon/" + virtualTable.GetType().Name + "_16.png"));

                if (selectedRow != null && selectedRow == virtualTable.ID)
                {
                    treeNode.AddProperty("selected", true);
                }

                var jsRow = new JsObject();
                foreach (var col in Tree.Columns)
                {
                    var _propValue = virtualTable.GetPropertyValue(col.Field_Bind);
                    if (_propValue != null)
                        jsRow.AddRawProperty(col.Field_Bind, _propValue.AsJavaScript());
                }
                treeNode.AddProperty("row", jsRow);

                ret.AddObject(treeNode);
            }

            return Tree.GetLoadDataScript(ret, true);

            //return "$('#" + Control.UniqueId + "').fancytree('option','source',buhta.FancyTree.convertFlatDataToTree(" + ret.ToJson() + "));";


        }

    }

}