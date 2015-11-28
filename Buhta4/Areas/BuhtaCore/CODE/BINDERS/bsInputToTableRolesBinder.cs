using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{
    public class bsInputToTableRolesBinder : BaseBinder
    {
        public string ModelPropertyName;

        public override BinderEventMethod ModelEventMethod { get; set; }
        public override BinderSetMethod ModelSetMethod { get; set; }

        public void SelectButtonClick(dynamic args)
        {
            var model = new SelectSchemaRolesDialogModel(Control.Model.Controller);

            var selectedList = Control.Model.GetPropertyValue(ModelPropertyName);
            if (selectedList is ObservableCollection<Guid>)
            {
                foreach (var roleID in (selectedList as ObservableCollection<Guid>))
                {
                    model.SelectedRows.Add(roleID);
                }
                model.NeedSave = false;
            }
            else
                throw new Exception(nameof(bsInputToTableRolesBinder) + "." + nameof(GetJsForSettingProperty) + "(): привязанное свойство должено быть 'ObservableCollection<Guid>'");

            var modal =Control.Model.CreateModal(@"~/Areas/BuhtaCore/Views/SelectSchemaRolesDialog.cshtml", model);
            modal.Show();
        }

        public override string GetJsForSettingProperty()
        {
            var value = Control.Model.GetPropertyValue(ModelPropertyName);
            if (value is IEnumerable<Guid>)
            {
                var list = new List<Таблица_TableRole>();
                string errorStr = ""; ;
                foreach (var roleID in (value as IEnumerable<Guid>))
                {
                    if (SchemaBaseRole.Roles.ContainsKey(roleID) && SchemaBaseRole.Roles[roleID] is Таблица_TableRole)
                        list.Add(SchemaBaseRole.Roles[roleID] as Таблица_TableRole);
                    else
                        errorStr += ", ?ошибка";
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
                throw new Exception(nameof(bsInputToTableRolesBinder) + "." + nameof(GetJsForSettingProperty) + "(): привязанное свойство должено быть 'IEnumerable<Guid>'");
        }

        public override void EmitBindingScript(StringBuilder script)
        {
            Control.Model.RegisterBinder(this);
            LastSendedText = GetJsForSettingProperty();
            script.AppendLine(LastSendedText);
        }

    }

}