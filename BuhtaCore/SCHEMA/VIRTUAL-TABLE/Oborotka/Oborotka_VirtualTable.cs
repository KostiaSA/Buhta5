using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Buhta
{
    [Export(typeof(SchemaVirtualTable))]
    public class Oborotka_VirtualTable : SchemaVirtualTable
    {
        public Oborotka_VirtualTable()
            : base()
        {
            ID = VirtualTableConst.Оборотка;
            Name = "$Оборотка";
            Description = "Оборотная ведомость";
            Position = 10;
            properties = new OborotkaProperties();

        }

        public override void CreateColumns()
        {
            base.CreateColumns();

            var c = new SchemaVirtualTableColumn();
            c.Table = this;
            c.Name = "Дата проводки";
            c.DataType = new DateDataType();
            Columns.Add(c);

            var props = Properties as OborotkaProperties;


            if (props.Registers.Count == 1)
            {
                int izmerCount = 0;
                int meraCount = 0;

                var registr = App.Schema.GetSampleObject<SchemaTable>(props.Registers[0]);

                foreach (var regCol in registr.Columns)
                {
                    if (regCol.ColumnRoles.Contains(RoleConst.Регистр_Измерение))
                    {
                        izmerCount++;

                        c = new SchemaVirtualTableColumn();
                        c.Table = this;
                        c.Name = regCol.Name;
                        c.DataType = regCol.DataType.Clone();
                        Columns.Add(c);

                    }
                }

                foreach (var regCol in registr.Columns)
                {
                    if (regCol.ColumnRoles.Contains(RoleConst.Регистр_Мера))
                    {
                        meraCount++;

                        if (props.ВхСальдо)
                        {
                            var meraCol = new OborotkaMeraColumn();
                            meraCol.Table = this;
                            meraCol.MeraType = OborotkaMeraType.ВхСальдо;
                            meraCol.Name = regCol.Name + " " + meraCol.MeraType;
                            meraCol.DataType = regCol.DataType.Clone();
                            Columns.Add(meraCol);
                        }
                        if (props.Дебет)
                        {
                            var meraCol = new OborotkaMeraColumn();
                            meraCol.Table = this;
                            meraCol.MeraType = OborotkaMeraType.Дебет;
                            meraCol.Name = regCol.Name + " " + meraCol.MeraType;
                            meraCol.DataType = regCol.DataType.Clone();
                            Columns.Add(meraCol);
                        }
                        if (props.Кредит)
                        {
                            var meraCol = new OborotkaMeraColumn();
                            meraCol.Table = this;
                            meraCol.MeraType = OborotkaMeraType.Кредит;
                            meraCol.Name = regCol.Name + " " + meraCol.MeraType;
                            meraCol.DataType = regCol.DataType.Clone();
                            Columns.Add(meraCol);
                        }
                        if (props.ИсхСальдо)
                        {
                            var meraCol = new OborotkaMeraColumn();
                            meraCol.Table = this;
                            meraCol.MeraType = OborotkaMeraType.ИсхСальдо;
                            meraCol.Name = regCol.Name + " " + meraCol.MeraType;
                            meraCol.DataType = regCol.DataType.Clone();
                            Columns.Add(meraCol);
                        }

                    }
                }
            }


        }

        public override void EmitJoinSql(StringBuilder sql, List<string> withCTE, string indent)
        {

            var props = Properties as OborotkaProperties;

          //  props.BegDate = new DateTime(2014, 1, 1);
          //  props.EndDate = new DateTime(2014, 1, 31);

            if (props.Registers.Count == 0)
            {
                MessageBox.Show("Не указан список регистров для оборотной ведомости.");
                return;
            }
            var cte_sql = new StringBuilder();
            cte_sql.AppendLine("[__obortka_temp__](");

            cte_sql.AppendLine("  [Регистр],");
            var table = App.Schema.GetSampleObject<SchemaTable>(props.Registers[0]);
            foreach (var registrCol in table.Columns)
            {
                if (registrCol.ColumnRoles.Contains(RoleConst.Регистр_Измерение))
                    cte_sql.AppendLine("  [" + registrCol.Name + "],");
            }
            foreach (var registrCol in table.Columns)
            {
                if (registrCol.ColumnRoles.Contains(RoleConst.Регистр_Мера))
                {
                    //if (props.ВхСальдо)
                    //    cte_sql.AppendLine("  0 AS [" + registrCol.Name + "__ВхСальдо],");
                    if (props.Дебет)
                        cte_sql.AppendLine("  [" + registrCol.Name + "__Дебет],");
                    if (props.Кредит)
                        cte_sql.AppendLine("  [" + registrCol.Name + "__Кредит],");
                    if (props.ИсхСальдо)
                        cte_sql.AppendLine("  [" + registrCol.Name + "__ИсхСальдо],");
                    cte_sql.AppendLine("  [" + registrCol.Name + "__before_today],");
                    cte_sql.AppendLine("  [" + registrCol.Name + "__today],");
                }
            }
            //            cte_sql.AppendLine("__ID__) ");
            cte_sql.RemoveLastChar(3);
            cte_sql.AppendLine(")");
            cte_sql.AppendLine("AS ");
            cte_sql.AppendLine("( ");


            int counter = 0;
            foreach (var registerID in props.Registers)
            {
                table = App.Schema.GetSampleObject<SchemaTable>(registerID);
                // первый select - остаток на сегодня (не EndDate), включает все проводки, что есть у данного регистра
                cte_sql.AppendLine("SELECT");
                counter++;

                cte_sql.AppendLine("  " + table.Name.AsSQL() + " AS [Регистр],");
                foreach (var registrCol in table.Columns)
                {
                    if (registrCol.ColumnRoles.Contains(RoleConst.Регистр_Измерение))
                        cte_sql.AppendLine("  [" + registrCol.Name + "] AS [" + registrCol.Name + "],");
                }
                foreach (var registrCol in table.Columns)
                {
                    if (registrCol.ColumnRoles.Contains(RoleConst.Регистр_Мера))
                    {
                        //if (props.ВхСальдо)
                        //    cte_sql.AppendLine("  0 AS [" + registrCol.Name + "__ВхСальдо],");
                        if (props.Дебет)
                            cte_sql.AppendLine("  0 AS [" + registrCol.Name + "__Дебет],");
                        if (props.Кредит)
                            cte_sql.AppendLine("  0 AS [" + registrCol.Name + "__Кредит],");
                        if (props.ИсхСальдо)
                            cte_sql.AppendLine("  0 AS [" + registrCol.Name + "__ИсхСальдо],");
                        cte_sql.AppendLine("  0 AS [" + registrCol.Name + "__before_today],");
                        cte_sql.AppendLine("  SUM([" + registrCol.Name + "]) AS [" + registrCol.Name + "__today],");
                    }
                }
                cte_sql.RemoveLastChar(3);
                cte_sql.AppendLine();
                cte_sql.AppendLine("FROM " + table.Get4PartsTableName());
                cte_sql.AppendLine("WHERE [" + table.GetColumnByRole(RoleConst.Регистр_ДбКр).Name + "] IN ('Д','К')");
                cte_sql.AppendLine("GROUP BY ");
                foreach (var registrCol in table.Columns)
                {
                    if (registrCol.ColumnRoles.Contains(RoleConst.Регистр_Измерение))
                        cte_sql.AppendLine("  [" + registrCol.Name + "],");
                }
                cte_sql.RemoveLastChar(3);
                cte_sql.AppendLine();

            }

            foreach (var registerID in props.Registers)
            {
                table = App.Schema.GetSampleObject<SchemaTable>(registerID);
                if (counter != 0)
                    cte_sql.AppendLine("UNION ALL");
                else
                    counter++;

                cte_sql.AppendLine("SELECT");

                cte_sql.AppendLine("  " + table.Name.AsSQL() + " AS [Регистр],");

                // измерения
                foreach (var registrCol in table.Columns)
                {
                    if (registrCol.ColumnRoles.Contains(RoleConst.Регистр_Измерение))
                        cte_sql.AppendLine("  [" + registrCol.Name + "],");
                }
                foreach (var registrCol in table.Columns)
                {
                    if (registrCol.ColumnRoles.Contains(RoleConst.Регистр_Мера))
                    {
                        //if (props.ВхСальдо)
                        //    cte_sql.AppendLine("  0 AS [" + registrCol.Name + "__ВхСальдо],");
                        if (props.Дебет)
                            cte_sql.AppendLine("  CASE WHEN [" + table.GetColumnByRole(RoleConst.Регистр_ДбКр).Name + "]='Д' AND [" + table.GetColumnByRole(RoleConst.Регистр_Дата).Name + "] BETWEEN " + props.BegDate.AsSQL() + " AND " + props.EndDate.AsSQL() + " THEN [" + registrCol.Name + "] ELSE 0 END  AS [" + registrCol.Name + "__Дебет],");
                        if (props.Кредит)
                            cte_sql.AppendLine("  CASE WHEN [" + table.GetColumnByRole(RoleConst.Регистр_ДбКр).Name + "]='К' AND [" + table.GetColumnByRole(RoleConst.Регистр_Дата).Name + "] BETWEEN " + props.BegDate.AsSQL() + " AND " + props.EndDate.AsSQL() + " THEN [" + registrCol.Name + "] ELSE 0 END  AS [" + registrCol.Name + "__Кредит],");
                        if (props.ИсхСальдо)
                            cte_sql.AppendLine("  0 AS [" + registrCol.Name + "__ИсхСальдо],");
                        cte_sql.AppendLine("  CASE WHEN [" + table.GetColumnByRole(RoleConst.Регистр_Дата).Name + "] > " + props.EndDate.AsSQL() + " THEN [" + registrCol.Name + "] ELSE 0 END  AS [" + registrCol.Name + "__before_today],");
                        cte_sql.AppendLine("  0 AS [" + registrCol.Name + "__today],");
                        //                        cte_sql.AppendLine("  " + table.Get4PartsTableName() + ".[" + registrCol.Name + "],");
                    }
                }
                cte_sql.RemoveLastChar(3);
                cte_sql.AppendLine();
                cte_sql.AppendLine("FROM "+table.Get4PartsTableName());
                cte_sql.AppendLine("WHERE [" + table.GetColumnByRole(RoleConst.Регистр_ДбКр).Name + "] IN ('Д','К') AND [" + table.GetColumnByRole(RoleConst.Регистр_Дата).Name + "] >= " + props.BegDate.AsSQL());

                //    cte_sql.AppendLine("WHERE [" + table.GetPrimaryKeyColumn().Name + "]=" + recordID.AsSQL());
            }
            cte_sql.AppendLine("),");


            // группировка
            cte_sql.AppendLine("[$Оборотка](");

            table = App.Schema.GetSampleObject<SchemaTable>(props.Registers[0]);
            cte_sql.AppendLine("  [Регистр],");
            foreach (var registrCol in table.Columns)
            {
                if (registrCol.ColumnRoles.Contains(RoleConst.Регистр_Измерение))
                    cte_sql.AppendLine("  [" + registrCol.Name + "],");
            }
            foreach (var registrCol in table.Columns)
            {
                if (registrCol.ColumnRoles.Contains(RoleConst.Регистр_Мера))
                {
                    if (props.ВхСальдо)
                        cte_sql.AppendLine("  [" + registrCol.Name + " ВхСальдо],");
                    if (props.Дебет)
                        cte_sql.AppendLine("  [" + registrCol.Name + " Дебет],");
                    if (props.Кредит)
                        cte_sql.AppendLine("  [" + registrCol.Name + " Кредит],");
                    if (props.ИсхСальдо)
                        cte_sql.AppendLine("  [" + registrCol.Name + " ИсхСальдо],");
                }
            }
            cte_sql.AppendLine("  __ID__");
            cte_sql.AppendLine(") ");
            cte_sql.AppendLine("AS");
            cte_sql.AppendLine("(");
            cte_sql.AppendLine("SELECT");
            cte_sql.AppendLine("  [Регистр],");

            foreach (var registrCol in table.Columns)
            {
                if (registrCol.ColumnRoles.Contains(RoleConst.Регистр_Измерение))
                    cte_sql.AppendLine("  [" + registrCol.Name + "],");
            }
            foreach (var registrCol in table.Columns)
            {
                if (registrCol.ColumnRoles.Contains(RoleConst.Регистр_Мера))
                {
                    if (props.ВхСальдо)
                        cte_sql.AppendLine("  SUM([" + registrCol.Name + "__today]-[" + registrCol.Name + "__before_today]-[" + registrCol.Name + "__Дебет]-[" + registrCol.Name + "__Кредит]) AS [" + registrCol.Name + ": ВхСальдо],");
                    if (props.Дебет)
                        cte_sql.AppendLine("  SUM([" + registrCol.Name + "__Дебет]) AS [" + registrCol.Name + ": Дебет],");
                    if (props.Кредит)
                        cte_sql.AppendLine(" -SUM([" + registrCol.Name + "__Кредит]) AS [" + registrCol.Name + ": Кредит],");
                    if (props.ИсхСальдо)
                        cte_sql.AppendLine("  SUM([" + registrCol.Name + "__today]-[" + registrCol.Name + "__before_today]) AS [" + registrCol.Name + ": ИсхСальдо],");
                }
            }
            cte_sql.AppendLine("  NEWID() AS __ID__");
            //cte_sql.RemoveLastChar(3);
            //cte_sql.AppendLine();
            cte_sql.AppendLine("FROM [__obortka_temp__]");
            cte_sql.AppendLine("GROUP BY");

            cte_sql.AppendLine("  [Регистр],");
            foreach (var registrCol in table.Columns)
            {
                if (registrCol.ColumnRoles.Contains(RoleConst.Регистр_Измерение))
                    cte_sql.AppendLine("  [" + registrCol.Name + "],");
            }
            cte_sql.RemoveLastChar(3);
            cte_sql.AppendLine();
            cte_sql.AppendLine("HAVING");
            foreach (var registrCol in table.Columns)
            {
                if (registrCol.ColumnRoles.Contains(RoleConst.Регистр_Мера))
                {
                    if (props.ВхСальдо)
                        cte_sql.AppendLine("  SUM([" + registrCol.Name + "__today]-[" + registrCol.Name + "__before_today]-[" + registrCol.Name + "__Дебет]+[" + registrCol.Name + "__Кредит])<>0 OR");
                    if (props.Дебет)
                        cte_sql.AppendLine("  SUM([" + registrCol.Name + "__Дебет])<>0 OR");
                    if (props.Кредит)
                        cte_sql.AppendLine("  SUM([" + registrCol.Name + "__Кредит])<>0 OR");
                    if (props.ИсхСальдо)
                        cte_sql.AppendLine("  SUM([" + registrCol.Name + "__today]-[" + registrCol.Name + "__before_today])<>0 OR");
                }
            }

            cte_sql.RemoveLastChar(5);

            cte_sql.AppendLine("),");

            withCTE.Add(cte_sql.ToString());

            sql.AppendLine("FROM [$Оборотка]");
        }

    }


    public class OborotkaIzmerColumn : SchemaVirtualTableColumn
    {

    }

    public enum OborotkaMeraType { ВхСальдо, ВхСальдоАктив, ВхСальдоПассив, Дебет, Кредит, ИсхСальдо, ИсхСальдоАктив, ИсхСальдоПассив }
    public class OborotkaMeraColumn : SchemaVirtualTableColumn
    {
        public OborotkaMeraType MeraType;
    }

    public class OborotkaProperties : SchemaVirtualTableProperties
    {
        public ObservableCollection<Guid> Registers { get; private set; }

        public DateTime BegDate;
        public DateTime EndDate;

        private bool вхСальдо;
        public bool ВхСальдо
        {
            get { return вхСальдо; }
            set { вхСальдо = value; firePropertyChanged("ВхСальдо"); }
        }

        private bool вхСальдо_Актив;
        public bool ВхСальдо_Актив
        {
            get { return вхСальдо_Актив; }
            set { вхСальдо_Актив = value; firePropertyChanged("ВхСальдо_Актив"); }
        }

        private bool вхСальдо_Пассив;
        public bool ВхСальдо_Пассив
        {
            get { return вхСальдо_Пассив; }
            set { вхСальдо_Пассив = value; firePropertyChanged("ВхСальдо_Пассив"); }
        }

        private bool дебет;
        public bool Дебет
        {
            get { return дебет; }
            set { дебет = value; firePropertyChanged("Дебет"); }
        }

        private bool кредит;
        public bool Кредит
        {
            get { return кредит; }
            set { кредит = value; firePropertyChanged("Кредит"); }
        }


        private bool исхСальдо;
        public bool ИсхСальдо
        {
            get { return исхСальдо; }
            set { исхСальдо = value; firePropertyChanged("ИсхСальдо"); }
        }

        private bool исхСальдо_Актив;
        public bool ИсхСальдо_Актив
        {
            get { return исхСальдо_Актив; }
            set { исхСальдо_Актив = value; firePropertyChanged("ИсхСальдо_Актив"); }
        }

        private bool исхСальдо_Пассив;
        public bool ИсхСальдо_Пассив
        {
            get { return исхСальдо_Пассив; }
            set { исхСальдо_Пассив = value; firePropertyChanged("ИсхСальдо_Пассив"); }
        }

        public OborotkaProperties()
        {
            Registers = new ObservableCollection<Guid>();
            Registers.CollectionChanged += Registers_CollectionChanged;
        }

        void Registers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            firePropertyChanged("Registers");
        }

        [JsonIgnore]
        public override string DisplayName
        {
            get
            {
                if (Registers == null || Registers.Count == 0)
                    return "";

                var list = new List<SchemaObject>();
                string errorStr = ""; ;
                foreach (var registrID in Registers)
                {
                    var obj = App.Schema.GetSampleObject<SchemaObject>(registrID);
                    //if (SchemaBaseRole.Roles.ContainsKey(objID) && SchemaBaseRole.Roles[objID] is Таблица_TableRole)
                    if (obj != null)
                    {
                        list.Add(obj);
                    }
                    else
                        errorStr += ", ?ошибка";
                }
                var sb = new StringBuilder();
                foreach (var obj in from o in list orderby o.Position, o.Name select o)
                {
                    sb.Append(obj.Name + ", ");
                }
                sb.RemoveLastChar(2);
                sb.Append(errorStr);
                return sb.ToString();
            }
        }

    }
}
