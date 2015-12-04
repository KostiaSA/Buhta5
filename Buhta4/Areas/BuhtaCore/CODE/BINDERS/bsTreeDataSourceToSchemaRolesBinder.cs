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
    public class bsTreeDataSourceToSchemaRolesBinder : OneWayBinder<object>
    {
        public string SelectedRowsModelPropertyName;
        public SchemaBaseRole RootRole;

        public bsTree Tree;

        public bsTreeDataSourceToSchemaRolesBinder()
        {
            IsNotAutoUpdate = true;
        }

        void AddRoleNode(jsArray nodeList, SchemaBaseRole role)
        {
            var node = new JsObject();
            node.AddProperty("title", "жопа");
            node.AddProperty("key", role.ID);
            node.AddProperty("expanded", true);

            if (selectedRows!=null && selectedRows.Contains(role.ID))
                node.AddProperty("selected", true);

            var row = new JsObject();
            row.AddProperty(nameof(SchemaBaseRole.ID), role.ID);
            row.AddProperty(nameof(SchemaBaseRole.Name), role.Name);
            row.AddProperty(nameof(SchemaBaseRole.Description), role.Description);
            row.AddProperty(nameof(SchemaBaseRole.Position), role.Position);
            node.AddProperty("row", row);
            nodeList.AddObject(node);

            var children = new jsArray();

            foreach (var childRole in SchemaBaseRole.Roles.Values.OrderBy((r) => r.Position))
            {
                if (childRole.GetType().BaseType != null && childRole.GetType().BaseType.Equals(role.GetType()))
                    AddRoleNode(children, childRole);
            }

            if (children.Length > 0)
                node.AddProperty("children", children);
        }


        ObservableCollection<Guid> selectedRows;
        public override string GetJsForSettingProperty()
        {

            if (SelectedRowsModelPropertyName != null)
            {
                var _selectedRows = Control.Model.GetPropertyValue(SelectedRowsModelPropertyName);
                if (!(_selectedRows is ObservableCollection<Guid>))
                    throw new Exception(nameof(bsTreeDataSourceToSchemaRolesBinder) + ": свойство '" + SelectedRowsModelPropertyName + "' должно быть типа '" + nameof(ObservableCollection<Guid>) + "'");
                selectedRows = (ObservableCollection<Guid>)_selectedRows;
            }


            //RootRole.Initialize();
            if (RootRole==null)
                throw new Exception(nameof(bsTreeDataSourceToSchemaRolesBinder) + ": при привязке свойства '" + SelectedRowsModelPropertyName + "' должна быть указана '" + nameof(RootRole) + "'");

            var jstree = new jsArray();
            AddRoleNode(jstree, RootRole);

            return "$('#" + Control.UniqueId + "').fancytree('option','source'," + jstree.ToJson() + ");";

        }

    }

}