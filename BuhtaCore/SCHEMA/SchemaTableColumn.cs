using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    public enum RegistrColumnRole { Реквизит, Измерение, Ресурс }

    [JsonObject(IsReference = true)]
    public class SchemaTableColumn  
    {
        private string name;
        [DisplayName("  Имя колонки"), Description("Имя колонки"), Category("  Колонка")]
        public string Name
        {
            get { return name; }
            set { name = value; firePropertyChanged("Name"); }
        }

        private string description;
        [DisplayName("Описание"), Description("Описание"), Category("  Колонка")]
        public string Description
        {
            get { return description; }
            set { description = value; firePropertyChanged("ID"); }
        }

        private RegistrColumnRole registrRole;
        [DisplayName("Тип поля"), Description("Тип поля"), Category("Регистр")]
        public RegistrColumnRole RegistrRole
        {
            get { return registrRole; }
            set { registrRole = value; firePropertyChanged("RegistrRole"); }
        }


        private bool isNotNullable;
        [DisplayName("Обязательное"), Description("Поле может быть заполнено (not null)"), Category("  Колонка")]
        public bool IsNotNullable
        {
            get { return isNotNullable; }
            set { isNotNullable = value; firePropertyChanged("IsNotNullable"); }
        }

        private bool isHidden;
        [DisplayName("Скрытое"), Description("Поле не показывается в форме редактирования"), Category("Форма редактирования")]
        public bool IsHidden
        {
            get { return isHidden; }
            set { isHidden = value; firePropertyChanged("IsHidden"); }
        }

        private bool isReadOnly;
        [DisplayName("Только-чтение"), Description("Поле не редактируется в формах."), Category("Форма редактирования")]
        public bool IsReadOnly
        {
            get { return isReadOnly; }
            set { isReadOnly = value; firePropertyChanged("IsReadOnly"); }
        }

        private int position;
        [Browsable(false)]
        public int Position
        {
            get { return position; }
            set { position = value; firePropertyChanged("Position"); }
        }

        private SchemaTable table;
        [Browsable(false)]
        public SchemaTable Table
        {
            get { return table; }
            set { table = value; firePropertyChanged("Table"); }
        }

        [Browsable(false)]
        [JsonIgnore]
        public string GetDataTypeDisplay
        {
            get
            {
                return "GetDataTypeDisplay";
                //return DataType == null ? "null" : DataType.GetNameDisplay;
            }
        }

        public SchemaTableColumn()
        {

        }

        public virtual void Validate(StringBuilder error)
        {
            if (string.IsNullOrWhiteSpace(Name))
                error.AppendLine("У колонки не заполнено поле 'Имя'.");

            if (Name != null && Name.Length > 128)
                error.AppendLine("Имя колонки длинее 128 символов: " + Name.Substring(0, 50).AsSQL());


            if (Table == null)
                error.AppendLine("У колонки не заполнено поле 'Таблица'.");


        }


        void ColumnRoles_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            firePropertyChanged("ColumnRoles");
        }


        public void firePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (table != null)
                table.firePropertyChanged("Columns");

        }


        public event PropertyChangedEventHandler PropertyChanged;

        public string GetDisplayName()
        {
            return Name;
        }

        public string Get4PartsTableName()
        {
            return "[?Get4PartsTableName?]";
        }



        public SchemaTable GetNativeTable()
        {
            return null;
        }

        public SchemaTableColumn GetNativeTableColumn()
        {
            return this;
        }



        public void EmitSelectSql(StringBuilder sql, string indent)
        {
            throw new NotImplementedException();
        }

        public void EmitJoinSql(StringBuilder sql, List<string> withCTE, string indent)
        {
            throw new NotImplementedException();
        }

        public string GetFullAlias()
        {
            return Name;
        }



        public bool GetIsSupportReadPermission()
        {
            return true;
        }

        public bool GetIsSupportInsertPermission()
        {
            return false;
        }

        public bool GetIsSupportUpdatePermission()
        {
            return true;
        }

        public bool GetIsSupportDeletePermission()
        {
            return false;
        }

        public bool GetIsSupportOwnedUpdatePermission()
        {
            return true;
        }

        public bool GetIsSupportOwnedDeletePermission()
        {
            return false;
        }

        public virtual string GetPermissionFolder()
        {
            return null;
        }

        public virtual Guid GetID()
        {
            return Table.ID;
        }

        public virtual string GetFieldName()
        {
            return Name;
        }

        public virtual string GetName()
        {
            return GetDisplayName();
        }

    }



}
