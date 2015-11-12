using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buhta
{
    [Export(typeof(SqlDataType))]
    public class RegistrSubcontoDataType : SqlDataType
    {
        public override string Name { get { return "Регистр.Субконто"; } }

        public ObservableCollection<Guid> AllowedSubcontos;

        public RegistrSubcontoDataType()
        {
            AllowedSubcontos = new ObservableCollection<Guid>();
            AllowedSubcontos.CollectionChanged += AllowedSubcontos_CollectionChanged;
        }

        void AllowedSubcontos_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            firePropertyChanged("AllowedSubcontos");
        }


        [Browsable(false)]
        public override string GetNameDisplay
        {
            get
            {
                return "Субконто (???,???)";
                //var table = App.Schema.GetObject<SchemaTable>((Guid)RefTableID);
                //return table == null ? "Ссылка-> ?null" : "Ссылка-> " + table.Name;
            }
        }

        public override Microsoft.SqlServer.Management.Smo.DataType GetSmoDataType()
        {
            return Microsoft.SqlServer.Management.Smo.DataType.UniqueIdentifier;
        }

    }
}
