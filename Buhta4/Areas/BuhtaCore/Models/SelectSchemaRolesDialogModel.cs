using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Buhta
{
    public class SelectSchemaRolesDialogModel : BaseModel
    {
        public bool NeedSave;

        public SelectSchemaRolesDialogModel(Controller controller) : base(controller)
        {
            SelectedRows = new ObservableCollection<Guid>();
            SelectedRows.CollectionChanged += SelectedRows_CollectionChanged;
        }

        public void OkButtonClick(dynamic args)
        {
            //EditedObject.Save;
        }

        public void CancelButtonClick(dynamic args)
        {
            //EditedObject.Save;
        }

        public void RowSelect(dynamic args)
        {
            Guid id = Guid.Parse(args.rowId.Value);

            if (args.isSelected.Value)
            {
                if (!SelectedRows.Contains(id))
                    SelectedRows.Add(id);
            }
            else
            {
                if (SelectedRows.Contains(id))
                    SelectedRows.Remove(id);
            }
        }

        public ObservableCollection<Guid> SelectedRows { get; set; }


        private void SelectedRows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            NeedSave = true;
            // TestProp1 = "выбрано " + SelectedRows.Count;
        }


    }
}
