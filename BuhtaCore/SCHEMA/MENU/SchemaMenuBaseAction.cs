using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Buhta
{

    [System.ComponentModel.TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
    [JsonObject(IsReference = true)]
    public class SchemaMenuBaseAction : INotifyPropertyChanged
    {

        private SchemaMenuBaseItem menuItem;
        [Browsable(false)]
        public SchemaMenuBaseItem MenuItem
        {
            get { return menuItem; }
            set { menuItem = value; firePropertyChanged("MenuItem"); }
        }

        [Browsable(false)]
        [JsonIgnore]
        public virtual string Name { get { return "SchemaMenuBaseAction"; } }

        [Browsable(false)]
        public virtual string GetFullName()
        {
            return Name;
        }


        public override string ToString()
        {
            return GetFullName();
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void firePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (menuItem != null)
                menuItem.firePropertyChanged("Action");
        }


        public virtual void Validate(StringBuilder error)
        {
            //if (MenuItem == null)
            //    error.AppendLine("У типа данных '" + Name + "' не заполнено поле 'Column'.");
        }

        public virtual void Execute()
        {
            throw new Exception("SchemaMenuBaseAction.Execute() не определен.");
        }

        public virtual SchemaMenuBaseAction Clone()
        {
            var clonedColumn = (SchemaMenuBaseAction)Activator.CreateInstance(GetType());
            return clonedColumn;
        }

    }



}
