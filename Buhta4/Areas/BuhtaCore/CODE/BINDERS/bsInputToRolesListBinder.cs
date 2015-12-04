using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{
    public class bsInputToRolesListBinder : OneWayBinder<object>
    {
        public SchemaBaseRole RootRole;
        public void SelectButtonClick(dynamic args)
        {

            var selectedList = Control.Model.GetPropertyValue(ModelPropertyName);
            if (selectedList is ObservableCollection<Guid>)
            {
                var model = new SelectSchemaRolesDialogModel(Control.Model.Controller, Control.Model, selectedList as ObservableCollection<Guid>, RootRole);
                var modal = Control.Model.CreateModal(@"~/Areas/BuhtaCore/Views/SelectSchemaRolesDialog.cshtml", model);
                modal.Show();
            }
            else
                throw new Exception(nameof(bsInputToRolesListBinder) + "." + nameof(GetJsForSettingProperty) + "(): привязанное свойство должено быть 'ObservableCollection<Guid>'");
        }

        public override string GetJsForSettingProperty()
        {
            var value = Control.Model.GetPropertyValue(ModelPropertyName);
            if (value is IEnumerable<Guid>)
            {
                var list = new List<SchemaBaseRole>();
                string errorStr = ""; ;
                foreach (var roleID in (value as IEnumerable<Guid>))
                {
                    if (SchemaBaseRole.Roles.ContainsKey(roleID))// && SchemaBaseRole.Roles[roleID].GetType is RootRole.GetType())
                        list.Add(SchemaBaseRole.Roles[roleID] as SchemaBaseRole);
                    else
                        errorStr += ", ?"+ roleID.ToString().Substring(0,6);
                }
                var sb = new StringBuilder();
                foreach (var tableRole in from role in list orderby role.Position, role.Name select role)
                {
                    sb.Append(tableRole.Name + ", ");
                }
                sb.RemoveLastChar(2);
                sb.Append(errorStr);
                return "$('#" + Control.UniqueId + "').val(" + sb.ToString().AsJavaScript() + ");";
            }
            else
                throw new Exception(nameof(bsInputToRolesListBinder) + "." + nameof(GetJsForSettingProperty) + "(): привязанное свойство должено быть 'IEnumerable<Guid>'");
        }

        public override void EmitBindingScript(StringBuilder script)
        {
            Control.Model.RegisterBinder(this);
            LastSendedText = GetJsForSettingProperty();
            script.AppendLine(LastSendedText);
        }

    }

}