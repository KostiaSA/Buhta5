using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{

    [System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
    [JsonObject(IsReference = true)]
    [Serializable]
    public class SqlDataType : INotifyPropertyChanged
    {

        private SchemaTableColumn column;
        [Browsable(false)]
        public SchemaTableColumn Column
        {
            get { return column; }
            set { column = value; firePropertyChanged("Column"); }
        }

        [JsonIgnore]
        public virtual string Name { get { return "SqlDataType"; } }

        [JsonIgnore]
        public virtual string GetNameDisplay
        {
            get
            {
                return Name;
            }
        }


        public override string ToString()
        {
            return GetNameDisplay;
        }

        public virtual Microsoft.SqlServer.Management.Smo.DataType GetSmoDataType()
        {
            throw new Exception("Abstract method GetSmoDataType()");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void firePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (column != null)
                column.firePropertyChanged("DataType");
        }

        public virtual IEditControl GetEditControl()
        {
            throw new Exception("Abstract method GetEditControl()");
        }

        public virtual void Validate(ValidateErrorList errors)
        {
            if (Column == null)
                errors.AddError(Name,"У типа данных не заполнено поле 'Column'.");
        }

        public virtual SqlDataType Clone()
        {
            var clonedColumn = (SqlDataType)Activator.CreateInstance(GetType());
            clonedColumn.Column = Column;
            return clonedColumn;
        }

        public virtual string GetDeclareSql()
        {
            return "abstract error";
        }

        public virtual bool CanAcceptSqlValueFromDataType(SqlDataType valueDataType)
        {
            return GetType().Name == valueDataType.GetType().Name;
        }

        public virtual bool IsNumeric()
        {
            return false;
        }
    }



}
