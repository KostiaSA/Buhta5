using DevExpress.XtraTreeList;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{

    public enum Algorithm { Пусто = 0, ПолеИзТаблицы = 1, FIFO = 2 }

    public class SchemaTableProvodkaField : INotifyPropertyChanged
    {
        private DbKrSaldo dbKr;
        public DbKrSaldo DbKr
        {
            get { return dbKr; }
            set { dbKr = value; firePropertyChanged("DbKr"); }
        }

        private string registrFieldName;
        public string RegistrFieldName
        {
            get { return registrFieldName; }
            set { registrFieldName = value; firePropertyChanged("RegistrFieldName"); }
        }

        private string dataFieldName;
        public string DataFieldName
        {
            get { return dataFieldName; }
            set { dataFieldName = value; firePropertyChanged("DataFieldName"); }
        }

        private string dataAlgorithm;
        public string DataAlgorithm
        {
            get { return dataAlgorithm; }
            set { dataAlgorithm = value; firePropertyChanged("DataAlgorithm"); }
        }

        private string sqlText;
        public string SqlText
        {
            get { return sqlText; }
            set { sqlText = value; firePropertyChanged("SqlText"); }
        }

        private SchemaTableProvodka provodka;
        [Browsable(false)]
        public SchemaTableProvodka Provodka
        {
            get { return provodka; }
            set { provodka = value; /* не вызывать firePropertyChanged!*/}
        }

        public void firePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            if (provodka != null)
                provodka.firePropertyChanged("Fields");

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

   
}
