using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public class SchemaTableDetailOper_HelperTable : Base_HelperTable
    {
        public static Guid StaticID = Guid.Parse("609920C9-495E-4FAC-8DF8-6BA602B16B83");

        public SchemaTableDetailOper_HelperTable(Schema schema)
            : base(schema)
        {
            ID = StaticID;
            Name = "Конфигурация.Таблица.Деталь.БизнесОперация";

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

            c = new SchemaTableColumn();
            c.Name = "Деталь";
            c.DataType = new ForeingKeyDataType() { Column = c, RefTableID = SchemaTableDetail_HelperTable.StaticID };
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
                    foreach (var oper in detail.Opers)
                    {
                        sql.AppendLine("IF NOT EXISTS(SELECT ID FROM [" + Name + "] WHERE ID=" + oper.ID.AsSQL() + ") INSERT [" + Name + "] (ID) VALUES(" + oper.ID.AsSQL() + ")");
                        sql.Append("UPDATE [" + Name + "] SET ");
                        sql.Append("Имя=" + oper.Name.AsSQL() + ",");
                        sql.Append("Таблица=" + table.ID.AsSQL() + ",");
                        sql.Append("Деталь=" + detail.ID.AsSQL() + " ");
                        sql.AppendLine("WHERE ID=" + oper.ID.AsSQL());
                    }
                }
            }

            return sql.ToString();
        }

    }
}
