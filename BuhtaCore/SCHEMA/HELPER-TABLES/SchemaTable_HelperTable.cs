using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class SchemaTable_HelperTable : Base_HelperTable
    {
        public static Guid StaticID = Guid.Parse("F759E782-638E-437F-85AE-BC4EC0AD9FED");

        public SchemaTable_HelperTable(Schema schema)
            : base(schema)
        {
            ID = StaticID;
            Name = "Конфигурация.Таблица";

            SchemaTableColumn c;

            c = new SchemaTableColumn();
            c.Name = "ID";
            c.DataType = new GuidDataType() { Column = c };
            c.ColumnRoles.Add(RoleConst.Таблица_Ключ);
            c.IsNotNullable = true;
            c.Table = this;
            Columns.Add(c);

            c = new SchemaTableColumn();
            c.Name = "Имя";
            c.DataType = new StringDataType() { Column = c, MaxSize = 128 };
            c.IsNotNullable = false;
            c.Table = this;
            Columns.Add(c);

        }

        public override string GetFillSql()
        {
            var sql = new StringBuilder();

            foreach (var table in schema.GetSampleObjects<SchemaTable>())
            {
                sql.AppendLine("IF NOT EXISTS(SELECT ID FROM [" + Name + "] WHERE ID=" + table.ID.AsSQL() + ") INSERT [" + Name + "] (ID) VALUES(" + table.ID.AsSQL() + ")");
                sql.Append("UPDATE [" + Name + "] SET ");
                sql.Append("Имя=" + table.Name.AsSQL() + " ");
                sql.AppendLine("WHERE ID=" + table.ID.AsSQL());
            }

            return sql.ToString();
        }

    }
}
