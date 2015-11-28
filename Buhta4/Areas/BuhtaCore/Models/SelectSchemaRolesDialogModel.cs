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
        ObservableCollection<Guid> EditedList;
        public SelectSchemaRolesDialogModel(Controller controller, BaseModel parentModel, ObservableCollection<Guid> editedList) : base(controller, parentModel)
        {
            SelectedRows = new ObservableCollection<Guid>();
            SelectedRows.CollectionChanged += SelectedRows_CollectionChanged;

            EditedList = editedList;

            foreach (var roleID in editedList)
                SelectedRows.Add(roleID);

            NeedSave = false;

        }

        public void OkButtonClick(dynamic args)
        {
            EditedList.Clear();
            foreach (var roleID in SelectedRows)
                EditedList.Add(roleID);
            Modal.Close();
        }

        public void CancelButtonClick(dynamic args)
        {
            Modal.Close();
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
