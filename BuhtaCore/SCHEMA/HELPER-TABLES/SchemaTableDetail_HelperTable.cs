using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class SchemaTableDetail_HelperTable : Base_HelperTable
    {
        public static Guid StaticID = Guid.Parse("8445D5D7-9F86-4A0E-B654-C4C9102650E3");

        public SchemaTableDetail_HelperTable(Schema schema)
            : base(schema)
        {
            ID = StaticID;
            Name = "Конфигурация.Таблица.Деталь";

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

            c = new SchemaTableColumn();
            c.Name = "Таблица";
            c.DataType = new ForeingKeyDataType() { Column = c, RefTableID = SchemaTable_HelperTable.StaticID };
            c.IsNotNullable = false;
            c.Table = this;
            Columns.Add(c);

        }

        public override string GetFillSql()
        {
            var sql = new StringBuilder();

            foreach (var table in schema.GetSampleObjects<SchemaTable>())
            {
                foreach (var detail in table.Details)
                {
                    sql.AppendLine("IF NOT EXISTS(SELECT ID FROM [" + Name + "] WHERE ID=" + detail.ID.AsSQL() + ") INSERT [" + Name + "] (ID) VALUES(" + detail.ID.AsSQL() + ")");
                    sql.Append("UPDATE [" + Name + "] SET ");
                    sql.Append("Имя=" + detail.Name.AsSQL() + ",");
                    sql.Append("Таблица=" + table.ID.AsSQL() + " ");
                    sql.AppendLine("WHERE ID=" + detail.ID.AsSQL());
                }
            }

            return sql.ToString();
        }

    }
}
