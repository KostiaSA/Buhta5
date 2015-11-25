using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Buhta
{
    public class bsInputRolesListBinder : BaseBsInputBinder
    {
        public bsInputRolesListBinder(string propertyName) : base(propertyName) { }

        //static BooleanBinder()
        //{
        //    BaseBinder.DefaultBinders.Add(typeof(Boolean), new BooleanBinder());
        //}

        //public override string GetDisplayText(object value)
        //{
        //    if (value is IEnumerable<Guid>)
        //    {
        //        var list = new List<Таблица_TableRole>();
        //        string errorStr = ""; ;
        //        foreach (var roleID in (value as IEnumerable<Guid>))
        //        {
        //            if (SchemaBaseRole.Roles.ContainsKey(roleID) && SchemaBaseRole.Roles[roleID] is Таблица_TableRole)
        //                list.Add(SchemaBaseRole.Roles[roleID] as Таблица_TableRole);
        //            else
        //                errorStr += ", ?ошибка";
        //        }
        //        var sb = new StringBuilder();
        //        foreach (var tableRole in from role in list orderby role.Position, role.Name select role)
        //        {
        //            sb.Append(tableRole.Name + ", ");
        //        }
        //        sb.RemoveLastChar(2);
        //        sb.Append(errorStr);
        //        return sb.ToString();
        //    }
        //    else
        //        throw new Exception(nameof(bsInputRolesListBinder)+ ".GetDisplayText(): тип value должен быть 'IEnumerable<Guid>'");

        //}

        public override object ParseDisplayText(string text)
        {
            return Boolean.Parse(text);
        }
    }
}